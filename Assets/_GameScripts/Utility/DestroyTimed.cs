using UnityEngine;
using System.Collections;

public class DestroyTimed : MonoBehaviour {

    public float lifeTime;

	void Start () {
        Destroy(gameObject, lifeTime);
	}
}
