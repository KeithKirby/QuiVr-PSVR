using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PS4;

public class PS4Mouse : MonoBehaviour {

    RectTransform _rt;

	// Use this for initialization
	void Start () {
        _rt = GetComponent<RectTransform>();

    }
	
	// Update is called once per frame
	void Update () {
        var mousePos = Input.mousePosition;
        _rt.position = mousePos;
    }
}
