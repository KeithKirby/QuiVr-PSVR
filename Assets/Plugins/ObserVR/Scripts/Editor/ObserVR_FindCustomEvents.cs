using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObserVR_FindCustomEvents : EditorWindow {
#if UNITY_EDITOR
    private static List<KeyValuePair<string, string>> customEvents;
    private static GUIStyle titleHeader;
    private static GUIStyle leftHeader;
    private static GUIStyle rightHeader;
    private static GUIStyle leftKey;
    private static GUIStyle rightVal;

    [MenuItem("Window/ObserVR/Custom Event List %#e", false, 1)]
    private static void FindStuff() {
        customEvents = new List<KeyValuePair<string, string>>();
        ProcessDirectory(Application.dataPath);
        GetWindow<ObserVR_FindCustomEvents>(true, "Custom Events").minSize = new Vector2(700, 50);
    }

    private void OnGUI() {
        titleHeader = new GUIStyle(GUI.skin.label) {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        leftHeader = new GUIStyle(GUI.skin.label) {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold
        };
        rightHeader = new GUIStyle(GUI.skin.label) {
            alignment = TextAnchor.MiddleRight,
            fontStyle = FontStyle.Bold
        };
        leftKey = new GUIStyle(GUI.skin.label) {
            alignment = TextAnchor.MiddleLeft
        };
        rightVal = new GUIStyle(GUI.skin.label) {
            alignment = TextAnchor.MiddleRight
        };
        CreateTable(customEvents);
    }

    private static void CreateTable(List<KeyValuePair<string, string>> events) {
        EditorGUILayout.LabelField(string.Format("Custom Event List ({0})", events.Count), titleHeader);
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Event", leftHeader);
        EditorGUILayout.LabelField("File", rightHeader);
        GUILayout.EndHorizontal();
        for (var i = 0; i < events.Count; i++) {
            GUILayout.BeginHorizontal();
            GUILayout.Label((i + 1) + ". " + events[i].Key.Substring(0, events[i].Key.IndexOf(')') + 1), leftKey);
            GUILayout.Label(events[i].Value, rightVal);
            GUILayout.EndHorizontal();
        }
    }

    public static void ProcessDirectory(string targetDirectory) {
        string[] fileEntries = Directory.GetFiles(targetDirectory, "*.cs");
        foreach (string fileName in fileEntries.Where(fileName => fileName.Substring(fileName.LastIndexOf('\\') + 1) != "ObserVR_FindCustomEvents.cs" && !fileName.Substring(fileName.LastIndexOf('\\') + 1).Contains("ObserVR"))) {
            FindEvents(fileName);
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries) {
            ProcessDirectory(subdirectory);
        }
    }

    private static void FindEvents(string path) {
        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines.Where(line => line.Contains("ObserVR_Events.CustomEvent"))) {
            customEvents.Add(new KeyValuePair<string, string>(line.Trim(), path.Substring(path.LastIndexOf('\\') + 1)));
        }
    }
#endif
}
