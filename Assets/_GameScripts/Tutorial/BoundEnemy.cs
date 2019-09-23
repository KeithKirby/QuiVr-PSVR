using UnityEngine;
using System.Collections;

public class BoundEnemy : MonoBehaviour {

    public GameObject[] Bindings;

	public void Die()
    {
        foreach(var v in Bindings)
        {
            foreach(var rb in v.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
            }
        }
    }
}
