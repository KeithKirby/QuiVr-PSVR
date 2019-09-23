using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindMainCamera : MonoBehaviour
{
	// Use this for initialization
	void Start () {
        var canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
