using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour {

    public Renderer mesh;
    Collider col;
    public bool toggleOnStart;
    public bool toggleOnKill;
    public ParticleSystem damageEffect;
    RFX4_ShaderColorGradient colorchange;

    [HideInInspector]
    public bool broken;

    void Awake()
    {
        col = GetComponent<Collider>();
        if (toggleOnStart)
            mesh.enabled = false;           
        col.enabled = false;
        colorchange = mesh.gameObject.GetComponent<RFX4_ShaderColorGradient>();    
    }

    public void Activate()
    {
        if(!broken)
        {
            col.enabled = true;
            mesh.enabled = true;
            colorchange.Play();
        }
    }

    public void Hit()
    {
        broken = true;
        if (damageEffect != null)
            damageEffect.Play();
        if (toggleOnKill)
            mesh.enabled = false;
        col.enabled = false;
        colorchange.Reverse();
    }

    public void Reset()
    {
        broken = false;
        if (toggleOnStart)
            Activate();
    }

    public void Deactivate()
    {
        col.enabled = false;
        if(toggleOnStart)
            mesh.enabled = false;
        colorchange.Reverse();
    }
}
