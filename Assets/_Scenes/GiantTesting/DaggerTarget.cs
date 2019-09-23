using UnityEngine;
using System.Collections;
using UnityEngine.Events;
 
public class DaggerTarget : MonoBehaviour {

	public UnityEvent OnStab;

	public void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.GetComponent<DaggerArrow>() != null) {
			col.gameObject.GetComponent<DaggerArrow>().Release ();
			OnStab.Invoke ();
		}
	}
}
