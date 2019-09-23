using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSpear : MonoBehaviour {

    public Color OffColor;
    public Color OnColor;
    public Color CracksOnColor;

    public MeshRenderer Gem;
    public Projector CrackProjector;
    public ParticleSystem BreakParticles;
    public ParticleSystem ActivePT;

    public bool Activated;
    public bool Broken;
    public UnityEvent OnSpearHit;

    public AudioClip BreakClip;
    public AudioClip ActivateClip;

    void Start()
    {
        CrackProjector.material = new Material(CrackProjector.material);
    }
     
    public void GemHit(ArrowCollision col)
    { 
        if(Activated && !Broken)
        {
            OnSpearHit.Invoke();
        }
    }

    public void Activate()
    {
        if(!Broken)
        {
            Gem.material.SetColor("_EmissionColor", OnColor * 3.5f);
            CrackProjector.material.SetColor("_TintColor", CracksOnColor * 2f);
            Activated = true;
            if (ActivePT != null)
                ActivePT.Play();
            VRAudio.PlayClipAtPoint(ActivateClip, Gem.transform.position, 1, 1, 0.95f);
        }
    }

    public void Deactivate()
    {
        Gem.material.SetColor("_EmissionColor", OffColor * 0.25f);
        CrackProjector.material.SetColor("_TintColor", OffColor * 0.175f);
        if (ActivePT != null)
            ActivePT.Stop();
    }

    public void Break()
    {
        Broken = true;
        Activated = false;
        if (BreakParticles != null)
            BreakParticles.Play();
        if (ActivePT != null)
            ActivePT.Stop();
        Gem.enabled = false;
        VRAudio.PlayClipAtPoint(BreakClip, Gem.transform.position, 1, 1, 0.95f);
        Deactivate();
    }
}
 