 /*
TODO:
- Remove blurriness from the video feed when the resolution has been changed
- Find a way to diplay Mixed Reality Capture data on the web app itself. If not possible
upload a schematic of the current status of the app.
- Find a way to take into account the tilt of the Hololens for patients wearing spectacles. The tilt 
changes the apparent position of the Holograms.
- Implement port forwarding such that the web app is accessible from other devices as well
- Implement grayscale shader on the PV image as an additional mode.
*/
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
    public GameObject depthtargetObject;
    private Orbital orbital;
    public GameObject pv_image_left;
    public GameObject pv_image_right;
    private Renderer pvLeftQuad;
    private Renderer pvRightQuad;  
    private Renderer depthQuad;
    public Shader grayscale_shader;
    public Material colormap_material;  
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
    private Material grayscale_mat;
    private RenderTexture pv_tex_r;
    private Texture pv_tex_current;
    private bool isGrayscale = false;
    private bool isNightmode = false;
    private int isfirstToggle = 0;

    private Renderer activeQuad;    

    void Start()
    {
        
        InitializeConnection();
        InitializeQuadPosition();
        InitializeOrbital();  
        InitializeQuads();       

        activeQuad = isNightmode ? depthQuad : pvLeftQuad;
        grayscale_mat = new Material(grayscale_shader);
        pv_tex_r = new RenderTexture(640, 360, 0, RenderTextureFormat.BGRA32);
        pv_tex_current = pvLeftQuad.material.mainTexture;

        SetupLimits(pvLeftQuad);
        SetupLimits(depthQuad);
        SetQuadPosition(activeQuad, "bottom_left");
        
    }

    private void InitializeConnection()
    {
        Debug.Log("Starting WebSocket connection...");
        ws = new WebSocket("wss://hololens-sense-9bd80b459134.herokuapp.com/");
        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;        
        ws.SslConfiguration.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

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
    }

    private void InitializeOrbital()
    {
        orbital = targetObject.GetComponent<Orbital>();  
        if (orbital == null)
        {
            Debug.LogError("Orbital component not found on targetObject.");
        }   
    }

    private void InitializeQuads()
    {
        Debug.Log("PV LEFT status: " + pv_image_left.activeSelf);
        Debug.Log("PV RIGHT status " + pv_image_right.activeSelf);
        Debug.Log("DEPTH QUAD status" + depthtargetObject.activeSelf);
        pvLeftQuad = pv_image_left.GetComponent<Renderer>();
        pvRightQuad = pv_image_right.GetComponent<Renderer>();
        depthQuad = depthtargetObject.GetComponent<Renderer>();
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

        if (isGrayscale)
        {
            Graphics.Blit(pv_tex_current, pv_tex_r, grayscale_mat);
            pvLeftQuad.material.mainTexture = pv_tex_r;
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
            isNightmode = !isNightmode;
            actionQueue.Enqueue(() => ToggleRenderer(isNightmode));
        }        

        else if (command.StartsWith("slider_value:"))
        {
            string valueStr = command.Substring("slider_value:".Length);
            if (float.TryParse(valueStr, out float value))
            {
                Debug.Log("Slider value received: " + value);
                actionQueue.Enqueue(() => ResizeQuad(activeQuad, value));
            }
            else
            {
                Debug.LogError("Failed to parse slider value");
            }
        }

        else if (command.StartsWith("move_"))
        {
            Debug.Log(command + "command recieved");
            actionQueue.Enqueue(() => UpdateCoordinates(activeQuad, command));           
        }

        else if (command.StartsWith("bottom_") || command.StartsWith("top_"))
        {
            Debug.Log(command + "command received");
            actionQueue.Enqueue(() => SetQuadPosition(activeQuad, command));
        }

        else if (command.StartsWith("filter_"))
        {
            Debug.Log(command + "command received");
            actionQueue.Enqueue(() => UpdateFilterMode(command));
        }

        else if (command == "toggle_grayscale")
        {
            Debug.Log(command + "command received");
            actionQueue.Enqueue(() => ToggleGrayscaleMap());
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

    private void ToggleRenderer(bool isNightmode)
    {
        activeQuad = isNightmode ? depthQuad : pvLeftQuad;
        isfirstToggle += 1;        
        if (isNightmode)
        {            
            depthtargetObject.SetActive(true);
            pv_image_left.SetActive(false);
            if (isfirstToggle == 1)
            {
                ResizeQuad(activeQuad, 0.5f);
                SetQuadPosition(activeQuad, "bottom_right");
            }
            Debug.Log("Setting Quad to false");
        }
        else
        {
            pv_image_left.SetActive(true);
            depthtargetObject.SetActive(false);
            Debug.Log("Setting Quad to true");
        }
        isNightmode = !isNightmode;
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
        float aspectRatio = 16f/9f;        
        Vector3 currentScale = quadMesh.transform.localScale;
        float newSizeX = Mathf.Lerp(lowerXlimit, upperXlimit, value) * aspectRatio;
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
        bottomLeft = new Vector3(-0.39f,-0.42f,-0.02f);
        bottomRight = new Vector3(0.27f,-0.42f,-0.02f);
        topLeft = new Vector3(-0.39f,0.14f,-0.02f);
        topRight = new Vector3(0.27f,0.14f,-0.02f);
    }
    private void SetQuadPosition(Renderer quad, string position)
    {
        switch (position)
        {
            case "bottom_left":
                quad.transform.localPosition = bottomLeft;
                break;
            case "bottom_right":
                quad.transform.localPosition = bottomRight;
                break;
            case "top_left":
                quad.transform.localPosition = topLeft;
                break;
            case "top_right":
                quad.transform.localPosition = topRight;
                break;
            default:
                Debug.Log("Invalid command received: " + position);
                break;
        }
    }

    private void UpdateFilterMode(string command)
    {
        FilterMode currentFiltermode = pvLeftQuad.material.mainTexture.filterMode;
        Debug.Log("Current filter mode: " + currentFiltermode);

        switch (command)
        {
            case "point_filter":
                pvLeftQuad.material.mainTexture.filterMode = FilterMode.Point;
                break;
            case "bilinear_filter":
                pvLeftQuad.material.mainTexture.filterMode = FilterMode.Bilinear;
                break;
            case "trilinear_filter":
                pvLeftQuad.material.mainTexture.filterMode = FilterMode.Trilinear;
                break;
            default:
                Debug.Log("Invalid command received");
                break;
        }
    }

    private void ToggleGrayscaleMap()
    {
        isGrayscale = !isGrayscale;

        if (!isGrayscale)
        {
            pvLeftQuad.material.mainTexture = pv_tex_current;            
        }        
    }   
    
}