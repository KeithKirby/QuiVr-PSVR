using UnityEngine;
using System.Collections;
using VRTK;

public class DaggerArrow : MonoBehaviour {

	GameObject domHand;

	public void Grab()
	{
		foreach (var v in FindObjectsOfType<VRTK_InteractGrab>()) {
			v.ForceRelease ();
		}
		domHand = FindObjectOfType<BowSetup>().Right.gameObject;
		if(Settings.GetBool("LeftHanded"))
		{
			domHand = FindObjectOfType<BowSetup>().Left.gameObject;
		}
		domHand.GetComponent<VRTK_ControllerActions> ().TriggerHapticPulse (2000, 2, 1);
		domHand.GetComponent<VRTK_InteractTouch> ().ForceStopTouching ();
		domHand.GetComponent<VRTK_InteractTouch>().ForceTouch(gameObject);
		domHand.GetComponent<VRTK_InteractGrab> ().AttemptGrab ();
		
	}

	public void Release()
	{
		if (domHand != null) {
			domHand.GetComponent<VRTK_ControllerActions> ().TriggerHapticPulse( 1400, 2, 1);
		}
		foreach (var v in FindObjectsOfType<VRTK_InteractGrab>()) {
			v.ForceRelease ();
		}
		FindObjectOfType<BowSetup> ().Reset ();
	}
}
