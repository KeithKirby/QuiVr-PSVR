using UnityEngine;
using ObserVR;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class ObserVR_ErrorReporter : MonoBehaviour {
    private static ObserVR_ProjectSettings settings;
    private static List<string> _logs;

    void Start() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");
        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            Debug.LogError("Couldn't find settings file or Application ID is missing...stopping analytics");
            ObserVR_Analytics.AnalyticsEnabled = false;
        }
        else {
            _logs = new List<string>();
        }
    }

    void OnEnable() {
        Application.logMessageReceived += LogHandler;
    }

    void OnDisable() {
        Application.logMessageReceived -= LogHandler;
    }

    void LogHandler(string logString, string stackTrace, LogType type) {
        if (!string.IsNullOrEmpty(ObserVR_Session.Instance.sessionID)) {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning) {
                if (!_logs.Contains(logString + stackTrace)) {
                    _logs.Add(logString + stackTrace);
                    ObserVR_Events.CustomEvent("LOG")
                        .AddParameter("type", type.ToString())
                        .AddParameter("message", logString)
                        .AddParameter("stacktrace", stackTrace.Replace("\n", " "))
                        .EndParameters();
                    if (type == LogType.Error || type == LogType.Exception) {
                        if (settings.debugMode) {
                            if (settings.sendEventsOnDebugMode) {
                                ObserVR_EventHandler.Instance.SendEvents();
                            }
                        }
                        else {
                            ObserVR_EventHandler.Instance.SendEvents();
                        }
                    }
                }
            }
        }
    }
}
