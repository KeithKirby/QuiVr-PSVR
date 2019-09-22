using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#if OBSERVR_VRTK
using VRTK;
#endif
#if OBSERVR_NVR
using NewtonVR;
#endif

[InitializeOnLoad]
public class ObserVR_ProjectSetup : EditorWindow, IHasCustomMenu {
    private static Dictionary<GameObject, bool> interactableObjects, spheres, AVProSpheres, NativeSpheres;
    private static Dictionary<string, bool> foundLibraries;
    private static List<string> foundLibraryNames;
    private static List<GameObject> sphereList, objectList, AVProList, NativePlayerList;
    private static string applicationID, applicationID_Dev;
    private static bool API_Status, AVProScriptsFound, NativePlayerScriptsFound, interactableObjectsFound, spheresFound, layersExist, librariesFound;
    private static bool[] layers, tags;
    private static int mainTab, currScene, numScenes, objectTypeTab, objectTab, mediaTab;
    private static ObserVR_ProjectSettings.Environment environment;
    private static string[] sceneNames, scenePaths;
    private static Vector2 sp, sp1, sp2, sp3, sp4, sp5, sp6, sp7, sp8;
    public static readonly Vector2 windowSize = new Vector2(350, 450);
    private static ObserVR_ProjectSettings settings;
    private static GUIStyle statusStyle, accountVerifiedStyle, accountUnverifiedStyle, unsavedSettingsStyle, frownyStyle;
    private GUIContent RefreshButton = new GUIContent("Refresh");

    /* Settings */
    private static int batchSize, maxSessionDuration;
    private static bool debugMode, prettyDebug, sendEventsOnDebugMode, useTeleportation;
    private static float generalFOVInterval, generalFOVMaxDistance, positionalTrackingInterval, minFramerateThreshold;
    /* End Settings */

#if OBSERVR_VRTK
    private static VRTK_InteractableObject[] interactableObjs_VRTK;
    private static bool teleporterExists;
#endif

#if OBSERVR_NVR
    private static NVRInteractableItem[] interactableObjs_NVR;
#endif

#if OBSERVR_AVPRO
    private static RenderHeads.Media.AVProVideo.MediaPlayer[] AVPro_Players;
#endif

#if UNITY_5_6_OR_NEWER
    private static UnityEngine.Video.VideoPlayer[] Native_Players;
#endif

    [MenuItem("Window/ObserVR/Settings %#o", false, 51)]
    static void Init() {
        ObserVR_ProjectSetup window = (ObserVR_ProjectSetup)GetWindow(typeof(ObserVR_ProjectSetup));
        window.titleContent = new GUIContent() {
            text = "Settings"
        };
        window.Show();
    }

    private void Awake() {
        Initialize();
    }

    public void AddItemsToMenu(GenericMenu menu) {
        menu.AddItem(RefreshButton, false, Initialize);
    }

    private static void Initialize() {
        CreateSettingsFile();
        objectTab = 0;
        objectTypeTab = -1;
        mediaTab = 0;
        currScene = -1;
        numScenes = 0;
        sp = sp1 = sp2 = sp3 = sp4 = sp5 = sp6 = sp7 = sp8 = Vector2.zero;
        statusStyle = new GUIStyle();
        accountVerifiedStyle = new GUIStyle();
        accountUnverifiedStyle = new GUIStyle();
        unsavedSettingsStyle = new GUIStyle();
        frownyStyle = new GUIStyle();
        API_Status = ObserVR_API.CheckAPIStatusSync();
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");

        statusStyle.fontSize = accountVerifiedStyle.fontSize = accountUnverifiedStyle.fontSize = unsavedSettingsStyle.fontSize = 18;
        frownyStyle.fontSize = 40;
        statusStyle.normal.textColor = Color.black;
        accountVerifiedStyle.normal.textColor = Color.green;
        accountUnverifiedStyle.normal.textColor = Color.red;
        accountUnverifiedStyle.fontStyle = accountVerifiedStyle.fontStyle = FontStyle.Bold;

        if (settings != null) {
            if (string.IsNullOrEmpty(settings.applicationID)) {
                settings.initialSetupDone = false;
            }
            applicationID = settings.applicationID;
            applicationID_Dev = settings.applicationID_Dev;
            environment = settings.environment;
            batchSize = settings.batchSize;
            debugMode = settings.debugMode;
            prettyDebug = settings.prettyDebug;
            sendEventsOnDebugMode = settings.sendEventsOnDebugMode;
            maxSessionDuration = settings.maxSessionDuration;
            useTeleportation = settings.useTeleportation;
            generalFOVInterval = settings.generalFOVInterval;
            generalFOVMaxDistance = settings.generalFOVMaxDistance;
            positionalTrackingInterval = settings.positionalTrackingInterval;
            minFramerateThreshold = settings.minFramerateThreshold;
            mainTab = settings.initialSetupDone ? 2 : 0;
            FindLibraries();
            GetScenes();
            GetInteractableObjects();
            GetLayers();
            GetTags();
            Get360Videos();
            GetSpheres();
        }
    }

