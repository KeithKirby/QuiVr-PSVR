using UnityEngine;
using System.Collections;

public class LoadingScene : MonoBehaviour {

    public bool checkStart;
    bool loadingTutorial;
    //public bool LockTransition = true;
    public string StartScene;

	IEnumerator Start () {
        Debug.Log("LoadingScene::Start()");
        if(checkStart)
        {
            yield return new WaitForSeconds(0.25f);
            bool FoveHeadset = false;
#if UNITY_STANDALONE_WIN
            try { FoveHeadset = FoveInterface.IsHardwareConnected(); }
            catch { }
#endif
            if (FoveHeadset)
                Settings.Set("ThirdPerson", true);
            
            while (!PlayerProfile.Ready) // Wait for player profile to be ready
                yield return null;

            while (!PlatformSetup.Initialized) // Wait for platformsetup to complete
                yield return null;

            //while(LockTransition==true)
                //yield return null;

            StartCoroutine(ChangeScene.LoadSceneAsync(StartScene));
        }
	}

    public void Click()
    {
        if(!loadingTutorial)
        {
            loadingTutorial = true;
            StartTutorial();
        }
    }

    public static void StartTutorial()
    {
        Tutorial t = FindObjectOfType<Tutorial>();
        if(t != null && !t.inTutorial && !t.finishedTutorial && !PhotonNetwork.inRoom)
        {
            t.StartTutorialImmediate();
        }
        else
        {
            GameObject tut = new GameObject();
            tut.AddComponent<TutorialTrigger>();
            tut.name = "TutorialTrigger";
            ChangeScene.Swap(2);
        }
    }
}
