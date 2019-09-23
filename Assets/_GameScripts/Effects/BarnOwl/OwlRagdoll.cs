using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class OwlRagdoll : MonoBehaviour {

    Rigidbody[] rbs;
    OwlHead headLook;
    Animation a;

    Transform[] Bones;
    Vector3[] BoneStartPts;
    Quaternion[] BoneStartRots;

    Vector3 startPos;
    Quaternion startRot;

    public UnityEvent OnKill;

    public bool GameEnabled = false; //caleb
    public int KillsToStart = 12; //caleb
    public int TimesKilled;
    public bool oneLife;
    public bool alwaysStartEvent = false;
    public int ArrowMultiplier = 2; //caleb : 2 is good number, 75 was default
    public int DeathDissolveTimer = 0; //caleb : 2 was default
    
    

    void Awake()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        BoneStartPts = new Vector3[rbs.Length];
        BoneStartRots = new Quaternion[rbs.Length];
        headLook = GetComponentInChildren<OwlHead>();
        for (int i = 0; i < rbs.Length; i++)
        {
            Rigidbody v = rbs[i];
            v.isKinematic = true;
            BoneStartPts[i] = v.transform.localPosition;
            BoneStartRots[i] = v.transform.localRotation;
        }
        a = GetComponent<Animation>();
    }
	
	public void Ragdoll(ArrowCollision impact)
    {
        TimesKilled++;
        a.enabled = false;
        headLook.Looking = false;
        foreach(var v in rbs)
        {
            v.velocity = Vector3.zero;
            v.angularVelocity = Vector3.zero;
            v.isKinematic = false;
        }
        Rigidbody rb = impact.hitObj!=null?impact.hitObj.GetComponent<Rigidbody>():null;
        float mult = impact.ArrowDamage / 35f;
        if (rb != null)
            rb.AddForceAtPosition(impact.ImpactVelocity.normalized * ArrowMultiplier * impact.velocityMag * mult, impact.impactPos); 
        OnKill.Invoke();    
        Invoke("Dissolve", DeathDissolveTimer);
        if (!oneLife)
        {           
            Invoke("ResetFromKill", 5);
        }
    }

    bool playedOnce;
    public void ResetKills()
    {
        TimesKilled = 0;
        playedOnce = false;
    }

    void Dissolve()
    {
        GetComponentInChildren<BeautifulDissolves.Dissolve>().TriggerDissolve();
    }

    void ResetFromKill()
    {
        int anger = 0;
        if (TimesKilled > 2)
            anger = 1;
        else if (TimesKilled > 5)
            anger = 2;
        if((TimesKilled >= KillsToStart || Armory.HasEffectEquipped(13) || alwaysStartEvent) && GameEnabled && !PhotonNetwork.inRoom)
        {
            //TimesKilled = 8;
            //Start Owl Minigame
            if(!playedOnce)
            {
                playedOnce = true;
                AAManager.instance.StartGameInitial();
            }
            else
                AAManager.instance.NewGame();
        }
        else
        {
            GetComponent<OwlSounds>().PlayRespawn(anger);
        }
        Reset();
    }

    public void Reset()
    {
        CancelInvoke();
        BeautifulDissolves.Dissolve dissolve = GetComponentInChildren<BeautifulDissolves.Dissolve>();
        dissolve.ResetDissolve();
        for (int i = 0; i < rbs.Length; i++)
        {
            Rigidbody v = rbs[i];
            v.velocity = Vector3.zero;
            v.angularVelocity = Vector3.zero;
            v.isKinematic = true;
            v.transform.localPosition = BoneStartPts[i];
            v.transform.localRotation = BoneStartRots[i];
        }
        GetComponent<OwlEvents>().RespawnEffect();
        a.enabled = true;
        headLook.Looking = true;
    }
}
