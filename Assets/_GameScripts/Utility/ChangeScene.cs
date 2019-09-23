using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class ChangeScene : MonoBehaviour
{
    public string Scene;
    public bool leaveNetwork;
    public bool onStart;
    public bool isNumber;

    public UnityEvent OnSetupChange;

    void Start()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        if (onStart)
            Invoke("Click", 3f);
    }

    public void Click()
    {
        if (PhotonNetwork.inRoom)
        {
            if (MPSphere.instance != null)
                MPSphere.instance.Click();
            else
                PhotonNetwork.LeaveRoom();
        }

        if (isNumber)
        {
            int s = 0;
            int.TryParse(Scene, out s);
            OnSetupChange.Invoke();
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync(s);
        }
        else
        {
            OnSetupChange.Invoke();
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync(Scene);
        }
    }

    public static void Swap(int sceneID)
    {
        if (PhotonNetwork.inRoom)
            PhotonNetwork.LeaveRoom();
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(sceneID);
    }

    public static IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log(string.Format("ChangeScene.Swap({0})", sceneName));
        if (PhotonNetwork.inRoom)
            PhotonNetwork.LeaveRoom();
        Time.timeScale = 1;
        AsyncOperation loadReq = SceneManager.LoadSceneAsync(sceneName);
        float lastProgress = loadReq.progress;
        loadReq.allowSceneActivation = false;
        while (loadReq.progress < 0.89)
        {
            if (loadReq.progress > lastProgress + 0.1f)
            {
                lastProgress += 0.1f;
                //Debug.Log("Load progress " + loadReq.progress);
            }
            yield return new WaitForSeconds(0.1f);
        }
        var sf = RenderMode.GetInst();
        sf.LoadTransition = true;
        sf.WaitForFadeOut("ChangeScene",
            () =>
            {
                loadReq.allowSceneActivation = true;
            });
    }
}