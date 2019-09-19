using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class SetPlatformOptionsPS4
{
	[MenuItem("SCE Publishing Utils/QuiVR US")]
	static void SetConfigUS()
	{
        // Param file settings.
        PlayerSettings.PS4.category = PlayerSettings.PS4.PS4AppCategory.Application;
		PlayerSettings.PS4.appVersion = "01.00";
		PlayerSettings.PS4.masterVersion = "01.00";
		// Use the title id from the SCE np example project, using for now as we don't yet have all the required PSN services enabled for our own test title..
		//PlayerSettings.PS4.contentID = "ED1987-NPXX51138_00-0123456789ABCDEF";
		// This is the Unity example title content ID
		PlayerSettings.PS4.contentID = "UP3542-CUSA10675_00-QUIVR00000000000";
		// The title ID of NPXX51362_00 uses a NP Communication ID of NPWR05690_00 ... see https://ps4.scedev.net/titles/107929/products/118345
		
		PlayerSettings.productName = "QuiVr";
		PlayerSettings.PS4.parentalLevel = 1;
		PlayerSettings.PS4.enterButtonAssignment = PlayerSettings.PS4.PS4EnterButtonAssignment.CrossButton;
		PlayerSettings.PS4.paramSfxPath = "Assets/Editor/SonyNPPS4PublishData/QuiVR_US.sfx";

		// PSN Settings.
		PlayerSettings.PS4.NPtitleDatPath = "Assets/Editor/SonyNPPS4PublishData/us_nptitle.dat";
		PlayerSettings.PS4.npTrophyPackPath = "Assets/Editor/SonyNPPS4PublishData/trophy.trp";
		PlayerSettings.PS4.npAgeRating = 12;
		// This is the Unity example title secret ( NPXX51362 )  ...
		PlayerSettings.PS4.npTitleSecret = "8e77397c76940ab7b4fc21398eca5de1acfe1468857f54c36ee3f7f389838aae0f64107bda288bff584f844837b75780cfe2f1e4ecab45d78af1580fe27b3660bd7e321bec5d52e50ee20e66c9655a9226bb87ef1a7006138e23d0a3d88d5dc8e684f8ead9c588f5e7826f21d3aa49b2205909d43441d1a3ade222e2c89da4b2";

        var photonSettings = PhotonNetwork.PhotonServerSettings;                
        Undo.RecordObject(photonSettings, "Switch photon region");
        photonSettings.PreferredRegion = CloudRegionCode.us;
        photonSettings.AppID = "0b07c439-f499-4c59-b89d-6df056ef8977";
        photonSettings.VoiceAppID = "bae2d84c-ff3e-4ac6-9aae-d9836ccf2dcd";
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(photonSettings);
        AssetDatabase.SaveAssets();

        // Replace the old NpToolkit module with the NpToolkit2 version
        string[] modules = PlayerSettings.PS4.includedModules;

        if (modules.Length == 0)
        {
            Debug.Log("The player settings modules list is empty. Please open the player settings to initialise the list and try again.");
            return;
        }

        bool alreadySet = false;
        bool changed = false;

        for (int i = modules.Length - 1; i >= 0; i--)
        {
            if (modules[i].IndexOf("libSceNpToolkit.prx", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log("Swapped module libSceNpToolkit.prx for libSceNpToolkit2.prx");
                modules[i] = "libSceNpToolkit2.prx";
                changed = true;
            }
            else if (modules[i].IndexOf("libSceNpToolkit2.prx", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                alreadySet = true;
            }
        }
        PlayerSettings.PS4.includedModules = modules;

        if ( alreadySet == false && changed == false)
        {
            Debug.LogError("Unable to find libSceNpToolkit.prx or libSceNpToolkit2.prx in modules list.");
        }

        AssetDatabase.Refresh();
    }
    
    [MenuItem("SCE Publishing Utils/QuiVR EU")]
	static void SetConfigEU()
	{
        // Param file settings.
        PlayerSettings.PS4.category = PlayerSettings.PS4.PS4AppCategory.Application;
		PlayerSettings.PS4.appVersion = "01.00";
		PlayerSettings.PS4.masterVersion = "01.00";
		PlayerSettings.PS4.contentID = "EP3399-CUSA13201_00-QUIVR00000000000";
		
		PlayerSettings.productName = "QuiVr";
		PlayerSettings.PS4.parentalLevel = 1;
		PlayerSettings.PS4.enterButtonAssignment = PlayerSettings.PS4.PS4EnterButtonAssignment.CrossButton;
		PlayerSettings.PS4.paramSfxPath = "Assets/Editor/SonyNPPS4PublishData/QuiVR_EU.sfx";

		// PSN Settings.
		PlayerSettings.PS4.NPtitleDatPath = "Assets/Editor/SonyNPPS4PublishData/eu_nptitle.dat";
		PlayerSettings.PS4.npTrophyPackPath = "Assets/Editor/SonyNPPS4PublishData/trophy.trp";
		PlayerSettings.PS4.npAgeRating = 12;
		PlayerSettings.PS4.npTitleSecret = "ad26e8588925245fe21c8e4470da46e5f60a1d19a81e4e49ea1856119ac0d175c0aa1cae2261c52fb4899c56922a458b1e5903884bc1fadd185b3aae0851b113ce0693849ddf299ddf81a1d06d452b3ccc22e507c012beb0ac6430e10d0f618ab6ef1604cba2d555fb79eb96a9fa418f3e8aed39d130aed912739bb1468bab3a";

        var photonSettings = PhotonNetwork.PhotonServerSettings;
        Undo.RecordObject(photonSettings, "Switch photon region");
        photonSettings.PreferredRegion = CloudRegionCode.eu;
        photonSettings.AppID = "6b00286a-a65f-45b6-abd6-9f08ad13bb8e";
        photonSettings.VoiceAppID = "bae2d84c-ff3e-4ac6-9aae-d9836ccf2dcd";
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(photonSettings);
        AssetDatabase.SaveAssets();

        // Replace the old NpToolkit module with the NpToolkit2 version
        string[] modules = PlayerSettings.PS4.includedModules;

        if (modules.Length == 0)
        {
            Debug.Log("The player settings modules list is empty. Please open the player settings to initialise the list and try again.");
            return;
        }

        bool alreadySet = false;
        bool changed = false;

        for (int i = modules.Length - 1; i >= 0; i--)
        {
            if (modules[i].IndexOf("libSceNpToolkit.prx", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log("Swapped module libSceNpToolkit.prx for libSceNpToolkit2.prx");
                modules[i] = "libSceNpToolkit2.prx";
                changed = true;
            }
            else if (modules[i].IndexOf("libSceNpToolkit2.prx", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                alreadySet = true;
            }
        }
        PlayerSettings.PS4.includedModules = modules;

        if ( alreadySet == false && changed == false)
        {
            Debug.LogError("Unable to find libSceNpToolkit.prx or libSceNpToolkit2.prx in modules list.");
        }

        AssetDatabase.Refresh();
    }

    // Replace whatever Input Manager you currently have with one to work with the Nptoolkit Sample
    [MenuItem("SCE Publishing Utils/Set Input Manager")]
    static void ReplaceInputManager()
    {
        // This is the InputManager asset that comes with the example project. Note that to avoid an import error, the '.asset' file extension has been removed
        string sourceFile = Path.Combine(Application.dataPath, "Editor/InputManager");

        // This is the InputManager in your ProjectSettings folder
        string targetFile = Application.dataPath;
        targetFile = targetFile.Replace("/Assets", "/ProjectSettings/InputManager.asset");

        // Replace the ProjectSettings file with the new one, and trigger a refresh so the Editor sees it
        FileUtil.ReplaceFile(sourceFile, targetFile);
        AssetDatabase.Refresh();

        Debug.Log("InputManager replaced!");
    }
}
