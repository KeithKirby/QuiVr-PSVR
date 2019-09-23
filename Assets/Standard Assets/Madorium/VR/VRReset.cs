using System.Collections;
using UnityEngine;

public class VRReset : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start() {
        yield return new WaitForSeconds(2);
        UnityEngine.XR.InputTracking.Recenter();
        PSVRManager.PublishRecenter();
    }
}