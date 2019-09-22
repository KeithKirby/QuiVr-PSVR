using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[DisallowMultipleComponent]
public class ObserVR_EventHandler : MonoBehaviour {
    public static ObserVR_EventHandler Instance { get; private set; }
    private static ObserVR_ProjectSettings settings;
    private static List<string> eventQueue;
    private static bool debugMode;
    private static bool prettyDebug;
    private static bool sendEventsOnDebugMode;
    private static string applicationID;

    [HideInInspector]
    public bool ObserVR_Initialized = false;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }

    void Start() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");

        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            Debug.LogError("Couldn't find settings file or Application ID is missing...stopping analytics");
            ObserVR_Analytics.AnalyticsEnabled = false;
        }
        else {
            eventQueue = new List<string>();
            debugMode = settings.debugMode;
            prettyDebug = settings.prettyDebug;
            sendEventsOnDebugMode = settings.sendEventsOnDebugMode;
            if (!string.IsNullOrEmpty(settings.applicationID) && !string.IsNullOrEmpty(settings.applicationID_Dev)) {
                applicationID = settings.environment == ObserVR_ProjectSettings.Environment.Dev ? settings.applicationID_Dev : settings.applicationID;
            }
            else {
                applicationID = settings.applicationID;
            }
            ObserVR_Initialized = true;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.Android) {
                if (ObserVR_API.StoredDataExists()) {
                    SendStoredData(applicationID);
                }
            }
        }
    }

    private void SendStoredData(string applicationID) {
        List<string> storedData = ObserVR_API.ReadDataFromFile();
        if (storedData != null) {
            foreach (string batch in storedData) {
                StartCoroutine(ObserVR_API.SendStoredDataPost(applicationID, batch));
            }
        }
        // Don't want to build up data on disk
        ObserVR_API.DeleteStoredData();
    }

    public void AddEvent(ObserVR_CustomEvent e) {
        if (!ObserVR_Analytics.AnalyticsEnabled) {
            Debug.LogError("Could not find ObserVR_Analytics prefab. Make sure it is present and enabled in the scene");
            return;
        }
        if (string.IsNullOrEmpty(e.user)) {
            e.user = ObserVR_User.Instance.userID;
        }
        if (string.IsNullOrEmpty(e.session)) {
            e.session = ObserVR_Session.Instance.sessionID;
        }
        string toJSON = JsonUtility.ToJson(e);
        toJSON = e.parameters != null ? ObserVR_Functions.JSONifyEvent(toJSON) : ObserVR_Functions.JSONifyEvent(toJSON, true);

        if (debugMode) {
#if UNITY_EDITOR || UNITY_WEBGL
            Debug.Log(prettyDebug ? PrettyDebug(e) : toJSON);
#endif
            if (sendEventsOnDebugMode) {
                eventQueue.Add(toJSON);
                if (eventQueue.Count >= settings.batchSize && !ObserVR_Session.Instance.isQuitting) {
                    SendEvents();
                }
            }
        }
        else {
            eventQueue.Add(toJSON);
            if (eventQueue.Count >= settings.batchSize && !ObserVR_Session.Instance.isQuitting) {
                SendEvents();
            }
        }
    }

    private static string PrettyDebug(ObserVR_CustomEvent e) {
        string print = string.Format("[{0}] ", ObserVR_Functions.GetCurrentTime().ToLocalTime().ToLongTimeString());
        switch (e.type) {
            case "SESSION_START":
                print += "Session started";
                break;
            case "SESSION_END":
                print += "Session ended";
                break;
            case "GENERAL_FOV":
                print += string.Format("User's gaze hit {0} at point ({1}, {2}, {3})", e.parameters.ToArray()[0].value, float.Parse(e.parameters.ToArray()[1].value, CultureInfo.InvariantCulture).ToString("0.000"), float.Parse(e.parameters.ToArray()[2].value, CultureInfo.InvariantCulture).ToString("0.000"), float.Parse(e.parameters.ToArray()[3].value, CultureInfo.InvariantCulture).ToString("0.000"));
                break;
            case "OBJECT_FOV":
                print += string.Format("User looked at {0} for {1} seconds", e.parameters.ToArray()[0].value, (Convert.ToDateTime(e.timestamp) - Convert.ToDateTime(e.parameters.ToArray()[1].value)).TotalSeconds.ToString("0.00"));
                break;
            case "TELEPORT":
                print += string.Format("User teleported to ({0}, {1}, {2}) with the {3} on the {4} hand", float.Parse(e.parameters.ToArray()[0].value, CultureInfo.InvariantCulture).ToString("0.000"), float.Parse(e.parameters.ToArray()[1].value, CultureInfo.InvariantCulture).ToString("0.000"), float.Parse(e.parameters.ToArray()[2].value, CultureInfo.InvariantCulture).ToString("0.000"), e.parameters.ToArray()[3].value, e.parameters.ToArray()[4].value);
                break;
            case "POSITION":
                print += string.Format("User is at ({0}, {1}, {2})", float.Parse(e.parameters.ToArray()[0].value, CultureInfo.InvariantCulture).ToString("0.000"), float.Parse(e.parameters.ToArray()[1].value, CultureInfo.InvariantCulture).ToString("0.000"), float.Parse(e.parameters.ToArray()[2].value, CultureInfo.InvariantCulture).ToString("0.000"));
                break;
            case "PHYSICAL_POSITION":
                print += string.Format("User's physical location is at ({0}, {1}, {2})", float.Parse(e.parameters.ToArray()[0].value, CultureInfo.InvariantCulture).ToString("0.000"), float.Parse(e.parameters.ToArray()[1].value, CultureInfo.InvariantCulture).ToString("0.000"), float.Parse(e.parameters.ToArray()[2].value, CultureInfo.InvariantCulture).ToString("0.000"));
                break;
            case "PLAYSPACE_SIZE":
                print += string.Format("Current playspace size is {0}m x {1}m", e.parameters.ToArray()[0].value, e.parameters.ToArray()[1].value);
                break;
            case "TOUCH":
                print += string.Format("User touched {0} for {1} seconds", e.parameters.ToArray()[0].value, (Convert.ToDateTime(e.timestamp) - Convert.ToDateTime(e.parameters.ToArray()[1].value)).TotalSeconds.ToString("0.00"));
                break;
            case "GRAB":
                print += string.Format("User grabbed {0} for {1} seconds", e.parameters.ToArray()[0].value, (Convert.ToDateTime(e.timestamp) - Convert.ToDateTime(e.parameters.ToArray()[1].value)).TotalSeconds.ToString("0.00"));
                break;
            case "USE":
                print += string.Format("User used {0} for {1} seconds", e.parameters.ToArray()[0].value, (Convert.ToDateTime(e.timestamp) - Convert.ToDateTime(e.parameters.ToArray()[1].value)).TotalSeconds.ToString("0.00"));
                break;
            case "FPS":
                print += string.Format("Current framerate is {0} FPS", e.parameters.ToArray()[0].value);
                break;
            case "SCENE_LOADED":
                print += string.Format("{0} loaded", e.parameters.ToArray()[0].value);
                break;
            case "SCENE_UNLOADED":
                print += string.Format("{0} unloaded", e.parameters.ToArray()[0].value);
                break;
            case "HEADSET_OFF":
                print += "Headset was taken off";
                break;
            case "HEADSET_ON":
                print += "Headset was put on";
                break;
            case "LOST_TRACKING":
                print += "Headset has lost tracking";
                break;
            case "REGAINED_TRACKING":
                print += "Headset has regained tracking";
                break;
            default:
                print += string.Format("Event of type \"{0}\" happened with the following parameters: ", e.type);
                if (e.parameters == null) {
                    print += "None";
                }
                else {
                    for (int i = 0; i < e.parameters.Count; i++) {
                        if (i == e.parameters.Count - 1) {
                            print += string.Format("[Key: {0}, Value: {1}]", e.parameters.ToArray()[i].key, e.parameters.ToArray()[i].value);
                        }
                        else {
                            print += string.Format("[Key: {0}, Value: {1}], ", e.parameters.ToArray()[i].key, e.parameters.ToArray()[i].value);
                        }
                    }
                }
                break;
        }
        return print;
    }

    public void SendEvents() {
#if UNITY_EDITOR || UNITY_WEBGL
        if (debugMode) {
            Debug.Log("Sending " + eventQueue.Count + " events...");
        }
#endif
        string events = ObserVR_Functions.PackageEvents(eventQueue);

#if UNITY_STANDALONE || UNITY_WSA
        if (!ObserVR_Session.Instance.isQuitting) {
            if (settings.offlineMode) {
                ObserVR_API.OfflineMode(events, null);
            }
            else {
                StartCoroutine(ObserVR_API.PostRequest(applicationID, events, debugMode));
            }
            eventQueue.Clear();
        }
        else {
            if (settings.offlineMode) {
                ObserVR_API.OfflineMode(events, Application.Quit);
            }
            else {
                ObserVR_API.PostRequest(applicationID, events, Application.Quit, debugMode);
            }
        }
#elif UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        StartCoroutine(ObserVR_API.PostRequest(applicationID, events, debugMode));
        eventQueue.Clear();
#endif
    }
}
