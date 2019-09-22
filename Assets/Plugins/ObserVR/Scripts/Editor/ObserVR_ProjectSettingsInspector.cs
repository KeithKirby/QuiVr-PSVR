using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObserVR_ProjectSettings))]
public class ObserVR_ProjectSettingsInspector : Editor {
    public override void OnInspectorGUI() {
        GUI.enabled = false;
        DrawDefaultInspector();
        GUI.enabled = true;
    }
}
