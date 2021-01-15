using System.Collections;
using UnityEngine;
public class DebugToScreen : MonoBehaviour
{
    private GUIStyle guiStyle = new GUIStyle();

    private string myLog;
    private Queue myLogQueue = new Queue();

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = string.Format("[{0}] {1}\n", type, myLog);
        myLogQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue(newString);
        }
        
        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
    }
    
    private void OnGUI()
    {
        guiStyle.fontSize = 20;
        GUILayout.Label(myLog, guiStyle);
    }
}