using UnityEngine;
using System.Collections;

public class ambiant_baroque_guitar : MonoBehaviour {

	public infinite_fantasy_pro infinite_fantasy_script;
	
	void OnTriggerEnter(Collider other) {
		
		infinite_fantasy_script.Ambiant_Baroque_Guitar_onClick ();
		
	}
}
