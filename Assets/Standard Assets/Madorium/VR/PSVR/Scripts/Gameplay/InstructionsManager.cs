using UnityEngine;
using System.Collections;

public class InstructionsManager : MonoBehaviour
{
	public CanvasGroup instructionsCanvasGroup;
	public float fadeSpeed = 2f;
	private bool hasStarted = false;
	//private WeaponController[] weaponControls;

	void Start ()
	{
//		weaponControls = GameObject.FindObjectsOfType<WeaponController>();
	}

    public void BeginGame()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(BeginGameCoroutine());
        }
    }

	// Fade down the instructions canvas, wait a couple seconds, and then start spawning targets and running the timer
	IEnumerator BeginGameCoroutine ()
	{
        instructionsCanvasGroup.interactable = false;

        while (instructionsCanvasGroup.alpha > 0)
		{
			instructionsCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
			yield return null;
		}

		yield return new WaitForSeconds(2f);

		//for(int i=0; i<weaponControls.Length; i++)
			//weaponControls[i].WeaponCanShoot(true);

		FindObjectOfType<TargetsManager>().BeginSpawning();
		FindObjectOfType<PSVRScoreManager>().StartTimer();
	}
}
