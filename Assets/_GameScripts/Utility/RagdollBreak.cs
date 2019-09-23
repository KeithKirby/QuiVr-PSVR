using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollBreak : MonoBehaviour {

    [BitStrap.Button]
    void Test()
    {
        foreach(var v in GetComponentsInChildren<Rigidbody>())
        {
            v.isKinematic = false;
        }
        BreakHierarchy();
    }
	
    public void BreakHierarchy()
    {
        foreach (var v in GetComponentsInChildren<Rigidbody>())
        {
            if (v.GetComponent<Joint>() == null)
                v.transform.SetParent(transform);
        }
    }

}
