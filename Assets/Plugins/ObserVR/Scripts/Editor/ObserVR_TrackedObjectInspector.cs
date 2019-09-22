using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
#if OBSERVR_VRTK
using VRTK;
#endif
#if OBSERVR_NVR
using NewtonVR;
#endif

[CustomEditor(typeof(ObserVR_TrackedObject))]
public class ObserVR_TrackedObjectInspector : Editor {
    private bool VRTK_Present;
    private bool NVR_Present;
    private ObserVR_TrackedObject script;

    private void OnEnable() {
        script = (ObserVR_TrackedObject)target;
#if OBSERVR_VRTK
        VRTK_Present = script.GetComponentInChildren<VRTK_InteractableObject>();
        script.trackTouch = script.trackGrab_VRTK = script.trackUse_VRTK = VRTK_Present;
#endif
#if OBSERVR_NVR
        NVR_Present = script.GetComponentInChildren<NVRInteractableItem>();
        script.trackGrab_NVR = NVR_Present;
#endif
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        GUILayout.Space(15);
        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
        script.nickname = EditorGUILayout.TextField(new GUIContent("Nickname", "[Optional] Gives the object a more descriptive name in our database. If blank, it uses the name of the GameObject this script is attached to"), script.nickname);
        GUILayout.Space(15);
        EditorGUILayout.LabelField("Field of View Settings", EditorStyles.boldLabel);
        script.trackFOV = EditorGUILayout.Toggle(new GUIContent("Track FOV", "Track when this object is in the user's FOV"), script.trackFOV);
        if (script.trackFOV) {
            script.minimumFOVThreshold = EditorGUILayout.Slider(new GUIContent("Min. FOV Threshold", "The minimum time an object has to be in the user's FOV to register an event (in seconds) [Default: 0.5]"), script.minimumFOVThreshold, 0.5f, 5.0f);
            script.leftThreshold = EditorGUILayout.Slider(new GUIContent("Left Threshold", "Left Threshold [Default: 0.4]"), script.leftThreshold, 0.1f, 0.5f);
            script.rightThreshold = EditorGUILayout.Slider(new GUIContent("Right Threshold", "Right Threshold [Default: 0.6]"), script.rightThreshold, 0.5f, 0.9f);
            script.downThreshold = EditorGUILayout.Slider(new GUIContent("Down Threshold", "Down Threshold [Default: 0.4]"), script.downThreshold, 0.1f, 0.5f);
            script.upThreshold = EditorGUILayout.Slider(new GUIContent("Up Threshold", "Up Threshold [Default: 0.6]"), script.upThreshold, 0.5f, 0.9f);
            script.checkLineOfSight = EditorGUILayout.Toggle(new GUIContent("Check Line of Sight", "Checks the user's line of sight to see if there are other objects obstructing the view"), script.checkLineOfSight);
            script.checkDistance = EditorGUILayout.Toggle(new GUIContent("Check Distance", "Checks the distance between the user and the object, won't track if too far away"), script.checkDistance);
            if (script.checkDistance) {
                script.maxDistance = EditorGUILayout.FloatField(new GUIContent("Distance Threshold", "The maximum distance that the object will register as being in the user's FOV [Default: 10]"), script.maxDistance);
            }
            script.FOVDebug = EditorGUILayout.Toggle(new GUIContent("FOV Debug", "Turns objects red when they are in your FOV and white when not in FOV. For tesing/debugging purposes only. (Debug Mode needs to be enabled)"), script.FOVDebug);
        }
#if OBSERVR_VRTK
        GUILayout.Space(15);
        EditorGUI.BeginDisabledGroup(VRTK_Present == false);
        EditorGUILayout.LabelField("VRTK Settings", EditorStyles.boldLabel);
        script.trackTouch = EditorGUILayout.Toggle(new GUIContent("Track Touch", "Track when this object is touched (only available if this gameobject contains a VRTK_InteractableObject script)"), script.trackTouch);
        script.trackGrab_VRTK = EditorGUILayout.Toggle(new GUIContent("Track Grab", "Track when this object is grabbed (only available if this gameobject contains a VRTK_InteractableObject script)"), script.trackGrab_VRTK);
        script.trackUse_VRTK = EditorGUILayout.Toggle(new GUIContent("Track Use", "Track when this object is used (only available if this gameobject contains a VRTK_InteractableObject script)"), script.trackUse_VRTK);
        if (!script.GetComponentInChildren<VRTK_InteractableObject>()) {
            script.trackTouch = script.trackGrab_VRTK = script.trackUse_VRTK = false;
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(15);
#endif
#if OBSERVR_NVR
        GUILayout.Space(15);
        EditorGUI.BeginDisabledGroup(NVR_Present == false);
        EditorGUILayout.LabelField("NewtonVR Settings", EditorStyles.boldLabel);
        script.trackGrab_NVR = EditorGUILayout.Toggle(new GUIContent("Track Grab", "Track when this object is grabbed (only available if this gameobject contains a NVRInteractableItem script)"), script.trackGrab_NVR);
        script.trackUse_NVR = EditorGUILayout.Toggle(new GUIContent("Track Use", "Track when this object is used (only available if this gameobject contains a NVRInteractableItem script)"), script.trackUse_NVR);
        if (!script.GetComponentInChildren<NVRInteractableItem>()) {
            script.trackGrab_NVR = script.trackUse_NVR = false;
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(15);
#endif
        if (GUI.changed) {
            EditorUtility.SetDirty(script);
            EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
        }
    }
}

