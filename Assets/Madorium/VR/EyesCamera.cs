using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesCamera : MonoBehaviour {

    public Camera EyesCam;

    static public EyesCamera Inst;

	void Awake () {
        if (null == Inst)
        {
            Inst = this;
        }
        else
        {
            Debug.LogError("[EyesCamera] Found more than 1 instance, removing : " + transform.name);
            Destroy(this.gameObject);
        }
	}
	
}