    private static void Get360Videos() {
        AVProScriptsFound = false;
        AVProSpheres = new Dictionary<GameObject, bool>();
        AVProList = new List<GameObject>();

        NativePlayerScriptsFound = false;
        NativeSpheres = new Dictionary<GameObject, bool>();
        NativePlayerList = new List<GameObject>();

#if OBSERVR_AVPRO
        AVPro_Players = FindObjectsOfType<RenderHeads.Media.AVProVideo.MediaPlayer>();
        if (AVPro_Players != null) {
            if (AVPro_Players.Length > 0) {
                AVProScriptsFound = true;
            }

            foreach (RenderHeads.Media.AVProVideo.MediaPlayer obj in AVPro_Players) {
                AVProSpheres[obj.gameObject] = obj.gameObject.GetComponent<ObserVR_TrackedVideo>();
            }
        }
#endif

#if UNITY_5_6_OR_NEWER
        Native_Players = FindObjectsOfType<UnityEngine.Video.VideoPlayer>();
        if (Native_Players != null) {
            if (Native_Players.Length > 0) {
                NativePlayerScriptsFound = true;
            }

            foreach (UnityEngine.Video.VideoPlayer obj in Native_Players) {
                NativeSpheres[obj.gameObject] = obj.gameObject.GetComponent<ObserVR_TrackedVideo>();
            }
        }
#endif
        AVProList = new List<GameObject>(AVProSpheres.Keys).OrderBy(go => go.name).ToList();
        NativePlayerList = new List<GameObject>(NativeSpheres.Keys).OrderBy(go => go.name).ToList();
    }

    private static void GetSpheres() {
        spheresFound = false;
        spheres = new Dictionary<GameObject, bool>();
        sphereList = new List<GameObject>();
        MeshFilter[] meshfilters = FindObjectsOfType<MeshFilter>();
        if (meshfilters != null) {
            foreach (MeshFilter mf in meshfilters) {
                if (mf.sharedMesh != null && mf.sharedMesh.name.ToLower().Contains("sphere")) {
                    spheresFound = true;
                    sphereList.Add(mf.gameObject);
                }
            }
        }
        foreach (GameObject sphere in sphereList) {
            spheres[sphere] = sphere.GetComponent<ObserVR_TrackedPhoto>();
        }
        sphereList = new List<GameObject>(spheres.Keys).OrderBy(go => go.name).ToList();
    }

    private static void GetLayers() {
        layersExist = false;
        layers = new bool[24];
        for (int i = 8; i < 32; i++) {
            if (LayerMask.LayerToName(i).Length > 0) {
                layersExist = true;
                break;
            }
        }
        if (layersExist) {
            for (int i = 8; i < 32; i++) {
                if (LayerMask.LayerToName(i).Length > 0) {
                    bool allObjectsTracked = true;
                    GameObject[] objectsWithLayers = ObserVR_Functions.GetObjectsInLayer(i);
                    if (objectsWithLayers != null) {
                        foreach (var obj in objectsWithLayers) {
                            if (!obj.GetComponent<ObserVR_TrackedObject>()) {
                                allObjectsTracked = false;
                                break;
                            }
                        }
                    }
                    else {
                        allObjectsTracked = false;
                    }
                    layers[i - 8] = allObjectsTracked;
                }
            }
        }
    }

    private static void GetTags() {
        tags = new bool[UnityEditorInternal.InternalEditorUtility.tags.Length];
        for (int i = 0; i < tags.Length; i++) {
            bool allObjectsTracked = true;
            GameObject[] objectsWithTags = ObserVR_Functions.GetObjectsWithTag(UnityEditorInternal.InternalEditorUtility.tags[i]);
            if (objectsWithTags != null) {
                foreach (var obj in objectsWithTags) {
                    if (!obj.GetComponent<ObserVR_TrackedObject>()) {
                        allObjectsTracked = false;
                        break;
                    }
                }
            }
            else {
                allObjectsTracked = false;
            }
            tags[i] = allObjectsTracked;
        }
    }

