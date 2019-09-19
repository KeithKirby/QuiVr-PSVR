using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToArmory : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds(2);
        while (!PlatformSetup.Initialized)
            yield return null;
        while (false == Armory.ValidFetch)
            yield return null;

        var tp = GetComponent<Teleporter>();
        tp.ForceUse();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
