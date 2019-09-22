using UnityEngine;
using ObserVR;

[DisallowMultipleComponent]
public class ObserVR_GeneralFOV : MonoBehaviour {
    private static ObserVR_ProjectSettings settings;
    private static float generalFOVMaxDistance;
    private static float generalFOVInterval;
    private static float _timeCounterFOV;
    private static Camera cam;

    void Start() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");

        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            Debug.LogError("Couldn't find settings file or Application ID is missing...stopping analytics");
            ObserVR_Analytics.AnalyticsEnabled = false;
        }
        else {
            cam = Camera.main ?? FindObjectOfType<Camera>();
            generalFOVInterval = settings.generalFOVInterval;
            generalFOVMaxDistance = settings.generalFOVMaxDistance;
        }
    }

    void Update() {
        if (cam == null) {
            cam = Camera.main ?? FindObjectOfType<Camera>();
        }
        else {
            if (_timeCounterFOV < generalFOVInterval) {
                _timeCounterFOV += Time.unscaledDeltaTime;
            }
            else {
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, generalFOVMaxDistance)) {
#if UNITY_EDITOR
                    if (settings.debugMode) {
                        Debug.DrawLine(cam.transform.position, hit.point, Color.red);
                    }
#endif
                    if (!string.IsNullOrEmpty(ObserVR_Session.Instance.sessionID)) {
                        ObserVR_Events.CustomEvent("GENERAL_FOV")
                        .AddParameter("hitObject", hit.transform.name)
                        .AddParameter("hitPoint_x", hit.point.x)
                        .AddParameter("hitPoint_y", hit.point.y)
                        .AddParameter("hitPoint_z", hit.point.z)
                        .AddParameter("hitNormal_x", hit.normal.x)
                        .AddParameter("hitNormal_y", hit.normal.y)
                        .AddParameter("hitNormal_z", hit.normal.z)
                        .EndParameters();
                    }
                }
                _timeCounterFOV = 0.0f;
            }
        }
    }
}
