using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelFader : MonoBehaviour
{
	public CanvasGroup canvasGroup;
    public Camera MainCamera;
	public float fadeSpeed = 2f;
    public bool UseBusySpinner = false;

    enum LoadState
    {
        FadeIn,
        Active,
        FadeOut,
        Black
    }
    LoadState _state;

    LayerMask _initialCameraMask;
    public LayerMask LoadingMask;
    public bool Black
    {
        get
        {
            return _state == LoadState.Black;
        }
    }

    // Canvas starts fully visible, then fades down after a couple of seconds
    IEnumerator Start ()
	{
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        canvasGroup.alpha = 1;
        _initialCameraMask = MainCamera.cullingMask;
        yield return new WaitForSeconds(2f);        
        SceneManager.sceneLoaded += SceneManager_sceneLoaded; // Also fade in on loads

        StartCoroutine(FadeIn()); // First time, fade in
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(FadeIn());
    }

    public delegate void FadeComplete();
    public void FadeToBlack()
    {
        StartCoroutine(CloseLevelFade());
    }
    
    IEnumerator FadeIn()
	{
        Debug.Log("LevelFader.FadeIn Start");

        _state = LoadState.FadeIn;
        EnableGameRendering(true);

        // hide shitty init stuff
        yield return new WaitForSeconds(1.2f); 

        while (canvasGroup.alpha > 0)
		{
			canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
			yield return null;
		}
        canvasGroup.alpha = 0;
        yield return new WaitForEndOfFrame();
        _state = LoadState.Active;
        Debug.Log("LevelFader.FadeIn End");
    }

    IEnumerator CloseLevelFade()
    {
        while (LoadState.FadeIn == _state)
            yield return null;

        _state = LoadState.FadeOut;

        EnableGameRendering(true);
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1;
        EnableGameRendering(false);        
        yield return new WaitForEndOfFrame();
        _state = LoadState.Black;
    }
    
    void EnableGameRendering(bool enable)
    {/*
        MainCamera.cullingMask = enable ? _initialCameraMask : LoadingMask;
        if (UseBusySpinner)
        {
            if (FindObjectOfType<VRPostReprojection>())
                FindObjectOfType<VRPostReprojection>().busySpinner.SetActive(!enable);
        }*/
    }

    public void HackyForceCanvasRGBA( float alpha )
    {
        //canvasGroup.alpha = Mathf.Clamp01( alpha );
    }
}
