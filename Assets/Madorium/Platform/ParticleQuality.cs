using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enables/Disables particle quality on platform basis
public class ParticleQuality : MonoBehaviour {

	// Use this for initialization
	void Awake() {
#if UNITY_PS4
        Destroy(gameObject);
#endif
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
