﻿using UnityEngine;
using System.Collections;

public class stop_collider : MonoBehaviour {

	public infinite_fantasy_pro infinite_fantasy_script;
	
	void OnTriggerEnter(Collider other) {
		
		infinite_fantasy_script.Stop_onClick ();
		
	}
}
