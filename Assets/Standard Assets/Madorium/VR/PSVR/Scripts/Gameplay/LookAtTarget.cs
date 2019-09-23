using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
	public Transform lookAtTarget;
	
	void Start () {
		if(!lookAtTarget)
			lookAtTarget = Camera.main.transform;
	}
	
	void LateUpdate () {
		transform.LookAt(2 * transform.position - lookAtTarget.position);
	}
}
