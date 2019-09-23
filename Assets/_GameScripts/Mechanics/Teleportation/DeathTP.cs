using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTP : MonoBehaviour {

    public Vector3 Offset;

	public void UpdateLocation(Transform target, Vector3 offset)
    {
        transform.position = target.position + offset;// + target.parent.TransformDirection(Offset);
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }
}
