using UnityEngine;
using WebSocketSharp;

public class HololensWebsocketClient : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        ws = new WebSocket("ws://192.168.137.1:8080");
        ws.OnMessage += (sender, e) =>
        {
             Debug.Log("Message received from server: " + e.Data);
            ProcessCommand(e.Data);
        };
        ws.Connect();
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
        if (command == "increase_distance")
        {
            Debug.Log("increase distance command received!");
        }

        else if (command == "decrease_distance")
        {
            Debug.Log("decrease distance command received");
        }
    }
}