using UnityEngine;
using WebSocketSharp;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Concurrent;

public class HololensWebsocketClient : MonoBehaviour
{
    private WebSocket ws;
    // public PinchSlider distanceSlider;
    public GameObject targetObject;
    private Orbital orbital;
    public GameObject pv_image_left;
    public GameObject pv_image_right;
    private Renderer pvLeftQuad;
    private Renderer pvRightQuad;    
    private ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();

    [SerializeField]
    public float minscaleFactorX = 0.2f;
    [SerializeField]
    public float maxscaleFactorX = 1.4f;
    [SerializeField]
    public float minscaleFactorZ = 0.2f;
    [SerializeField]
    public float maxscaleFactorZ = 1.4f;
    private float lowerXlimit;
    private float upperXlimit;
    private float lowerZlimit;
    private float upperZlimit;
    private Vector3 bottomLeft;
    private Vector3 bottomRight;
    private Vector3 topLeft;
    private Vector3 topRight;

    void Start()
    {
        Debug.Log("Starting WebSocket connection...");
        ws = new WebSocket("ws://172.20.10.8:8080");
        ws.OnOpen += (sender, e) => Debug.Log("WebSocket connection opened.");
        ws.OnError += (sender, e) => Debug.LogError("WebSocket error: " + e.Message);
        ws.OnClose += (sender, e) => Debug.Log("WebSocket connection closed: " + e.Reason);
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message received from server: " + e.Data);
            try
            {
                ProcessCommand(e.Data);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during ProcessCommand: {ex}");
            }
            
        };
        ws.Connect();
        InitializeQuadPosition();
        orbital = targetObject.GetComponent<Orbital>();  
        if (orbital == null)
        {
            Debug.LogError("Orbital component not found on targetObject.");
        }     

        Debug.Log("PV LEFT status: " + pv_image_left.activeSelf);
        Debug.Log("PV RIGHT status " + pv_image_right.activeSelf);

        pvLeftQuad = pv_image_left.GetComponent<Renderer>();
        pvRightQuad = pv_image_right.GetComponent<Renderer>();

        SetupLimits(pvLeftQuad);
        SetupLimits(pvRightQuad);
        SetQuadPosition("bottom_left");
        
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }

    void Update()
    {
        // Process actions from the queue
        while (actionQueue.TryDequeue(out var action))
        {
            action();
        }
    }


    private void ProcessCommand(string command)
    {
        Debug.Log("Processing command: " + command);
        if (command == "increase_distance")
        {
            Debug.Log("increase distance command received!");
            AdjustDistance(0.1f);
        }

        else if (command == "decrease_distance")
        {
            Debug.Log("decrease distance command received");
            AdjustDistance(-0.1f);
        }

        else if (command == "toggle_left")
        {
            Debug.Log("toggle left command received");
            actionQueue.Enqueue(() => ToggleRenderer(pv_image_left));
        }

        else if (command == "toggle_right")
        {
            Debug.Log("toggle right command received");
            actionQueue.Enqueue(() => ToggleRenderer(pv_image_right));
        }

        else if (command.StartsWith("slider_value:"))
        {
            string valueStr = command.Substring("slider_value:".Length);
            if (float.TryParse(valueStr, out float value))
            {
                Debug.Log("Slider value received: " + value);
                actionQueue.Enqueue(() => ResizeQuad(pvLeftQuad, value));
            }
            else
            {
                Debug.LogError("Failed to parse slider value");
            }
        }

        else if (command.StartsWith("move_"))
        {
            Debug.Log(command + "command recieved");
            actionQueue.Enqueue(() => UpdateCoordinates(pvLeftQuad, command));           
        }

        else if (command.StartsWith("bottom_") || command.StartsWith("top_"))
        {
            Debug.Log(command + "command received");
            actionQueue.Enqueue(() => SetQuadPosition(command));
        }
    }

    private void AdjustDistance(float amount)
    {
        Debug.Log("Adjusting distance by: " + amount);
        if (orbital != null)
        {
            Vector3 localOffset = orbital.LocalOffset;
            localOffset.z = Mathf.Clamp(localOffset.z + amount, 1.0f, 2.0f); // Adjust limits as needed
            orbital.LocalOffset = localOffset;

            // Update the slider value to reflect the new distance
            // if (distanceSlider != null)
            // {
            //     distanceSlider.SliderValue = (localOffset.z - 1.0f) / (2.0f - 1.0f);
            // }
        }
    }

    private void ToggleRenderer(GameObject quad)
    {
        bool currentStatus = quad.activeSelf;
        if (currentStatus)
        {            
            quad.SetActive(false);
            Debug.Log("Setting Quad to false");
        }
        else
        {
            quad.SetActive(true);
            Debug.Log("Setting Quad to true");
        }
    }

    private void UpdateCoordinates(Renderer quadMesh, string command)
    {
        Vector3 positionCoords = quadMesh.transform.position;

        switch(command)
        {
            case "move_right":
                positionCoords.x += 0.1f;
                break;
            case "move_down":
                positionCoords.y -= 0.1f;
                break;
            case "move_up":
                positionCoords.y += 0.1f;
                break;
            case "move_left":
                positionCoords.x -= 0.1f;
                break;
            default:
                Debug.Log("Invalid command received" + command);
                break;
        }
        quadMesh.transform.position = positionCoords;
    }
    private void ResizeQuad(Renderer quadMesh, float value)
    {
        Vector3 currentScale = quadMesh.transform.localScale;
        float newSizeX = Mathf.Lerp(lowerXlimit, upperXlimit, value);
        float newSizeZ = Mathf.Lerp(lowerZlimit, upperZlimit, value); 
        Vector3 newScale = new Vector3(newSizeX, currentScale.y, newSizeZ);
        quadMesh.transform.localScale = newScale;

    }
    private void SetupLimits(Renderer quadMesh)
    {
        float intialXscale = quadMesh.transform.localScale.x;
        float initialZscale = quadMesh.transform.localScale.z;

        lowerXlimit = (float)(1.0 - minscaleFactorX) * intialXscale;
        upperXlimit = (float)(1.0 + maxscaleFactorX) * intialXscale;
        lowerZlimit = (float)(1.0 - minscaleFactorZ) * initialZscale;
        upperZlimit = (float)(1.0 + maxscaleFactorZ) * initialZscale;
        
    }
    private void InitializeQuadPosition()
    {
        // bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(-0.39f,-0.42f,-0.02f));
        // bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(0.27f,-0.42f,-0.02f));
        // topLeft = Camera.main.ViewportToWorldPoint(new Vector3(-0.39f,0.14f,-0.02f));
        // topRight = Camera.main.ViewportToWorldPoint(new Vector3(0.27f,0.14f,-0.02f));
        
        bottomLeft = new Vector3(-0.39f,-0.42f,-0.02f);
        bottomRight = new Vector3(0.27f,-0.42f,-0.02f);
        topLeft = new Vector3(-0.39f,0.14f,-0.02f);
        topRight = new Vector3(0.27f,0.14f,-0.02f);
    }
    private void SetQuadPosition(string position)
    {
        switch (position)
        {
            case "bottom_left":
                pv_image_left.transform.localPosition = bottomLeft;
                break;
            case "bottom_right":
                pv_image_left.transform.localPosition = bottomRight;
                break;
            case "top_left":
                pv_image_left.transform.localPosition = topLeft;
                break;
            case "top_right":
                pv_image_left.transform.localPosition = topRight;
                break;
            default:
                Debug.Log("Invalid command received: " + position);
                break;
        }
    }
    
}