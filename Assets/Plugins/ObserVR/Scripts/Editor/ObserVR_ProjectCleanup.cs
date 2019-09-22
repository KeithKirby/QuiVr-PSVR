#if UNITY_5_6_OR_NEWER
using UnityEditor.Build;
#endif
#if UNITY_2018
using UnityEditor.Build.Reporting;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;

#if UNITY_2018 && UNITY_EDITOR
public class ObserVR_ProjectCleanup : Editor, IPreprocessBuildWithReport {
#elif UNITY_5_6_OR_NEWER && UNITY_EDITOR
public class ObserVR_ProjectCleanup : Editor, IPreprocessBuild {
#elif UNITY_EDITOR
public class ObserVR_ProjectCleanup : Editor {
#endif
    private static ObserVR_ProjectSettings settings;

#if (UNITY_2018 || UNITY_5_6_OR_NEWER) && UNITY_EDITOR
    public int callbackOrder { get { return 0; } }
#endif

    private static void CheckSettings() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");
        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            return;
        }
        if (settings.environment == ObserVR_ProjectSettings.Environment.Dev) {
            if (EditorUtility.DisplayDialog("ObserVR Analytics", "It appears that you're still in Dev. Do you want to switch to Prod for this build?", "Switch to Prod", "Stay on Dev")) {
                settings.environment = ObserVR_ProjectSettings.Environment.Prod;
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }
        if (settings.debugMode) {
            if (EditorUtility.DisplayDialog("ObserVR Analytics", "It appears that Debug Mode is still enabled. Do you want to turn it off for this build?", "Turn Off", "Leave On")) {
                settings.debugMode = false;
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }
    }

    private static void SwitchBackToDev() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");
        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            return;
        }
        if (settings.environment == ObserVR_ProjectSettings.Environment.Prod && !string.IsNullOrEmpty(settings.applicationID_Dev)) {
            settings.environment = ObserVR_ProjectSettings.Environment.Dev;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            Debug.Log("[ObserVR]: Switched back to Dev post-build...");
        }
    }

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        SwitchBackToDev();
    }

#if (UNITY_2018 || UNITY_5_6_OR_NEWER) && UNITY_EDITOR
#if UNITY_2018
    public void OnPreprocessBuild(BuildReport report) {
#elif UNITY_5_6_OR_NEWER
    public void OnPreprocessBuild(BuildTarget target, string path) {
#endif
        if (EditorBuildSettings.scenes.Length > 0) {
            string openScene = SceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
            ObserVR_Analytics[] prefab = Resources.FindObjectsOfTypeAll<ObserVR_Analytics>();
            if (prefab.Length != 0 && !prefab[0].gameObject.activeSelf) {
                if (EditorUtility.DisplayDialog("Wait!", "The ObserVR Analytics prefab was found in the first scene but it appears to be inactive. Do you want to set it active and include ObserVR Analytics in this build?", "Yes", "No")) {
                    prefab[0].gameObject.SetActive(true);
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                    CheckSettings();
                }
                EditorSceneManager.OpenScene(openScene);
                return;
            }
            if (!FindObjectOfType<ObserVR_Analytics>()) {
                if (EditorUtility.DisplayDialog("Wait!", "Couldn't find ObserVR Analytics prefab in the first scene. Do you want to insert it and include ObserVR Analytics in this build?", "Yes", "No")) {
                    GameObject g = (GameObject)Instantiate(Resources.Load("ObserVR_Analytics"));
                    g.name = "ObserVR_Analytics";
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                    for (int i = 1; i < EditorBuildSettings.scenes.Length; i++) {
                        EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path);
                        if (FindObjectOfType<ObserVR_Analytics>()) {
                            DestroyImmediate(FindObjectOfType<ObserVR_Analytics>().gameObject);
                        }
                        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                    }
                    CheckSettings();
                }
                else {
                    for (int i = 1; i < EditorBuildSettings.scenes.Length; i++) {
                        EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path);
                        if (FindObjectOfType<ObserVR_Analytics>()) {
                            DestroyImmediate(FindObjectOfType<ObserVR_Analytics>().gameObject);
                        }
                        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                    }
                }
                EditorSceneManager.OpenScene(openScene);
                return;
            }
            CheckSettings();
            EditorSceneManager.OpenScene(openScene);
        }
    }
#endif

    [MenuItem("Window/ObserVR/Run Pre-Build Cleanup", false, 2)]
    static void BuildPrep() {
        if (EditorBuildSettings.scenes.Length > 0) {
            string openScene = SceneManager.GetActiveScene().path;
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
                EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path);
                if (i == 0) {
                    if (!FindObjectOfType<ObserVR_Analytics>()) {
                        GameObject g = (GameObject)Instantiate(Resources.Load("ObserVR_Analytics"));
                        g.name = "ObserVR_Analytics";
                    }
                }
                else {
                    if (FindObjectOfType<ObserVR_Analytics>()) {
                        DestroyImmediate(FindObjectOfType<ObserVR_Analytics>().gameObject);
                    }
                }
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }
            CheckSettings();
            EditorSceneManager.OpenScene(openScene);
            EditorUtility.DisplayDialog("Done!", "Pre-Build Cleanup Successful.", "Close");
        }
        else {
            EditorUtility.DisplayDialog("Oops!", "It appears you have not yet added any scenes to the build in Build Settings.", "Close");
        }
    }
}
