using UnityEngine;
using UnityEditor;
using System.IO;

public class SetPSVROptions
{
    // Set all of the Player Settings at once, rather than individually
    [MenuItem("PlayStation VR/Change All Settings")]
    static void SetAllPlayerSettings()
    {
        SetOptions();
        ReplaceInputManager();
        ReplaceTagManager();
        SetScenesForBuild();
    }

    // Set a VR resolution and enable VR and Social Screen support
    [MenuItem("PlayStation VR/Individual Settings/Set VR Player Settings")]
	static void SetOptions()
	{
		PlayerSettings.PS4.videoOutReprojectionRate = 120;
		PlayerSettings.PS4.videoOutInitialWidth = 1920;

        // Enable VR (cross-platform setting)
        PlayerSettings.virtualRealitySupported = true;

        // Enable Social Screen Support (shows a different, non-VR view on the monitor)
        PlayerSettings.PS4.socialScreenEnabled = 1;

        // Post-reprojection support currently only works in Forward
        PlayerSettings.renderingPath = RenderingPath.Forward;

        Debug.Log("PlayerSettings configured!");
	}

    // Replace whatever Input Manager you currently have with one to work with the VR Project
    [MenuItem("PlayStation VR/Individual Settings/Set VR Input Manager")]
    static void ReplaceInputManager()
    {
        // This is the InputManager asset that comes with the example project. Note that to avoid an import error, the '.asset' file extension has been removed
        string sourceFile = Path.Combine(Application.dataPath, "PlayStation VR Example/Editor/InputManager");

        // This is the InputManager in your ProjectSettings folder
        string targetFile = Application.dataPath;
        targetFile = targetFile.Replace("/Assets", "/ProjectSettings/InputManager.asset");

        // Replace the ProjectSettings file with the new one, and trigger a refresh so the Editor sees it
        FileUtil.ReplaceFile(sourceFile, targetFile);
        AssetDatabase.Refresh();

        Debug.Log("InputManager replaced!");
    }

    // Replace your layers and tags with the ones required by the VR Project
    [MenuItem("PlayStation VR/Individual Settings/Set VR Tag Manager")]
    static void ReplaceTagManager()
    {
        // This is the InputManager asset that comes with the example project. Note that to avoid an import error, the '.asset' file extension has been removed
        string sourceFile = Path.Combine(Application.dataPath, "PlayStation VR Example/Editor/TagManager");

        // This is the InputManager in your ProjectSettings folder
        string targetFile = Application.dataPath;
        targetFile = targetFile.Replace("/Assets", "/ProjectSettings/TagManager.asset");

        // Replace the ProjectSettings file with the new one, and trigger a refresh so the Editor sees it
        FileUtil.ReplaceFile(sourceFile, targetFile);
        AssetDatabase.Refresh();

        Debug.Log("TagManager replaced!");
    }

    // Replace the current list of the scenes for the build with the ones for the VR Example
    [MenuItem("PlayStation VR/Individual Settings/Set Build Scenes")]
    static void SetScenesForBuild()
    {
        EditorBuildSettingsScene[] vrScenes = new EditorBuildSettingsScene[5];
        vrScenes[0] = new EditorBuildSettingsScene("Assets/PlayStation VR Example/Scenes/PSVRExample_Setup.unity", true);
        vrScenes[1] = new EditorBuildSettingsScene("Assets/PlayStation VR Example/Scenes/PSVRExample_MainMenu.unity", true);
        vrScenes[2] = new EditorBuildSettingsScene("Assets/PlayStation VR Example/Scenes/PSVRExample_DualShock4.unity", true);
        vrScenes[3] = new EditorBuildSettingsScene("Assets/PlayStation VR Example/Scenes/PSVRExample_MoveControllers.unity", true);
        vrScenes[4] = new EditorBuildSettingsScene("Assets/PlayStation VR Example/Scenes/PSVRExample_AimController.unity", true);

        EditorBuildSettings.scenes = vrScenes;

        Debug.Log("Scenes in Build changed!");
    }
}
