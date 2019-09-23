using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BloodFader : MonoBehaviour
{
	public CanvasGroup canvasGroup;
    public Camera MainCamera;
	public float fadeSpeed = 0.5f;

    bool _active;

    void Awake()
    {
        ClearBloodEffect();
    }

    void Start ()
	{
        ClearBloodEffect();
    }
    
    public void ClearBloodEffect()
    {
        canvasGroup.alpha = 0.0f;
        _active = false;
    }

    public void TriggerBloodEffect(float alpha = 0.25f)
    {
        if( _active )
            return;

        canvasGroup.alpha = alpha;
        _active = true;
        StartCoroutine(FadeToClearRoutine());
    }

    IEnumerator FadeToClearRoutine()
	{
        while (canvasGroup.alpha > 0.0f)
		{
			canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
			yield return null;
		}
        canvasGroup.alpha = 0;
        _active = false;
    }

}
