using UnityEngine;
using System.Collections;

public class IgnoreRotation : MonoBehaviour {

    Quaternion startRotation;

	// Use this for initialization
	void Start () {
        startRotation = transform.rotation * Quaternion.Inverse(transform.root.rotation); ;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.rotation = startRotation * transform.root.rotation;
	}
}
