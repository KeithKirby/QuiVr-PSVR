using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailDistanceScale : MonoBehaviour {

    public TrailRenderer trail;
    public EffectSettings settings;

    void Update()
    {
        if(settings.Target != null)
        {
            float dist = Vector3.Distance(transform.position, settings.Target.transform.position);
            if (dist < 8)
            {
                trail.widthMultiplier = dist;
            }
        }
    }

}
