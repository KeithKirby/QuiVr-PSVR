using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using ObserVR;

[RequireComponent(typeof(ObserVR_User)), RequireComponent(typeof(ObserVR_EventHandler)), DisallowMultipleComponent]
public class ObserVR_Session : MonoBehaviour {
    private static ObserVR_ProjectSettings _settings;
    public static ObserVR_Session Instance { get; private set; }
    public ObserVR_ProjectSettings Settings { get { return _settings; } }
    private static float maxSessionDurationInSeconds;

    [HideInInspector]
    public string sessionID;
    [HideInInspector]
    public bool isQuitting;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }

    void Start() {
        _settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");
        if (_settings == null || string.IsNullOrEmpty(_settings.applicationID)) {
            Debug.LogError("Couldn't find settings file or Application ID is missing...stopping analytics");
            ObserVR_Analytics.AnalyticsEnabled = false;
        }
        else {
            maxSessionDurationInSeconds = ConvertHoursToSeconds(_settings.maxSessionDuration);
        }
#if UNITY_2018
        Application.quitting += EndSession;
#endif
    }

    void OnEnable() {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;
    }

    private static void SceneLoaded(Scene arg0, LoadSceneMode arg1) {
        ObserVR_Events.CustomEvent("SCENE_LOADED").AddParameter("sceneName", arg0.name).AddParameter("sceneIndex", arg0.buildIndex).EndParameters();
    }

    private static void SceneUnloaded(Scene arg0) {
        ObserVR_Events.CustomEvent("SCENE_UNLOADED").AddParameter("sceneName", arg0.name).AddParameter("sceneIndex", arg0.buildIndex).EndParameters();
    }

    public void CreateNewSession() {
        sessionID = Guid.NewGuid().ToString();
        ObserVR_Events.CustomEvent("SESSION_START")
            .AddParameter("platform", Application.platform.ToString())
            .AddParameter("debugMode", _settings.debugMode ? "on" : "off")
            .AddParameter("ObserVR_version", _settings.version)
            .AddParameter("buildVersion", Application.version)
            .AddParameter("unityVersion", Application.unityVersion)
            .AddParameter("engine", "unity")
            .EndParameters();
    }

#if UNITY_ANDROID || UNITY_IOS
    void OnApplicationPause(bool pause) {
        if (pause) {
            ObserVR_Events.CustomEvent("SESSION_PAUSE").NoParameters();
            if (_settings.debugMode) {
                if (_settings.sendEventsOnDebugMode) {
                    ObserVR_EventHandler.Instance.SendEvents();
                }
            }
            else {
                ObserVR_EventHandler.Instance.SendEvents();
            }
        }
    }
#endif

#if UNITY_2018
    private void EndSession() {
        if (!isQuitting) {
            isQuitting = true;
            ObserVR_Events.CustomEvent("SESSION_END").NoParameters();
            if (_settings.debugMode) {
                if (_settings.sendEventsOnDebugMode) {
                    ObserVR_EventHandler.Instance.SendEvents();
                }
            }
            else {
                ObserVR_EventHandler.Instance.SendEvents();
            }
        }
    }
#elif UNITY_5_6_OR_NEWER
    void OnApplicationQuit() {
        if (!isQuitting) {
            Application.CancelQuit();
            isQuitting = true;
            ObserVR_Events.CustomEvent("SESSION_END").NoParameters();
            if (_settings.debugMode) {
                if (_settings.sendEventsOnDebugMode) {
                    ObserVR_EventHandler.Instance.SendEvents();
                }
                else {
                    Application.Quit();
                }
            }
            else {
                ObserVR_EventHandler.Instance.SendEvents();
            }
        }
    }
#endif

    private static float ConvertHoursToSeconds(int hours) {
        return hours * 60.0f * 60.0f;
    }

    private static void ForceShutoffAnalytics() {
#if UNITY_EDITOR
        Debug.LogWarning("Max session time reached...stopping ObserVR Analytics");
#endif
        ObserVR_Events.CustomEvent("SESSION_END").NoParameters();
        if (_settings.debugMode) {
            if (_settings.sendEventsOnDebugMode) {
                ObserVR_EventHandler.Instance.SendEvents();
            }
        }
        else {
            ObserVR_EventHandler.Instance.SendEvents();
        }
        ObserVR_Analytics.AnalyticsEnabled = false;
    }

    void Update() {
        if (Time.realtimeSinceStartup > maxSessionDurationInSeconds) {
            ForceShutoffAnalytics();
        }
    }
}
