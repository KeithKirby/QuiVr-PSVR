using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour {

    public WormAnimations wanim;
    public AudioSource MouthSrc;
    public ParticleSystem RoarSpittle;
    public ParticleSystem DissolveParticles;
    public WormAudio[] AudioClips;
    Vector3 target;
    WormEvent wevent;
    public AnimationCurve RotateCurve;
    public BeautifulDissolves.Dissolve dissolve;
    public SkinnedMeshRenderer[] rend;
    public EnvTextureSwap.EnvTexture[] WormTextures;
    public WeakPoint[] WeakPoints;
    public bool canDamage;

    int currentPhase;

    void Awake()
    {
        wevent = GetComponentInParent<WormEvent>();
    }

    void Start()
    {
        MapTile mt = GetComponentInParent<MapTile>();
        if(mt != null)
        {
            foreach(var v in WormTextures)
            {
                if (mt.Environment == v.env)
                {
                    foreach(var r in rend)
                    {
                        r.material.mainTexture = v.tex;
                    }
                }
                    
            }
        }
    }

    public void Move(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
    }

    public void Dissolve()
    {
        if (dissolve != null)
            dissolve.TriggerDissolve();
        if(DissolveParticles != null)
            DissolveParticles.Play();
        Collider[] cs = GetComponentsInChildren<Collider>();
        foreach(var v in cs)
        {
            v.enabled = false;
        }
    }

    public void PlayRoar()
    {
        MouthSrc.clip = GetClip("InitRoar");
        MouthSrc.time = 0;
        MouthSrc.pitch = 0.9f;
        MouthSrc.Play();
        RoarSpittle.Play();
    }

    public void PlayClip(string clipName, float pitch = 1f)
    {
        MouthSrc.clip = GetClip(clipName);
        MouthSrc.time = 0;
        MouthSrc.pitch = pitch;
        MouthSrc.Play();
    }

    public void RockExplosion()
    {
        wevent.EnvEffects[1].Play();
        wevent.EnvEffects[1].GetComponent<AudioSource>().Play();
    }

    public void ActivateWeakPoint(int phase)
    {
        currentPhase = phase;
        int setID = (currentPhase - 1) * 2;
        WeakPoints[setID].Activate();
        WeakPoints[setID + 1].Activate();
    }

    public void ActivateRemainingPoints()
    {
        int setID = (currentPhase - 1) * 2;
        if (!WeakPoints[setID].broken)
            WeakPoints[setID].Activate();
        else if (!WeakPoints[setID + 1].broken)
            WeakPoints[setID + 1].Activate();
        else
        {
            Debug.Log("Both weak points destroyed, ending phase " + currentPhase);
            wevent.phaseComplete = true;
        }

    }

    public void BreakPoint(int ptID)
    {
        if (!canDamage)
            return;
        int setID = (currentPhase - 1) * 2;
        if(!WeakPoints[setID + ptID].broken)
        {
            WeakPoints[setID + ptID].Hit();
            if (ptID == 0)
                WeakPoints[setID + 1].Deactivate();
            else
                WeakPoints[setID].Deactivate();
            wevent.damaged = true;
        }
    }

    public void EnsurePhaseEnd(int phase)
    {
        int setID = (phase - 1) * 2;
        canDamage = false;
        WeakPoints[setID + 1].Deactivate();
        WeakPoints[setID].Deactivate();
    }

    public void PlayAnimation(string anim)
    {
        wanim.PlayAnim(anim);
    }

    public bool isAnimPlaying()
    {
        return wanim.isPlaying();
    }

    public float GetAnimLength(string anim)
    {
        return wanim.GetAnimLength(anim);
    }

    public AudioClip GetClip(string clipName)
    {
        foreach(var v in AudioClips)
        {
            if (v.name == clipName)
                return v.GetClip();
        }
        return null;
    }

    [System.Serializable]
    public class WormAudio
    {
        public string name;
        public AudioClip[] clip;

        public AudioClip GetClip()
        {
            if (clip.Length > 0)
                return clip[Random.Range(0, clip.Length)];
            return null;
        }

        public override string ToString()
        {
            if (name != null)
                return name;
            return base.ToString();
        }

    }

}