    private static void GetInteractableObjects() {
        interactableObjectsFound = false;
        interactableObjects = new Dictionary<GameObject, bool>();
#if OBSERVR_VRTK
        teleporterExists = FindObjectOfType<VRTK_BasicTeleport>();
        interactableObjs_VRTK = Resources.FindObjectsOfTypeAll<VRTK_InteractableObject>();
        if (interactableObjs_VRTK != null) {
            if (interactableObjs_VRTK.Length > 0) {
                interactableObjectsFound = true;
            }

            foreach (VRTK_InteractableObject obj in interactableObjs_VRTK) {
                interactableObjects[obj.gameObject] = obj.gameObject.GetComponent<ObserVR_TrackedObject>();
            }
        }
#endif

#if OBSERVR_NVR
        interactableObjs_NVR = Resources.FindObjectsOfTypeAll<NVRInteractableItem>();

        if (interactableObjs_NVR != null) {
            if (interactableObjs_NVR.Length > 0) {
                interactableObjectsFound = true;
            }

            foreach (NVRInteractableItem obj in interactableObjs_NVR) {
                interactableObjects[obj.gameObject] = obj.gameObject.GetComponent<ObserVR_TrackedObject>();
            }
        }
#endif
        objectList = new List<GameObject>(interactableObjects.Keys).OrderBy(go => go.name).ToList();
    }

    private static void GetScenes() {
        numScenes = EditorBuildSettings.scenes.Length;
        sceneNames = new string[numScenes];
        scenePaths = new string[numScenes];
        if (numScenes > 0) {
            for (int i = 0; i < numScenes; i++) {
                scenePaths[i] = EditorBuildSettings.scenes[i].path;
                string name = EditorBuildSettings.scenes[i].path.Substring(EditorBuildSettings.scenes[i].path.LastIndexOf('/') + 1);
                var sceneNameLength = name.Length - 6;
                if (sceneNameLength >= 0) {
                    name = name.Substring(0, name.Length - 6);
                    sceneNames[i] = name;
                }

                if (SceneManager.GetActiveScene().name == sceneNames[i]) {
                    currScene = i;
                }
            }
        }
    }

    private static bool DoesTypeExist(string className) {
        var foundType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         from type in assembly.GetTypes()
                         where type.Name == className
                         select type).FirstOrDefault();

