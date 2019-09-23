using UnityEngine;
using System.Collections;

public class LightningBolt : MonoBehaviour {

    Transform source;
    Transform target;
    LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }
	
    public void Setup(GameObject targ, GameObject src)
    {
        target = targ.transform;
        source = src.transform;
    }

	void Update ()
    {
        if(source != null && target != null)
        {
            lr.SetPosition(0, target.position);
            lr.SetPosition(1, source.position);
        }
	}
}
