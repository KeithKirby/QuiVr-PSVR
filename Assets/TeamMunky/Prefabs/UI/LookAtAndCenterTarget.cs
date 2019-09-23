using UnityEngine;

public class LookAtAndCenterTarget : MonoBehaviour
{
    public float Offset = 2.5f;
//	public Transform lookAtTarget;
	
	void Start () {
		//if(!lookAtTarget)
//			lookAtTarget = Camera.main.transform;
	}
	
	void LateUpdate ()
    {
        var ar = CameraRigs.ActiveRig;
        if (null != ar)
        {
            var arTfm = ar.transform;
            transform.position = arTfm.position + arTfm.forward * Offset;
            var toCamera = arTfm.position - transform.position;
            transform.LookAt(transform.position - toCamera);
        }
	}
}
