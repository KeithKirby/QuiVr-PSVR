using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicHinge : MonoBehaviour {

    public float ClosedAngle = 0;
    public float OpenAngle = 90;

    public int Steps = 5;
    public float StepDistance = 5;

    public float CloseRate = 10.0f;
    public float OpenRate = 10.0f;

    public Transform Hinge;

    Rigidbody _rb;

    int _contact = 0;
    float _angle = 0;

	// Use this for initialization
	void Start ()
    {
        _angle = OpenAngle;
        _rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate() {
        bool dirty = false;
		if(_contact > 0)
        {
            if (_angle != ClosedAngle)
            {
                dirty = true;
                _angle = Mathf.MoveTowardsAngle(_angle, ClosedAngle, CloseRate * Time.deltaTime);
            }
        }
        else
        {
            if (_angle != OpenAngle)
            {
                dirty = true;
                _angle = Mathf.MoveTowardsAngle(_angle, OpenAngle, OpenRate * Time.deltaTime);
            }
        }
        if (dirty)
        {
            var rotation = transform.localRotation.eulerAngles;
            rotation.x = _angle;
            transform.localRotation = Quaternion.Euler(rotation);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "HandModel") // Ugh
        {
            //++_contact;
        }
    }

    public Vector3 HingeUp
    {
        get
        {
            return transform.up;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "HandModel") // Ugh
        {
            Vector3 direction;
            float distance;
            var localCol = GetComponent<Collider>();
            var openDir = HingeUp;
            if(Physics.ComputePenetration(localCol, transform.position, transform.rotation, other, other.transform.position, other.transform.rotation, out direction, out distance))
            {
                float extractDistance;
                float dot = Vector3.Dot(openDir, direction);
                if(dot<0)
                {
                    extractDistance = localCol.bounds.extents.y - distance;
                }
                else
                {
                    extractDistance = distance;
                }
                Vector3 hinge = transform.position;
                Vector3 toOther = other.transform.position - hinge;
                float distToOther = Vector3.Dot(toOther, transform.forward); // - localCol.bounds.extents.z / 0.5f;

                Debug.Log("ToHinge:" + distToOther);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "HandModel") // Ugh
        {
            //--_contact;
        }
    }
}
