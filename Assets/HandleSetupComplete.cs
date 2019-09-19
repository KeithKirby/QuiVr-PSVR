using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleSetupComplete : MonoBehaviour {

    private void Awake()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
    }

    // Use this for initialization
    void Start () {
        GameEventPublisher.SetupComplete += GameEventPublisher_SetupComplete;

    }

    void GameEventPublisher_SetupComplete()
    {
        GameEventPublisher.SetupComplete -= GameEventPublisher_SetupComplete;
        var fc = RenderMode.GetInst();
        fc.LoadTransition = true;
        fc.WaitForFadeOut("HandleSetupComplete",
            () =>
            {
                SceneManager.LoadScene(1);
                fc.LoadTransition = false;
            });
    }
}
