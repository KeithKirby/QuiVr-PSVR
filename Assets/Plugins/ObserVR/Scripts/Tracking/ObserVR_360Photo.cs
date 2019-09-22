using UnityEngine;
using System;
using ObserVR;

public class ObserVR_360Photo : MonoBehaviour {
    private static ObserVR_ProjectSettings settings;
    private static Camera cam;
    [HideInInspector]
    public float sphereRadius;

    void Start() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");
        cam = Camera.main ?? FindObjectOfType<Camera>();

        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            ObserVR_Analytics.AnalyticsEnabled = false;
            enabled = false;
        }
        else {
            InvokeRepeating("SendFOVPointPhoto", 0.1f, settings.generalFOVInterval);
        }
    }

    private void SendFOVPointPhoto() {
        RaycastHit hit;
        Ray cursorRay = new Ray(transform.position, -cam.transform.forward);
        if (Physics.Raycast(cursorRay, out hit, 200)) {
            double lat = Math.Round(90 - (float)Math.Acos(hit.point.y / sphereRadius) * 180 / Math.PI);
            double lon = Math.Round(((180 + (float)Math.Atan2(hit.point.x, hit.point.z) * 180 / Math.PI) % 360) - 180);
#if UNITY_EDITOR
            if (settings.debugMode) {
                Debug.DrawLine(hit.point, cam.transform.position, Color.red);
            }
#endif
            ObserVR_Events.CustomEvent("GENERAL_FOV").AddParameter("hitLat", lat).AddParameter("hitLon", lon).EndParameters();
        }
    }
}
