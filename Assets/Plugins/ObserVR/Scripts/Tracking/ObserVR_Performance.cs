using UnityEngine;
using ObserVR;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class ObserVR_Performance : MonoBehaviour {
    private static ObserVR_ProjectSettings settings;
    private static float _frameCounter = 0.0f;
    private static float _timeCounter = 0.0f;
    private static float _lastFramerate;
    private static float minFramerateThreshold;
    private const float Interval = 5.0f;

    void Start() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");

        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            Debug.LogError("Couldn't find settings file or Application ID is missing...stopping analytics");
            ObserVR_Analytics.AnalyticsEnabled = false;
        }
        else {
            minFramerateThreshold = settings.minFramerateThreshold;
        }
    }

    private static void GetRenderedObjectsInScene(float framerate) {
        int totalRenderCount = 0;
        Renderer[] rendArray = FindObjectsOfType<Renderer>();
        List<string> renderedObjects = new List<string>();
        if (rendArray != null) {
            for (int i = 0; i < rendArray.Length; i++) {
                if (rendArray[i].isVisible) {
                    if (totalRenderCount < 50) {
                        renderedObjects.Add(rendArray[i].gameObject.name);
                    }
                    totalRenderCount++;
                }
            }
        }
        ObserVR_Events.CustomEvent("FPS")
            .AddParameter("fps", framerate)
            .AddParameter("minThreshold", minFramerateThreshold)
            .AddParameter("objectCount", totalRenderCount)
            .AddParameter("objectList", renderedObjects.ToArray())
            .EndParameters();
    }

    void Update() {
        if (_timeCounter < Interval) {
            _timeCounter += Time.unscaledDeltaTime;
            _frameCounter++;
        }
        else {
            _lastFramerate = _frameCounter / _timeCounter;
            if (_lastFramerate >= minFramerateThreshold) {
                GetRenderedObjectsInScene(_lastFramerate);
            }
            _frameCounter = 0.0f;
            _timeCounter -= Interval;
        }
    }
}
