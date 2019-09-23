using UnityEngine;
using System.Collections;

public class dark_collider : MonoBehaviour {

	public infinite_fantasy_pro infinite_fantasy_script;
	
	void OnTriggerEnter(Collider other) {
		
		infinite_fantasy_script.Dark_onClick ();

	}
	
	void OnTriggerStay(Collider other) {
		
		
		
		
	}
	
	void OnTriggerExit(Collider other) {
		
		
	}
}