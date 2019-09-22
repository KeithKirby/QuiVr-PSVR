using UnityEngine;
using ObserVR;
using System;
#if OBSERVR_VRTK
using VRTK;
#endif
#if OBSERVR_NVR
using NewtonVR;
#endif

[DisallowMultipleComponent]
public class ObserVR_TrackedObject : MonoBehaviour {
    [HideInInspector]
    public string nickname;
    [HideInInspector]
    public bool trackFOV = true;
    [HideInInspector]
    public float minimumFOVThreshold = 0.5f;
    [HideInInspector]
    public float leftThreshold = 0.4f;
    [HideInInspector]
    public float rightThreshold = 0.6f;
    [HideInInspector]
    public float downThreshold = 0.4f;
    [HideInInspector]
    public float upThreshold = 0.6f;
    [HideInInspector]
    public bool checkLineOfSight;
    [HideInInspector]
    public bool checkDistance;
    [HideInInspector]
    public float maxDistance = 10.0f;
    [HideInInspector]
    public bool FOVDebug;
    private bool inFOV;
    private bool debugMode;
    private string fovStartTime;
    private Vector3 grabStartPos;
    private Collider objCollider;
    private Renderer objRenderer;
    private static ObserVR_ProjectSettings settings;
    private static Camera cam;

#if OBSERVR_VRTK
    [HideInInspector]
    public bool trackTouch = true;
    [HideInInspector]
    public bool trackGrab_VRTK = true;
    [HideInInspector]
    public bool trackUse_VRTK = true;
    private VRTK_InteractableObject interactEvents_VRTK;
    private string touchTime;
    private string grabTime_VRTK;
    private string useTime_VRTK;
#endif
#if OBSERVR_NVR
    [HideInInspector]
    public bool trackGrab_NVR = true;
    [HideInInspector]
    public bool trackUse_NVR = true;
    private NVRInteractableItem interactEvents_NVR;
    private string grabTime_NVR;
    private string useTime_NVR;
#endif

