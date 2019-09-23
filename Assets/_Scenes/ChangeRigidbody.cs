using UnityEngine;
using System.Collections;
using VRTK;

public class ChangeRigidbody : MonoBehaviour {

	void Start()
	{
		GetComponent<VRTK_InteractableObject> ().InteractableObjectGrabbed += Release;
	}

	void Release(object o, InteractableObjectEventArgs e)
	{
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		transform.SetParent (null);
	}
}
