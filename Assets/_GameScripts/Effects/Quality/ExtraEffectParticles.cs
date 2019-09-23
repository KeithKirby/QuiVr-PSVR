using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExtraEffectParticles : MonoBehaviour {

    ParticleSystem p;
    ParticleSystem.EmissionModule e;
    public int FXClass = 1;
    public bool destroy;
    public bool enable;
    public bool useEmmission;

    public static ExtraEffectParticles mainInstance;
    static List<ExtraEffectParticles> lst;

    void Awake()
    {
        if (mainInstance != null)
            mainInstance = this;
        if (lst == null)
            lst = new List<ExtraEffectParticles>();
        lst.Add(this);
        p = GetComponent<ParticleSystem>();
        if(p != null)
            e = p.emission;
        if (destroy && Settings.GetInt("EffectQuality", 1) < FXClass && !enable)
            Destroy(p);
        else if (destroy && Settings.GetInt("EffectQuality", 1) >= FXClass && enable)
            Destroy(p);
        InvokeRepeating("InstanceCheck", 0.5f, Random.Range(0.32f, 0.65f));
    }

    private void OnDestroy()
    {
        if(mainInstance == this && lst.Count > 1)
        {
            if (lst[0] != this)
                mainInstance = lst[0];
            else
                mainInstance = lst[1];
            lst.Remove(this);
        }
    }

    void InstanceCheck()
    {
        if(mainInstance == this)
        {
            int fxq = Settings.GetInt("EffectQuality", 1);
            foreach(var v in lst)
            {
                v.CheckSettings(fxq);
            }
        }
    }

    void CheckSettings(int fxq)
    {
        if(p != null && !destroy)
        {
            bool extra = fxq >= FXClass;
            if (enable)
                extra = !extra;
            if (extra)
            {
                if (!useEmmission && !p.isPlaying)
                    p.Play();
                else if (useEmmission && !e.enabled)
                    e.enabled = true;
            }
            else
            {
                if (p.isPlaying && !useEmmission)
                    p.Stop();
                else if (useEmmission && e.enabled)
                    e.enabled = false;
            }
        }
    }
}
