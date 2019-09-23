using UnityEngine;
using System.Collections;

public class ArrowPrefab : MonoBehaviour {

    public MeshRenderer Tip;
    public MeshRenderer Shaft;
    public MeshRenderer[] Feathers;
    public TrailRenderer Trail;

    Arrow a;

    void Awake()
    {
        a = GetComponentInParent<Arrow>();
    }

    void FixedUpdate()
    {
        if (!ReferenceEquals(Trail, null) && !ReferenceEquals(a, null))
        {
            if (a.WasFired())
                Trail.enabled = true;
        }
    }

	public void Setup(Color[] c)
    {
        if (c.Length > 0 && Trail != null)
        {
            Trail.material.SetColor("_Color", c[0]);       
        }  
        if (c.Length > 1)
            Tip.material.color = c[1];
        if (c.Length > 2)
            Shaft.material.color = c[2];
        if (c.Length > 3)
        {
            foreach(var v in Feathers)
            {
                v.material.color = c[3];
            }
        }    
    }
}
