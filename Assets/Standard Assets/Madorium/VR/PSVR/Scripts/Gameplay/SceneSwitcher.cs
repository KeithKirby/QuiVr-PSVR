using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
	public CanvasGroup canvasGroup;
    public Camera MainCamera;
	public float fadeSpeed = 2f;

    LayerMask _initialCameraMask;
    public LayerMask LoadingMask;


    // Canvas starts fully visible, then fades down after a couple of seconds
    IEnumerator Start ()
	{
        canvasGroup.alpha = 1;
        _initialCameraMask = MainCamera.cullingMask;
        yield return new WaitForSeconds(2f);
        StartCoroutine( FadeDownRoutine () );
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
	}

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(FadeDownRoutine());
    }

    // Public call to then call the coroutine (using index)
    public void SwitchToScene (int sceneIndex)
	{
		StartCoroutine("SwitchToSceneRoutine", sceneIndex);
	}

    // Public call to then call the coroutine (using name)
    public void SwitchToScene(string sceneName)
    {
        StartCoroutine("SwitchToSceneRoutine", sceneName);
    }

    public delegate void OnSwitch();
    public void SwitchToScene(OnSwitch onReady)
    {
        StartCoroutine("SwitchToSceneRoutine", onReady);
    }

    IEnumerator FadeDownRoutine()
	{
        EnableGameRendering(true);

        while (canvasGroup.alpha > 0)
		{
			canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
			yield return null;
		}
        canvasGroup.alpha = 0;
    }

	// Switch to another scene after a short delay
	IEnumerator SwitchToSceneRoutine (int sceneIndex)
	{
        Debug.Log("##### 1");

		yield return new WaitForSeconds(0.5f);

        Debug.Log("##### 2");

        while (canvasGroup.alpha < 1)
		{
			canvasGroup.alpha += Time.deltaTime * fadeSpeed;
			yield return null;
		}

        Debug.Log("##### 3");

        EnableGameRendering(false);

        Debug.Log("##### 4");

        SceneManager.LoadSceneAsync(sceneIndex);

        Debug.Log("##### 5");
    }

    // Switch to another scene after a short delay
    IEnumerator SwitchToSceneRoutine(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        EnableGameRendering(false);

        SceneManager.LoadSceneAsync(sceneName);
    }

    void EnableGameRendering(bool enable)
    {
        MainCamera.cullingMask = enable ? _initialCameraMask : LoadingMask;
    }

    // Switch to another scene after a short delay    
    IEnumerator SwitchToSceneRoutine(OnSwitch onReady)
    {
        Debug.Log("##### 1");

        yield return new WaitForSeconds(0.5f);

        Debug.Log("##### 2");

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        Debug.Log("##### 3");

        EnableGameRendering(false);

        Debug.Log("##### 4");

        //SceneManager.LoadSceneAsync(sceneIndex);
        onReady();

        Debug.Log("##### 5");
    }
}
