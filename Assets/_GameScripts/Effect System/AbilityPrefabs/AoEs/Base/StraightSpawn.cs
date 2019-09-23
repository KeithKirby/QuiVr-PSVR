using UnityEngine;
using System.Collections;

public class StraightSpawn : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        RaycastHit hit;
        int layer = 13;
        int mask = ~(1 << layer);
        if (Physics.Raycast(new Ray(transform.position + (transform.TransformDirection(Vector3.up) * 0.1f), transform.TransformDirection(Vector3.down)), out hit, 2f, mask))
        {
            Vector3 norm = hit.normal;
            transform.position += norm * 0.05f;
            transform.rotation = Quaternion.LookRotation(norm);
            transform.Rotate(Vector3.left, -90);
        }
    }
	
}
