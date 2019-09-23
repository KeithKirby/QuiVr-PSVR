using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Flower : MonoBehaviour
{
    public Worm.WormAudio[] AudioClips;
    public WormAnimations wanim;
    public AudioSource MouthSrc;
    public ParticleSystem LayerSpawn;
    public ParticleSystem Spiral;
    FlowerEvent wevent;
    List<EventSpear> Spears;
    public bool canDamage;

    void Awake()
    {
        wevent = GetComponentInParent<FlowerEvent>();
        EventSpear[] spears = GetComponentsInChildren<EventSpear>();
        Spears = new List<EventSpear>();
    }

    public void SetupSpears(System.Random rand)
    {
        //Get all spear options
        EventSpear[] spears = GetComponentsInChildren<EventSpear>();
        spears = spears.OrderBy(x => rand.Next()).ToArray();

        //Setup Used Spears
        int numSpears = 3;
        for(int i=0; i<numSpears; i++)
        {
            Spears.Add(spears[i]);
        }
        for(int i=numSpears; i<spears.Length; i++)
        {
            spears[i].gameObject.SetActive(false);
        }

        //Set code for spears
        for (int i = 0; i < Spears.Count; i++)
        {
            int x = i;
            Spears[i].OnSpearHit.AddListener(delegate { BreakSpear(x); });
        }
    }

    public void PlayAnimation(string anim)
    {
        wanim.PlayAnim(anim);
    }

    public bool isAnimPlaying()
    {
        return wanim.isPlaying();
    }

    public void ActivateRandomSpear(System.Random rand)
    {
        List<EventSpear> unbroken = new List<EventSpear>();
        foreach(var v in Spears)
        {
            if (!v.Broken)
                unbroken.Add(v);
        }
        if (unbroken.Count > 0)
            unbroken[rand.Next(0, unbroken.Count)].Activate();
    }

    public void BreakSpear(int ptID)
    {
        Debug.Log("Breaking Spear: " + (ptID+1) + " [of " + Spears.Count + "]");
        if (!canDamage)
            return;
        if (!Spears[ptID].Broken)
        {
            Spears[ptID].Break();
            wevent.damaged = true;
            canDamage = false;
        }
    }

    public bool isDead()
    {
        foreach(var v in Spears)
        {
            if (!v.Broken)
                return false;
        }
        return true;
    }

    public float GetAnimLength(string anim)
    {
        return wanim.GetAnimLength(anim);
    }

    public AudioClip GetClip(string clipName)
    {
        foreach (var v in AudioClips)
        {
            if (v.name == clipName)
                return v.GetClip();
        }
        return null;
    }

    public void PlayClip(string clipName, float pitch = 1f)
    {
        MouthSrc.clip = GetClip(clipName);
        MouthSrc.time = 0;
        MouthSrc.pitch = pitch;
        MouthSrc.Play();
    }
}
