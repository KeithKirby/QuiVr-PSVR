using UnityEngine;
using System.Collections;

public class ambiant_harp_collider : MonoBehaviour {

	public infinite_fantasy_pro infinite_fantasy_script;
	
	void OnTriggerEnter(Collider other) {
		
		infinite_fantasy_script.Ambiant_Harp_onClick ();
		
	}
}
