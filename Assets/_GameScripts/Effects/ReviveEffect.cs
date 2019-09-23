using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveEffect : MonoBehaviour {

    public ParticleSystem ReviveBeam;
    ParticleSystem.MainModule beamMod;
    public ParticleSystem RevivePoint;
    public RFX4_AudioCurves Sound;

    public PlayerSync sync;
    public PlayerLife life;
    public Transform Target;

    bool dead;

    void Awake()
    {
        if (ReviveBeam != null)
            beamMod = ReviveBeam.main;
    }

    bool changedDeath;
    void Update()
    {
        if(sync != null)
        {
            if(sync.isDead() != dead)
            {
                dead = sync.isDead();
                changedDeath = true;
            }
        }
        else if(life != null)
        {
            if (life.isDead != dead)
            {
                dead = life.isDead;
                changedDeath = true;
            }
        }
        if(changedDeath)
        {
            changedDeath = false;
            if(dead)
            {
                if(GameBase.instance != null)
                {
                    Health curTarg = GameBase.instance.CurrentTarget;
                    if (pvpmanager.instance.PlayingPVP)
                        curTarg = pvpmanager.instance.GetMyGate();
                    if (curTarg != null)
                        ReviveBeam.transform.position = curTarg.transform.position;
                }
                ReviveBeam.Play();
                RevivePoint.Play();
                if(Sound != null)
                    Sound.Play();
            }
            else
            {
                ReviveBeam.Stop();
                RevivePoint.Stop();
                if (Sound != null)
                    Sound.Reverse();
            }
        }
        if(dead)
        {
            float dist = Vector3.Distance(transform.position, Target.position);
            transform.LookAt(Target);
            beamMod.startSizeY = dist / 10f;
            RevivePoint.transform.position = Target.position;
        }
    }
}
