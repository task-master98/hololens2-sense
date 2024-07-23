using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class DebugLogger : MonoBehaviour
{
    public Text logText;
    private Queue<string> logQueue = new Queue<string>();
    private const int maxLogCount = 50;

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"{type}: {logString}";
        if (type == LogType.Exception || type == LogType.Error)
        {
            logEntry += $"\nStack Trace: {stackTrace}";
        }

        logQueue.Enqueue(logEntry);
        if (logQueue.Count > maxLogCount)
        {
            logQueue.Dequeue();
        }

        UpdateLogText();
    }

    private void UpdateLogText()
    {
        logText.text = string.Join("\n", logQueue.ToArray());
    }
    
}