        return foundType != null;
    }

    private static bool DoesNamespaceExist(string desiredNamespace) {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Any(type => type.Namespace == desiredNamespace);
    }

    private static void FindLibraries() {
        foundLibraryNames = new List<string>();
        foundLibraries = new Dictionary<string, bool>();
        librariesFound = false;
        foreach (KeyValuePair<string, string> library in settings.supportedLibraries) {
            if (DoesTypeExist(library.Key) || DoesNamespaceExist(library.Key)) {
                librariesFound = true;
                foundLibraryNames.Add(library.Key);
                foundLibraries[library.Key] = DoesScriptingDefineSymbolExist(library.Value);

                if (!settings.initialSetupDone) {
                    AddScriptingDefineSymbol(library.Value);
                }
            }
        }
    }

    private static void AddScriptingDefineSymbol(string define) {
        if (string.IsNullOrEmpty(define)) {
            return;
        }
        string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> definesList = new List<string>(scriptingDefineSymbols.Split(';'));
        if (!definesList.Contains(define)) {
            definesList.Add(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", definesList.ToArray()));
        }
    }

    private static void RemoveScriptingDefineSymbol(string define) {
        if (string.IsNullOrEmpty(define)) {
            return;
        }
        string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> definesList = new List<string>(scriptingDefineSymbols.Split(';'));
        if (definesList.Contains(define)) {
            definesList.Remove(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", definesList.ToArray()));
        }
    }

    private static bool DoesScriptingDefineSymbolExist(string define) {
        if (string.IsNullOrEmpty(define)) {
            return false;
        }
        string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> definesList = new List<string>(scriptingDefineSymbols.Split(';'));
        return definesList.Contains(define);
    }

    private static void CreateSettingsFile() {
        ObserVR_ProjectSettings temp = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");
        if (temp == null) {
            temp = CreateInstance<ObserVR_ProjectSettings>();
            if (!Directory.Exists("Assets/Resources")) {
                Directory.CreateDirectory("Assets/Resources");
            }
            AssetDatabase.CreateAsset(temp, "Assets/Resources/ObserVR_ProjectSettings.asset");
        }
    }

    private void OnGUI() {
        var logo = Resources.Load<Texture2D>("ObserVR_Logo");
        var rect = GUILayoutUtility.GetRect(position.width - 8, 150, GUI.skin.box);
        if (logo) {
            GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);
        }
        if (settings != null) {
            GUILayout.Space(10);
            mainTab = GUILayout.Toolbar(mainTab, settings.mainToolbarStrings);
            switch (mainTab) {
                case 0:
                    InitialSetupTab();
                    break;
                case 1:
                    ObjectTrackingTab();
                    break;
                case 2:
                    TrackingSettingsTab();
                    break;
                default:
                    TrackingSettingsTab();
                    break;
            }
        }
        else {
            GUILayout.Space(15);
            EditorGUILayout.HelpBox("Project recently compiled, please refresh window", MessageType.Info);
            GUILayout.Space(30);
            if (GUILayout.Button("Refresh")) {
                Close();
                ObserVR_ProjectSetup window = (ObserVR_ProjectSetup)GetWindow(typeof(ObserVR_ProjectSetup));
                window.minSize = windowSize;
                window.titleContent = new GUIContent() {
                    text = "Settings"
                };
                window.Show();
            }
        }
    }

    void InitialSetupTab() {
        GUILayout.Space(10);
        if (API_Status) {
            GUILayout.Space(10);
            sp2 = EditorGUILayout.BeginScrollView(sp2);
            EditorGUILayout.LabelField("Application ID (Prod):", EditorStyles.boldLabel);
            applicationID = EditorGUILayout.TextField(applicationID);
            GUILayout.Space(15);
            EditorGUILayout.LabelField("Application ID (Dev) [Optional]:", EditorStyles.boldLabel);
            applicationID_Dev = EditorGUILayout.TextField(applicationID_Dev);
            GUILayout.Space(30);
            if (GUILayout.Button("Link Account", GUILayout.Height(50))) {
                if (!string.IsNullOrEmpty(applicationID.Trim())) {
                    if (ObserVR_API.CheckCredentialsSync(applicationID.Trim())) {
                        settings.initialSetupDone = true;
                        settings.applicationID = applicationID;
                        if (!string.IsNullOrEmpty(applicationID_Dev)) {
                            settings.applicationID_Dev = applicationID_Dev;
                            settings.environment = ObserVR_ProjectSettings.Environment.Dev;
                        }
                        EditorUtility.SetDirty(settings);
                        AssetDatabase.SaveAssets();

                        if (EditorBuildSettings.scenes.Length > 0) {
                            string openScene = SceneManager.GetActiveScene().path;
                            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
                            if (!GameObject.Find("ObserVR_Analytics")) {
                                GameObject g = (GameObject)Instantiate(Resources.Load("ObserVR_Analytics"));
                                g.name = "ObserVR_Analytics";
                            }
                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            EditorSceneManager.OpenScene(openScene);
                        }
                        if (EditorUtility.DisplayDialog("Success!", "Project Linked Successfully!", "Close")) {
                            Close();
                        }
                    }
                    else {
                        EditorUtility.DisplayDialog("Error!", "Could not validate Application ID.", "Close");
                    }
                }
                else {
                    EditorUtility.DisplayDialog("Error!", "Please enter a valid Application ID.", "Close");
                }
            }
            GUILayout.Space(40);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Status: ", statusStyle);
            GUILayout.Label(settings.initialSetupDone ? "Linked ✓" : "Unlinked", settings.initialSetupDone ? accountVerifiedStyle : accountUnverifiedStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (settings.initialSetupDone) {
                if (librariesFound) {
                    GUILayout.Space(40);
                    EditorGUILayout.LabelField("Supported libraries found in project:", EditorStyles.boldLabel);
                    GUILayout.Space(10);
                    foreach (string libraryName in foundLibraryNames) {
                        foundLibraries[libraryName] = GUILayout.Toggle(foundLibraries[libraryName], settings.supportedLibraryNames[libraryName]);
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button("Save")) {
                        foreach (string libraryName in foundLibraryNames) {
                            if (foundLibraries[libraryName]) {
                                AddScriptingDefineSymbol(settings.supportedLibraries[libraryName]);
                            }
                            else {
                                RemoveScriptingDefineSymbol(settings.supportedLibraries[libraryName]);
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
        else {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Cannot connect to ObserVR servers.");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(40);
            if (GUILayout.Button("Retry")) {
                API_Status = ObserVR_API.CheckAPIStatusSync();
            }
        }
    }

    void ObjectTrackingTab() {
        if (settings != null) {
            if (!settings.initialSetupDone) {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("You need to link your account in Project Settings");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("before you can add analytics.");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else {
                if (numScenes > 0) {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Select scene to find objects");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    currScene = EditorGUILayout.Popup(currScene, sceneNames, EditorStyles.toolbarDropDown);
                    if (currScene != -1 && SceneManager.GetActiveScene().path != scenePaths[currScene]) {
                        EditorSceneManager.OpenScene(scenePaths[currScene]);
                        Close();
                        ObserVR_ProjectSetup window = (ObserVR_ProjectSetup)GetWindow(typeof(ObserVR_ProjectSetup));
                        window.minSize = windowSize;
                        window.titleContent = new GUIContent() {
                            text = "Settings"
                        };
                        mainTab = 1;
                        window.Show();
                    }
                }
                GUILayout.Space(15);
                objectTab = GUILayout.Toolbar(objectTab, settings.objectTabStrings);
                GUILayout.Space(15);
                if (objectTab == 0) {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Select the objects you want to track:");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    EditorGUILayout.HelpBox("For each object selected, the following will be tracked automatically: \n •When the object is in the user's FOV \n •When the object is touched/grabbed/used (if using VRTK or NewtonVR)", MessageType.Info);
                    GUILayout.Space(10);
                    objectTypeTab = GUILayout.Toolbar(objectTypeTab, settings.objectTypeStrings);
                    GUILayout.Space(15);
                    if (objectTypeTab == 0) {
                        GUILayout.Space(5);
                        GUILayout.Label("Tag name:");
                        GUILayout.Space(5);
                        sp4 = EditorGUILayout.BeginScrollView(sp4);
                        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++) {
                            tags[i] = GUILayout.Toggle(tags[i], UnityEditorInternal.InternalEditorUtility.tags[i]);
                        }
                        EditorGUILayout.EndScrollView();
                        GUILayout.Space(15);
                        if (GUILayout.Button("Select All", GUILayout.Height(25))) {
                            for (int i = 0; i < tags.Length; i++) {
                                tags[i] = true;
                            }
                        }
                        if (GUILayout.Button("None", GUILayout.Height(25))) {
                            for (int i = 0; i < tags.Length; i++) {
                                tags[i] = false;
                            }
                        }
                        GUILayout.Label("\n");
                        if (GUILayout.Button("Save", GUILayout.Height(25))) {
                            for (int i = 0; i < tags.Length; i++) {
                                GameObject[] objectsWithTags = ObserVR_Functions.GetObjectsWithTag(UnityEditorInternal.InternalEditorUtility.tags[i]);
                                if (objectsWithTags != null) {
                                    foreach (GameObject obj in objectsWithTags) {
                                        if (tags[i]) {
                                            if (!obj.GetComponent<ObserVR_TrackedObject>()) {
                                                obj.AddComponent<ObserVR_TrackedObject>();
                                            }
                                        }
                                        else {
                                            if (obj.GetComponent<ObserVR_TrackedObject>()) {
                                                DestroyImmediate(obj.GetComponent<ObserVR_TrackedObject>());
                                            }
                                        }
                                    }
                                }
                            }
                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            EditorUtility.DisplayDialog("Saved!", "Your settings have been updated.", "Close");
                        }
                    }
                    else if (objectTypeTab == 1) {
                        if (layersExist) {
                            GUILayout.Space(5);
                            GUILayout.Label("Layer name:");
                            GUILayout.Space(5);
                            sp1 = EditorGUILayout.BeginScrollView(sp1);
                            for (int i = 8; i < 32; i++) {
                                if (LayerMask.LayerToName(i).Length > 0) {
                                    layers[i - 8] = GUILayout.Toggle(layers[i - 8], LayerMask.LayerToName(i));
                                }
                            }
                            EditorGUILayout.EndScrollView();
                            GUILayout.Space(15);
                            if (GUILayout.Button("Select All", GUILayout.Height(25))) {
                                for (int i = 8; i < 32; i++) {
                                    layers[i - 8] = true;
                                }
                            }
                            if (GUILayout.Button("None", GUILayout.Height(25))) {
                                for (int i = 8; i < 32; i++) {
                                    layers[i - 8] = false;
                                }
                            }
                            GUILayout.Label("\n");
                            if (GUILayout.Button("Save", GUILayout.Height(25))) {
                                for (int i = 8; i < 32; i++) {
                                    GameObject[] objectsWithLayers = ObserVR_Functions.GetObjectsInLayer(i);
                                    if (objectsWithLayers != null) {
                                        foreach (GameObject obj in objectsWithLayers) {
                                            if (layers[i - 8]) {
                                                if (!obj.GetComponent<ObserVR_TrackedObject>()) {
                                                    obj.AddComponent<ObserVR_TrackedObject>();
                                                }
                                            }
                                            else {
                                                if (obj.GetComponent<ObserVR_TrackedObject>()) {
                                                    DestroyImmediate(obj.GetComponent<ObserVR_TrackedObject>());
                                                }
                                            }
                                        }
                                    }
                                }
                                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                                EditorUtility.DisplayDialog("Saved!", "Your settings have been updated.", "Close");
                            }
                        }
                        else {
                            sp6 = EditorGUILayout.BeginScrollView(sp6);
                            EditorGUILayout.HelpBox("Could not find any user-defined layers", MessageType.Info);
                            EditorGUILayout.EndScrollView();
                        }
                    }
                    else if (objectTypeTab == 2) {
                        if (interactableObjectsFound) {
                            GUILayout.Space(5);
                            GUILayout.Label("Object name:");
                            GUILayout.Space(5);
                            sp = EditorGUILayout.BeginScrollView(sp);
                            foreach (GameObject objectName in objectList) {
                                interactableObjects[objectName] = GUILayout.Toggle(interactableObjects[objectName], objectName.name);
                            }
                            EditorGUILayout.EndScrollView();
                            GUILayout.Space(15);
                            if (GUILayout.Button("Select All", GUILayout.Height(25))) {
                                foreach (GameObject go in objectList) {
                                    interactableObjects[go] = true;
                                }
                            }
                            if (GUILayout.Button("None", GUILayout.Height(25))) {
                                foreach (GameObject go in objectList) {
                                    interactableObjects[go] = false;
                                }
                            }
                            GUILayout.Label("\n");
                            if (GUILayout.Button("Save", GUILayout.Height(25))) {
                                foreach (GameObject go in objectList) {
                                    if (go != null) {
                                        if (interactableObjects[go]) {
                                            if (!go.GetComponent<ObserVR_TrackedObject>()) {
                                                go.AddComponent<ObserVR_TrackedObject>();
                                            }
                                        }
                                        else {
                                            if (go.GetComponent<ObserVR_TrackedObject>()) {
                                                DestroyImmediate(go.GetComponent<ObserVR_TrackedObject>());
                                            }
                                        }
                                    }
                                }
                                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                                EditorUtility.DisplayDialog("Saved!", "Your settings have been updated.", "Close");
                            }
                        }
                        else {
                            sp5 = EditorGUILayout.BeginScrollView(sp5);
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("Could not find any objects with interactable scripts.");
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("You can manually add the \"ObserVR_TrackedObject.cs\"");
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("script to objects that you want to track in your FOV");
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            EditorGUILayout.EndScrollView();
                        }
                    }
                }
                else if (objectTab == 1) {
                    GUILayout.Space(15);
                    mediaTab = GUILayout.Toolbar(mediaTab, settings.mediaTabStrings);
                    GUILayout.Space(15);
                    if (mediaTab == 0) {
                        if (AVProScriptsFound || NativePlayerScriptsFound) {
                            GUILayout.Space(5);
                            GUILayout.Label("Found Videos:");
                            GUILayout.Space(5);
                            sp8 = EditorGUILayout.BeginScrollView(sp8);
                            if (AVProScriptsFound) {
                                GUILayout.Label("Objects using AVPro:", EditorStyles.boldLabel);
                                foreach (GameObject obj in AVProList) {
                                    AVProSpheres[obj] = GUILayout.Toggle(AVProSpheres[obj], obj.name);
                                }
                            }
                            if (NativePlayerScriptsFound) {
                                GUILayout.Label("Objects using native VideoPlayer:", EditorStyles.boldLabel);
                                foreach (GameObject obj in NativePlayerList) {
                                    NativeSpheres[obj] = GUILayout.Toggle(NativeSpheres[obj], obj.name);
                                }
                            }
                            EditorGUILayout.EndScrollView();
                            GUILayout.Space(15);
                            if (GUILayout.Button("Select All")) {
                                foreach (GameObject obj in AVProList) {
                                    AVProSpheres[obj] = true;
                                }
                                foreach (GameObject obj in NativePlayerList) {
                                    NativeSpheres[obj] = true;
                                }
                            }
                            if (GUILayout.Button("None")) {
                                foreach (GameObject obj in AVProList) {
                                    AVProSpheres[obj] = false;
                                }
                                foreach (GameObject obj in NativePlayerList) {
                                    NativeSpheres[obj] = false;
                                }
                            }
                            GUILayout.Label("\n");
                            if (GUILayout.Button("Save")) {
                                foreach (GameObject obj in AVProList) {
                                    if (obj != null) {
                                        if (AVProSpheres[obj]) {
                                            if (!obj.GetComponent<ObserVR_TrackedVideo>()) {
                                                obj.AddComponent<ObserVR_TrackedVideo>();
#if UNITY_5_6_OR_NEWER && OBSERVR_AVPRO
                                                obj.GetComponent<ObserVR_TrackedVideo>().playerType = ObserVR_TrackedVideo.VideoPlayerType.AVProVideoPlayer;
#endif
                                            }
                                        }
                                        else {
                                            if (obj.GetComponent<ObserVR_TrackedVideo>()) {
                                                DestroyImmediate(obj.GetComponent<ObserVR_TrackedVideo>());
                                            }
                                        }
                                    }
                                }
                                foreach (GameObject obj in NativePlayerList) {
                                    if (obj != null) {
                                        if (NativeSpheres[obj]) {
                                            if (!obj.GetComponent<ObserVR_TrackedVideo>()) {
                                                obj.AddComponent<ObserVR_TrackedVideo>();
#if UNITY_5_6_OR_NEWER && OBSERVR_AVPRO
                                                obj.GetComponent<ObserVR_TrackedVideo>().playerType = ObserVR_TrackedVideo.VideoPlayerType.NativeVideoPlayer;
#endif
                                            }
                                        }
                                        else {
                                            if (obj.GetComponent<ObserVR_TrackedVideo>()) {
                                                DestroyImmediate(obj.GetComponent<ObserVR_TrackedVideo>());
                                            }
                                        }
                                    }
                                }
                                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                                EditorUtility.DisplayDialog("Saved!", "Your settings have been updated.", "Close");
                            }
                        }
                        else {
                            EditorGUILayout.HelpBox("No objects containing supported video players were found in this scene", MessageType.Info);
                        }
                    }
                    else if (mediaTab == 1) {
                        if (spheresFound) {
                            GUILayout.Space(5);
                            GUILayout.Label("Found Spheres:");
                            GUILayout.Space(5);
                            sp7 = EditorGUILayout.BeginScrollView(sp7);
                            foreach (GameObject sphere in sphereList) {
                                spheres[sphere] = GUILayout.Toggle(spheres[sphere], sphere.name);
                            }
                            EditorGUILayout.EndScrollView();
                            GUILayout.Space(15);
                            if (GUILayout.Button("Select All")) {
                                foreach (GameObject sphere in sphereList) {
                                    spheres[sphere] = true;
                                }
                            }
                            if (GUILayout.Button("None")) {
                                foreach (GameObject sphere in sphereList) {
                                    spheres[sphere] = false;
                                }
                            }
                            GUILayout.Label("\n");
                            if (GUILayout.Button("Save")) {
                                foreach (GameObject sphere in sphereList) {
                                    if (sphere != null) {
                                        if (spheres[sphere]) {
                                            if (!sphere.GetComponent<ObserVR_TrackedPhoto>()) {
                                                sphere.AddComponent<ObserVR_TrackedPhoto>();
                                            }
                                        }
                                        else {
                                            if (sphere.GetComponent<ObserVR_TrackedPhoto>()) {
                                                DestroyImmediate(sphere.GetComponent<ObserVR_TrackedPhoto>());
                                            }
                                        }
                                    }
                                }
                                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                                EditorUtility.DisplayDialog("Saved!", "Your settings have been updated.", "Close");
                            }
                        }
                        else {
                            EditorGUILayout.HelpBox("Could not find any spheres in this scene", MessageType.Info);
                        }
                    }
                }
            }
        }
    }

    void TrackingSettingsTab() {
        if (settings != null) {
            if (!settings.initialSetupDone) {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("You need to link your account in Project Settings");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("before you can add analytics.");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                sp3 = EditorGUILayout.BeginScrollView(sp3);
                EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
                if (!string.IsNullOrEmpty(settings.applicationID) && !string.IsNullOrEmpty(settings.applicationID_Dev)) {
                    environment = (ObserVR_ProjectSettings.Environment)EditorGUILayout.EnumPopup("Environment", environment);
                }
                else {
                    environment = ObserVR_ProjectSettings.Environment.Prod;
                }
                batchSize = (int)EditorGUILayout.Slider(new GUIContent("Batch Size", "How many events should be sent in each batch [Default: 50]"), batchSize, 30, 100);
                debugMode = EditorGUILayout.Toggle(new GUIContent("Debug Mode", "If Debug Mode is enabled, events will print to the console as they occur (Editor Only). Should be disabled for production builds"), debugMode);
                if (debugMode) {
                    prettyDebug = EditorGUILayout.Toggle(new GUIContent("Debug (Pretty)", "If Debug (Pretty) is enabled, events will print to the console in an easy-to-read format"), prettyDebug);
                    sendEventsOnDebugMode = EditorGUILayout.Toggle(new GUIContent("Send Events on Debug", "By default, events will not send to the server while Debug Mode is enabled. If this setting is enabled, events will send to the server while Debug Mode is enabled"), sendEventsOnDebugMode);
                }
                maxSessionDuration = (int)EditorGUILayout.Slider(new GUIContent("Max Session Length", "If a session length (in hours) exceeds this duration, typically due to the app being left open and not actually being used, analytics will auto-shutoff [Default: 8]"), maxSessionDuration, 1, 72);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("General Field of View Settings", EditorStyles.boldLabel);
                generalFOVInterval = EditorGUILayout.Slider(new GUIContent("General FOV Interval", "How often to track a user's general FOV in a scene or 360°/Spherical video (in seconds) [Default: 1]"), generalFOVInterval, 1.0f, 30.0f);
                generalFOVMaxDistance = EditorGUILayout.FloatField(new GUIContent("Max Distance", "The maximum distance that a user's general FOV will register a hit [Default: 30]"), generalFOVMaxDistance);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Position Tracking Settings", EditorStyles.boldLabel);
#if OBSERVR_VRTK
                if (teleporterExists) {
                    useTeleportation =
                        EditorGUILayout.Toggle(
                            new GUIContent("Track Teleportation",
                                "Tracks when a player teleports"),
                            useTeleportation);
                }
                else {
                    useTeleportation = false;
                }
#endif
                positionalTrackingInterval = EditorGUILayout.Slider(new GUIContent("Tracking Interval", "How often you want to poll the user's position (in seconds) [Default: 1]"), positionalTrackingInterval, 0.5f, 30);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Performance Tracking Settings", EditorStyles.boldLabel);
                minFramerateThreshold = EditorGUILayout.Slider(new GUIContent("Min. FPS Threshold", "We will not track FPS if it is below this minimum threshold. This is because framerate could be low when first loading up app, loading scenes, etc. [Default: 20]"), minFramerateThreshold, 1.0f, 45.0f);

                if (settings.environment != environment ||
                    settings.batchSize != batchSize ||
                    settings.debugMode != debugMode ||
                    settings.prettyDebug != prettyDebug ||
                    settings.sendEventsOnDebugMode != sendEventsOnDebugMode ||
                    settings.maxSessionDuration != maxSessionDuration ||
                    settings.useTeleportation != useTeleportation ||
                    settings.generalFOVInterval != generalFOVInterval ||
                    settings.generalFOVMaxDistance != generalFOVMaxDistance ||
                    settings.positionalTrackingInterval != positionalTrackingInterval ||
                    settings.minFramerateThreshold != minFramerateThreshold) {
                    GUILayout.Space(45);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("You have unsaved changes", unsavedSettingsStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("☹", frownyStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUI.color = Color.yellow;
                    if (GUILayout.Button("Save Changes", GUILayout.Height(50))) {
                        if (!Application.isPlaying) {
                            settings.environment = environment;
                            settings.batchSize = batchSize;
                            settings.debugMode = debugMode;
                            settings.prettyDebug = prettyDebug;
                            settings.sendEventsOnDebugMode = sendEventsOnDebugMode;
                            settings.maxSessionDuration = maxSessionDuration;
                            settings.useTeleportation = useTeleportation;
                            settings.generalFOVInterval = generalFOVInterval;
                            settings.generalFOVMaxDistance = generalFOVMaxDistance;
                            settings.positionalTrackingInterval = positionalTrackingInterval;
                            settings.minFramerateThreshold = minFramerateThreshold;
                            EditorUtility.SetDirty(settings);
                            AssetDatabase.SaveAssets();
                            EditorUtility.DisplayDialog("Saved!", "Your changes have been saved!", "Close");
                        }
                        else {
                            EditorUtility.DisplayDialog("Alert!", "You cannot change settings while in Play mode", "Close");
                        }
                    }
                    GUI.color = Color.white;
                    if (GUILayout.Button("Revert Changes", GUILayout.Height(50))) {
                        environment = settings.environment;
                        batchSize = settings.batchSize;
                        debugMode = settings.debugMode;
                        prettyDebug = settings.prettyDebug;
                        sendEventsOnDebugMode = settings.sendEventsOnDebugMode;
                        maxSessionDuration = settings.maxSessionDuration;
                        useTeleportation = settings.useTeleportation;
                        generalFOVInterval = settings.generalFOVInterval;
                        generalFOVMaxDistance = settings.generalFOVMaxDistance;
                        positionalTrackingInterval = settings.positionalTrackingInterval;
                        minFramerateThreshold = settings.minFramerateThreshold;
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
