using UnityEngine;
using System.Collections;

public class battle_collider : MonoBehaviour {

	public infinite_fantasy_pro infinite_fantasy_script;
	
	void OnTriggerEnter(Collider other) {
		
		infinite_fantasy_script.Battle_onClick ();
		infinite_fantasy_script.Forte_onClick ();

		
	}
}
