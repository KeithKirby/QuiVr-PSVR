using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObserVR_Analytics))]
public class ObserVR_GoToSettingsInspector : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUILayout.Space(10);
        if (GUILayout.Button("Open Settings")) {
            ObserVR_ProjectSetup window = (ObserVR_ProjectSetup)EditorWindow.GetWindow(typeof(ObserVR_ProjectSetup));
            window.minSize = ObserVR_ProjectSetup.windowSize;
            window.titleContent = new GUIContent() {
                text = "Settings"
            };
            window.Show();
        }
    }
}

