using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRelativeScale))]
public class RelativeScaleSetEyesCamera : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        while (null == EyesCamera.Inst)
            yield return null;

        var relativeScale = GetComponent<CameraRelativeScale>();
        relativeScale.SetTargetCamera(EyesCamera.Inst.EyesCam);
	}
	
}
