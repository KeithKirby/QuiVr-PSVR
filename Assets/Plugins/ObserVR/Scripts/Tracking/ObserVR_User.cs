using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using ObserVR;
using System;

[RequireComponent(typeof(ObserVR_Session)), RequireComponent(typeof(ObserVR_EventHandler)), DisallowMultipleComponent]
public class ObserVR_User : MonoBehaviour {
    public static ObserVR_User Instance { get; private set; }
    private static ObserVR_ProjectSettings settings;
    private static ObserVR_UserInfo userInfo;

    [Serializable]
    public class ObserVR_UserInfo {
        public string city;
        public string country_code;
        public string country_name;
        public string continent_code;
        public float latitude;
        public float longitude;
        public string zip;
        public string region_name;

        public ObserVR_UserInfo() {
            city = country_code = country_name = continent_code = zip = region_name = "Unknown";
            latitude = longitude = 0;
        }
    }

    [HideInInspector]
    public string userID;

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
            if (settings.environment == ObserVR_ProjectSettings.Environment.Dev) {
                StartCoroutine(PlayerPrefs.HasKey("ObserVR_userID_Dev") ? UserExists() : CreateUser());
            }
            else {
                StartCoroutine(PlayerPrefs.HasKey("ObserVR_userID") ? UserExists() : CreateUser());
            }
        }
    }

    private IEnumerator UserExists() {
        if (ObserVR_Session.Instance.Settings == null) {
            yield return new WaitUntil(() => ObserVR_Session.Instance.Settings != null);
        }
        if (settings.environment == ObserVR_ProjectSettings.Environment.Dev) {
            userID = PlayerPrefs.GetString("ObserVR_userID_Dev");
        }
        else {
            userID = PlayerPrefs.GetString("ObserVR_userID");
        }
        ObserVR_Session.Instance.CreateNewSession();
    }

    private IEnumerator CreateUser() {
        if (ObserVR_Session.Instance.Settings == null) {
            yield return new WaitUntil(() => ObserVR_Session.Instance.Settings != null);
        }
        userID = Guid.NewGuid().ToString();
        ObserVR_Session.Instance.CreateNewSession();
        var headset = "Unknown";
        userInfo = new ObserVR_UserInfo();
        using (UnityWebRequest getUserInfo = UnityWebRequest.Get(ObserVR_ProjectSettings.userLookup)) {
#if UNITY_2017_2_OR_NEWER
            yield return getUserInfo.SendWebRequest();
#else
            yield return getUserInfo.Send();
#endif
#if UNITY_2017_1_OR_NEWER
            if (getUserInfo.isNetworkError || getUserInfo.isHttpError) {
#else
            if (getUserInfo.isError) {
#endif
                //Should try again
                Debug.Log("Couldn't get user info.");
            }
            else {
                if (getUserInfo.responseCode == 200) {
                    userInfo = JsonUtility.FromJson<ObserVR_UserInfo>(getUserInfo.downloadHandler.text);
                }
                else {
                    Debug.Log("Couldn't get user info, bad response code.");
                }
            }
        }
#if OBSERVR_SVR && OBSERVR_OVR
        DateTime startPoint = DateTime.UtcNow;
        yield return new WaitUntil(() => SteamVR.instance != null || OVRPlugin.initialized || (DateTime.UtcNow.Subtract(startPoint).TotalSeconds >= 5));
        if (SteamVR.instance != null) {
            headset = SteamVR.instance.hmd_ModelNumber;
        }
        else if (OVRPlugin.hmdPresent) {
            headset = OVRPlugin.GetSystemHeadsetType().ToString();
        }
        else {
#if UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.XRDevice.isPresent) {
                headset = UnityEngine.XR.XRDevice.model;
            }
#else
            if (UnityEngine.VR.VRDevice.isPresent) {
                headset = UnityEngine.VR.VRDevice.model;
            }
#endif
        }
#elif OBSERVR_SVR
        DateTime startPoint = DateTime.UtcNow;
        yield return new WaitUntil(() => SteamVR.instance != null || (DateTime.UtcNow.Subtract(startPoint).TotalSeconds >= 5));
        if (SteamVR.instance != null) {
            headset = SteamVR.instance.hmd_ModelNumber;
        }
        else {
#if UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.XRDevice.isPresent) {
                headset = UnityEngine.XR.XRDevice.model;
            }
#else
            if (UnityEngine.VR.VRDevice.isPresent) {
                headset = UnityEngine.VR.VRDevice.model;
            }
#endif
        }
#elif OBSERVR_OVR
        DateTime startPoint = DateTime.UtcNow;
        yield return new WaitUntil(() => OVRPlugin.initialized || (DateTime.UtcNow.Subtract(startPoint).TotalSeconds >= 5));
        if (OVRPlugin.hmdPresent) {
            headset = OVRPlugin.GetSystemHeadsetType().ToString();
        }
        else {
#if UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.XRDevice.isPresent) {
                headset = UnityEngine.XR.XRDevice.model;
            }
#else
            if (UnityEngine.VR.VRDevice.isPresent) {
                headset = UnityEngine.VR.VRDevice.model;
            }
#endif
        }
#else
#if UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.XRDevice.isPresent) {
                headset = UnityEngine.XR.XRDevice.model;
            }
#else
            if (UnityEngine.VR.VRDevice.isPresent) {
                headset = UnityEngine.VR.VRDevice.model;
            }
#endif
#endif
#if !OBSERVR_OVR && (UNITY_ANDROID || UNITY_IOS)
            headset = SystemInfo.deviceModel;
#elif UNITY_WEBGL
            headset = "web";
#endif
        SendUserCreateEvent(headset);
        if (!settings.debugMode || (settings.debugMode && settings.sendEventsOnDebugMode)) {
            if (settings.environment == ObserVR_ProjectSettings.Environment.Dev) {
                PlayerPrefs.SetString("ObserVR_userID_Dev", userID);
            }
            else {
                PlayerPrefs.SetString("ObserVR_userID", userID);
            }
        }
    }

    private static void SendUserCreateEvent(string headset) {
        ObserVR_Events.CustomEvent("USER_CREATE")
            .AddParameter("gpu", SystemInfo.graphicsDeviceVendor + " " + SystemInfo.graphicsDeviceName)
            .AddParameter("cpu", SystemInfo.processorType)
            .AddParameter("ram", SystemInfo.systemMemorySize)
            .AddParameter("os", SystemInfo.operatingSystem)
            .AddParameter("headset", headset)
            .AddParameter("city_name", userInfo.city)
            .AddParameter("country_code", userInfo.country_code)
            .AddParameter("country_name", userInfo.country_name)
            .AddParameter("continent_code", userInfo.continent_code)
            .AddParameter("latitude", userInfo.latitude)
            .AddParameter("longitude", userInfo.longitude)
            .AddParameter("postal_code", userInfo.zip)
            .AddParameter("region_name", userInfo.region_name)
            .EndParameters();
    }
}