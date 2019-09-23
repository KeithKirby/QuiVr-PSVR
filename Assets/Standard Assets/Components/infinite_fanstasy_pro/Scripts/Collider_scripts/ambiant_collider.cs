using UnityEngine;
using System.Collections;

public class ambiant_collider : MonoBehaviour {

	
	public infinite_fantasy_pro infinite_fantasy_script;
	
	void OnTriggerEnter(Collider other) {
		

	}
	
	void OnTriggerStay(Collider other) {
		

		infinite_fantasy_script.Ambiant_Light_onClick ();


	}
	
	void OnTriggerExit(Collider other) {

				
	}
}
