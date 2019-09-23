using UnityEngine;
using System;
using System.IO;

public class Dbg : MonoBehaviour
{
    public string LogFile = "log.txt";
    public bool EchoToConsole = true;
    public bool AddTimeStamp = true;

    private StreamWriter OutputStream;

    void Awake()
    {
        if(OutputStream == null)
            OutputStream = new StreamWriter(LogFile, true);
    }

    void OnDestory()
    {
        if (OutputStream != null)
        {
            OutputStream.Close();
            OutputStream = null;
        }
    }

    private void Write(string message, string stacktrace)
    {
        if (AddTimeStamp)
        {
            DateTime now = DateTime.Now;
            message = string.Format("[{0:H:mm:ss}] {1}", now, message);
        }

        if (OutputStream != null)
        {
            OutputStream.WriteLine(message);
            OutputStream.WriteLine(stacktrace);
            OutputStream.Flush();
        }
    }

    void OnEnable() {
        Application.RegisterLogCallback(HandleLog);
        if(OutputStream == null)
            OutputStream = new StreamWriter(LogFile, true);
    }
    void OnDisable() {
        Application.RegisterLogCallback(null);
        if (OutputStream != null)
        {
            OutputStream.Close();
            OutputStream = null;
        }
    }
    void HandleLog(string logString, string stackTrace, LogType type) {
        Write(logString, stackTrace);
    }

}