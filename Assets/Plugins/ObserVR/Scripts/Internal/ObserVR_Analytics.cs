using UnityEngine;

[DisallowMultipleComponent]
public class ObserVR_Analytics : MonoBehaviour {
    public static ObserVR_Analytics Instance { get; private set; }

    public static bool AnalyticsEnabled {
        get { return Instance.gameObject.activeSelf; }
        set { Instance.gameObject.SetActive(value); }
    }

    void Awake() {
        if (Instance)
            DestroyImmediate(gameObject);
        else {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    void OnEnable() {
        AnalyticsEnabled = true;
    }

    void OnDisable() {
        AnalyticsEnabled = false;
    }
}

