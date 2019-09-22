using UnityEngine;
using UnityEditor;

public class ObserVR_Credits : EditorWindow {
    private static ObserVR_ProjectSettings settings;
    public static readonly Vector2 windowSize = new Vector2(350, 450);

    [MenuItem("Window/ObserVR/About", false, 52)]
    private static void ShowCredits() {
        ObserVR_Credits window = GetWindow<ObserVR_Credits>(true, "About ObserVR");
        window.minSize = windowSize;
        window.Show();
    }

    void Awake() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");
    }

    public void OnGUI() {
        var logo = Resources.Load<Texture2D>("ObserVR_Logo");
        var rect = GUILayoutUtility.GetRect(position.width - 8, 150, GUI.skin.box);
        if (logo) {
            GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);
        }

        GUILayout.Space(20);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("ObserVR is brought to you by:\n");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Lucas Toohey");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Jacob Copus");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Justin Cellona");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(20);
        if (GUILayout.Button("ObserVR, Inc.")) {
            Application.OpenURL("https://www.observr.tech");
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Go to dashboard")) {
            Application.OpenURL("http://dashboard.observr.tech");
        }

        GUILayout.Space(30);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Plugin version: " + settings.version);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
}
