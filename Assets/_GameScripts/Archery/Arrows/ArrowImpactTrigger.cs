using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArrowImpactTrigger : MonoBehaviour {

    public UnityEvent OnTrigger;

	void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Arrow>() != null)
        {
            Debug.Log("Arrow Triggered");
            OnTrigger.Invoke();
        }
    }
}
