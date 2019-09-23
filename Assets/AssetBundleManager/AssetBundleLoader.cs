using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using AssetBundles;

public class AssetBundleLoader : MonoBehaviour {

    IEnumerator Start()
    {

        //Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
        //Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
        //Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);

        // rjy - might be a better global init spot for this
        yield return StartCoroutine(Initialize());
    }

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project. 
        // 	Another approach would be to make this configurable in the standalone player.)
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        //AssetBundleManager.SetDevelopmentAssetBundleServer();
        AssetBundleManager.SetSourceAssetBundleURL(Application.streamingAssetsPath + "/");
#else
		// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
		AssetBundleManager.SetSourceAssetBundleURL(Application.streamingAssetsPath + "/");
		// Or customize the URL based on your deployment or configuration
		//AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
#endif

        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();

        if (request != null)
            yield return StartCoroutine(request);
    }

    public static IEnumerator LoadBundledSceneAsync(string sceneName)
    {
        // Simple naming convention for bundled scenes
        string sceneAssetBundle = sceneName + "-bundle";

        Debug.Log(string.Format("ChangeScene.Swap({0})", sceneName));

        if (PhotonNetwork.inRoom)
            PhotonNetwork.LeaveRoom();

        Time.timeScale = 1;
        string error;
        LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(sceneAssetBundle, out error);
        if (bundle != null)
        {
            AsyncOperation loadReq = SceneManager.LoadSceneAsync(sceneName);

            float lastProgress = loadReq.progress;
            while (loadReq.progress < 0.9)
            {
                if (loadReq.progress > lastProgress + 0.1f)
                {
                    lastProgress += 0.1f;
                    Debug.Log("Load progress " + loadReq.progress);
                }
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("Load progress " + loadReq.progress);
        }
        else
            Debug.LogError("Bundle not loaded: " + sceneAssetBundle);
    }
}
