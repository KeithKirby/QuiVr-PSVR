using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPlayerCreate : MonoBehaviour {

    static int _count = 0;

	// Use this for initialization
	void Start()
    {
        Debug.LogFormat("Created Player {0} {1}", gameObject.name, _count++);
	}
}
