using UnityEngine;
using System.Collections;

public class med_collider : MonoBehaviour {

	public infinite_fantasy_pro infinite_fantasy_script;
	
	void OnTriggerEnter(Collider other) {
		
		infinite_fantasy_script.Med_onClick ();
		
	}
}
