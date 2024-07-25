using UnityEngine;
using WebSocketSharp;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class HololensWebsocketClient : MonoBehaviour
{
    private WebSocket ws;
    public PinchSlider distanceSlider;
    public GameObject targetObject;
    private Orbital orbital;

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
            ProcessCommand(e.Data);
        };
        ws.Connect();
        orbital = targetObject.GetComponent<Orbital>();
        if (orbital == null)
        {
            Debug.LogError("Orbital component not found on targetObject.");
        }

        // Add listener to the PinchSlider
        distanceSlider.OnValueUpdated.AddListener(OnSliderUpdated);
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
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
            if (distanceSlider != null)
            {
                distanceSlider.SliderValue = (localOffset.z - 1.0f) / (2.0f - 1.0f);
            }
        }
    }

    private void OnSliderUpdated(SliderEventData eventData)
    {
        float distance = Mathf.Lerp(1.0f, 2.0f, eventData.NewValue); // Map slider value to desired range
        if (orbital != null)
        {
            Vector3 localOffset = orbital.LocalOffset;
            localOffset.z = distance;
            orbital.LocalOffset = localOffset;
        }
    }
}