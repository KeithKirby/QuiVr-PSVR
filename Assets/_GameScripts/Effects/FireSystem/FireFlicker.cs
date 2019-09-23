using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Light))]
public class FireFlicker : MonoBehaviour {

    Light l;
    float initIntens;
    public float flickerIntens;
	// Use this for initialization
	void Start () {
        l = GetComponent<Light>();
        initIntens = l.intensity;
	}

    float t;
	void Update () {
        l.intensity = (Mathf.Sin(t) * flickerIntens) + initIntens;
        t += Random.Range(-0.1f, 0.4f);
        if (t > 10 * Mathf.PI)
            t = 0;
	}
}