    void Start() {
        if (ObserVR_Analytics.Instance == null || !ObserVR_Analytics.AnalyticsEnabled) {
            Debug.LogError("Could not find ObserVR_Analytics prefab. Make sure it is present and enabled in the scene");
            enabled = false;
        }

        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");
        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            enabled = false;
        }
        else {
            cam = Camera.main ?? FindObjectOfType<Camera>();
            objCollider = GetComponent<Collider>();
            objRenderer = GetComponentInChildren<Renderer>();
            debugMode = settings.debugMode;
        }
    }

    void OnEnable() {
#if OBSERVR_VRTK
        interactEvents_VRTK = GetComponentInChildren<VRTK_InteractableObject>();

        if (interactEvents_VRTK != null) {
            if (trackTouch) {
                interactEvents_VRTK.InteractableObjectTouched += StartTouch;
                interactEvents_VRTK.InteractableObjectUntouched += EndTouch;
            }
            if (trackGrab_VRTK) {
                interactEvents_VRTK.InteractableObjectGrabbed += StartGrab_VRTK;
                interactEvents_VRTK.InteractableObjectUngrabbed += EndGrab_VRTK;
            }
            if (trackUse_VRTK) {
                interactEvents_VRTK.InteractableObjectUsed += StartUse_VRTK;
                interactEvents_VRTK.InteractableObjectUnused += EndUse_VRTK;
            }
        }
#endif
#if OBSERVR_NVR
        interactEvents_NVR = GetComponentInChildren<NVRInteractableItem>();

        if (interactEvents_NVR != null) {
            if (trackGrab_NVR) {
                interactEvents_NVR.OnBeginInteraction.AddListener(StartGrab_NVR);
                interactEvents_NVR.OnEndInteraction.AddListener(EndGrab_NVR);
            }
            if (trackUse_NVR) {
                interactEvents_NVR.OnUseButtonDown.AddListener(StartUse_NVR);
                interactEvents_NVR.OnUseButtonUp.AddListener(EndUse_NVR);
            }
        }
#endif
    }

    void OnDisable() {
#if OBSERVR_VRTK
        if (interactEvents_VRTK != null) {
            if (trackTouch) {
                interactEvents_VRTK.InteractableObjectTouched -= StartTouch;
                interactEvents_VRTK.InteractableObjectUntouched -= EndTouch;
            }
            if (trackGrab_VRTK) {
                interactEvents_VRTK.InteractableObjectGrabbed -= StartGrab_VRTK;
                interactEvents_VRTK.InteractableObjectUngrabbed -= EndGrab_VRTK;
            }
            if (trackUse_VRTK) {
                interactEvents_VRTK.InteractableObjectUsed -= StartUse_VRTK;
                interactEvents_VRTK.InteractableObjectUnused -= EndUse_VRTK;
            }
        }
#endif
#if OBSERVR_NVR
        if (interactEvents_NVR != null) {
            if (trackGrab_NVR) {
                interactEvents_NVR.OnBeginInteraction.RemoveAllListeners();
                interactEvents_NVR.OnEndInteraction.RemoveAllListeners();
            }
            if (trackUse_NVR) {
                interactEvents_NVR.OnUseButtonDown.RemoveAllListeners();
                interactEvents_NVR.OnUseButtonUp.RemoveAllListeners();
            }
        }
#endif
    }

    #region Field of View

    void Update() {
        if (trackFOV) {
            if (cam == null) {
                cam = Camera.main ?? FindObjectOfType<Camera>();
            }
            else {
                if (gameObject.activeSelf && objRenderer && objRenderer.isVisible) {
                    Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
                    if (viewPos.x >= leftThreshold && viewPos.x <= rightThreshold && viewPos.y >= downThreshold &&
                        viewPos.y <= upThreshold && viewPos.z > 0) {
                        if (checkLineOfSight) {
                            if (checkDistance) {
                                if (!inFOV) {
                                    if (Vector3.SqrMagnitude(transform.position - cam.transform.position) <=
                                        maxDistance * maxDistance) {
                                        RaycastHit hit = new RaycastHit();
                                        if (Physics.Linecast(cam.transform.position, transform.position, out hit)) {
                                            if (hit.collider == objCollider) {
#if UNITY_EDITOR
                                                if (debugMode && FOVDebug) {
                                                    Debug.Log("Looking at " +
                                                              (string.IsNullOrEmpty(nickname) ? name : nickname));
                                                    foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
                                                        rend.material.color = Color.red;
                                                    }
                                                }
#endif
                                                fovStartTime = ObserVR_Functions.GetCurrentTimeString();
                                                inFOV = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else {
                                if (!inFOV) {
                                    RaycastHit hit = new RaycastHit();
                                    if (Physics.Linecast(cam.transform.position, transform.position, out hit)) {
                                        if (hit.collider == objCollider) {
#if UNITY_EDITOR
                                            if (debugMode && FOVDebug) {
                                                Debug.Log(
                                                    "Looking at " + (string.IsNullOrEmpty(nickname) ? name : nickname));
                                                foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
                                                    rend.material.color = Color.red;
                                                }
                                            }
#endif
                                            fovStartTime = ObserVR_Functions.GetCurrentTimeString();
                                            inFOV = true;
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            if (checkDistance) {
                                if (!inFOV) {
                                    if (Vector3.SqrMagnitude(transform.position - cam.transform.position) <=
                                        maxDistance * maxDistance) {
#if UNITY_EDITOR
                                        if (debugMode && FOVDebug) {
                                            Debug.Log("Looking at " + (string.IsNullOrEmpty(nickname) ? name : nickname));
                                            foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
                                                rend.material.color = Color.red;
                                            }
                                        }
#endif
                                        fovStartTime = ObserVR_Functions.GetCurrentTimeString();
                                        inFOV = true;
                                    }
                                }
                            }
                            else {
                                if (!inFOV) {
#if UNITY_EDITOR
                                    if (debugMode && FOVDebug) {
                                        Debug.Log("Looking at " + (string.IsNullOrEmpty(nickname) ? name : nickname));
                                        foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
                                            rend.material.color = Color.red;
                                        }
                                    }
#endif
                                    fovStartTime = ObserVR_Functions.GetCurrentTimeString();
                                    inFOV = true;
                                }
                            }
                        }
                    }
                    else {
#if UNITY_EDITOR
                        if (debugMode && FOVDebug) {
                            foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
                                rend.material.color = Color.white;
                            }
                        }
#endif
                        if (inFOV) {
                            if ((Convert.ToDateTime(ObserVR_Functions.GetCurrentTimeString()) -
                                 Convert.ToDateTime(fovStartTime)).TotalSeconds >= minimumFOVThreshold) {
                                ObserVR_Events.CustomEvent("OBJECT_FOV").AddParameter("assetName",
                                        string.IsNullOrEmpty(nickname) ? name : nickname)
                                    .AddParameter("startTime", fovStartTime)
                                    .EndParameters();
                            }
                            inFOV = false;
                        }
                    }
                }
            }
        }
    }

    #endregion


    #region Interactions

#if OBSERVR_VRTK
    private void StartTouch(object sender, InteractableObjectEventArgs e) {
        touchTime = ObserVR_Functions.GetCurrentTimeString();
    }

    private void EndTouch(object sender, InteractableObjectEventArgs e) {
        ObserVR_Events.CustomEvent("TOUCH")
            .AddParameter("assetName", string.IsNullOrEmpty(nickname) ? name : nickname)
            .AddParameter("startTime", touchTime)
            .AddParameter("position", transform.position)
            .AddParameter("hand", e.interactingObject.transform.name)
            .EndParameters();
    }

    private void StartGrab_VRTK(object sender, InteractableObjectEventArgs e) {
        grabTime_VRTK = ObserVR_Functions.GetCurrentTimeString();
        grabStartPos = transform.position;
    }

    private void EndGrab_VRTK(object sender, InteractableObjectEventArgs e) {
        ObserVR_Events.CustomEvent("GRAB")
            .AddParameter("assetName", string.IsNullOrEmpty(nickname) ? name : nickname)
            .AddParameter("startTime", grabTime_VRTK)
            .AddParameter("startPosition", grabStartPos)
            .AddParameter("endPosition", transform.position)
            .AddParameter("hand", e.interactingObject.transform.name)
            .EndParameters();
    }

    private void StartUse_VRTK(object sender, InteractableObjectEventArgs e) {
        useTime_VRTK = ObserVR_Functions.GetCurrentTimeString();
    }

    private void EndUse_VRTK(object sender, InteractableObjectEventArgs e) {
        ObserVR_Events.CustomEvent("USE")
            .AddParameter("assetName", string.IsNullOrEmpty(nickname) ? name : nickname)
            .AddParameter("startTime", useTime_VRTK)
            .AddParameter("position", transform.position)
            .AddParameter("hand", e.interactingObject.transform.name)
            .EndParameters();
    }
#endif

#if OBSERVR_NVR
    private void StartGrab_NVR() {
        grabTime_NVR = ObserVR_Functions.GetCurrentTimeString();
    }

    private void EndGrab_NVR() {
        ObserVR_Events.CustomEvent("GRAB")
            .AddParameter("assetName", string.IsNullOrEmpty(nickname) ? name : nickname)
            .AddParameter("startTime", grabTime_NVR)
            .AddParameter("position", transform.position)
            .EndParameters();
    }

    private void StartUse_NVR() {
        useTime_NVR = ObserVR_Functions.GetCurrentTimeString();
    }

    private void EndUse_NVR() {
        ObserVR_Events.CustomEvent("USE")
            .AddParameter("assetName", string.IsNullOrEmpty(nickname) ? name : nickname)
            .AddParameter("startTime", useTime_NVR)
            .AddParameter("position", transform.position)
            .EndParameters();
    }
#endif

    #endregion
}