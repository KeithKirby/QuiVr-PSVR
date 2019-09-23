using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volleyball_Event : EventTile
{
    public Transform[] Blockers;
    public GameObject TargetPlatform;
    public int PlatformHitReq = 5;
   
    public float TargetMoveDist;
    public float MoveDistance;

    [Header("Gate Damage")]
    public GameObject LaserPrefab;
    public GameObject LaserImpact;
    public int LaserDamage;

    public int timesHit = 0;

    [Header("Runtime")]
    public float[] BlockerSpeed;
    float[] CurBlockerProg;
    int[] BlockerDir;
    public float PlatSpeed;
    float CurPlatProg;
    int platDir = 1;

    public override void Awake()
    {
        BlockerSpeed = new float[Blockers.Length];
        BlockerDir = new int[Blockers.Length];
        CurBlockerProg = new float[Blockers.Length];
        base.Awake();
    }

    public override void StartIntro()
    {
        if (!startedIntro)
        {
            timesHit = 0;
            StartCoroutine("Intro");
            base.StartIntro();
            SetValues();
        }
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(4);
        OnIntroEnd();
    }

    public override void StartEvent()
    {
        Debug.Log("Starting Event Loop");
        StartCoroutine("EventLoop");
        base.StartEvent();
    }

    IEnumerator EventLoop()
    {
        while(timesHit < PlatformHitReq)
        {
            UpdatePlatValues();
            yield return true;
        }
        yield return new WaitForSeconds(1f);
        if (!TargetGate.destroyed)
        {
            EndEvent();
        }
    }

    void SetValues()
    {
        PlatSpeed = GetRandomNext(6f, 7f);
        CurPlatProg = GetRandomNext(0f, 1f);
        BlockerSpeed = new float[Blockers.Length];
        BlockerDir = new int[Blockers.Length];
        CurBlockerProg = new float[Blockers.Length];
        for (int i=0; i<BlockerSpeed.Length; i++)
        {
            BlockerSpeed[i] = GetRandomNext(1.5f, 2.3f);
            BlockerDir[i] = (int)Mathf.Sign(GetRandomNext(-5f, 5f));
            CurBlockerProg[i] = GetRandomNext(0f, 1f);
        }
        //Initial Positions
        Vector3 pos = TargetPlatform.transform.localPosition;
        pos.x = (TargetMoveDist * CurPlatProg) - (TargetMoveDist / 2f);
        TargetPlatform.transform.localPosition = pos;

        for(int i=0; i<Blockers.Length; i++)
        {
            float x = (CurBlockerProg[i] * MoveDistance) - (MoveDistance / 2f);
            pos = Blockers[i].localPosition;
            pos.x = x;
            Blockers[i].localPosition = pos;
        }
    }

    void UpdatePlatValues()
    {
        CurPlatProg += (Time.deltaTime/PlatSpeed) * platDir;
        if(platDir < 0 && CurPlatProg <= 0)
        { platDir = 1; CurPlatProg = 0; }
        else if(platDir > 0 && CurPlatProg >= 1)
        { platDir = -1; CurPlatProg = 1; }
        Vector3 pos = TargetPlatform.transform.localPosition;
        pos.x = (TargetMoveDist * CurPlatProg) - (TargetMoveDist / 2f);
        TargetPlatform.transform.localPosition = pos;
        for(int i=0; i<Blockers.Length;i++)
        {
            CurBlockerProg[i] += (Time.deltaTime/BlockerSpeed[i]) * BlockerDir[i];
            if (BlockerDir[i] < 0 && CurBlockerProg[i] <= 0)
            { BlockerDir[i] = 1; CurBlockerProg[i] = 0; }
            else if (BlockerDir[i] > 0 && CurBlockerProg[i] >= 1)
            { BlockerDir[i] = -1; CurBlockerProg[i] = 1; }
            SetBlockerPosition(i);
        }
    }

    void SetBlockerPosition(int blockerID)
    {
        if(Blockers[blockerID] != null)
        {
            float x = (CurBlockerProg[blockerID] * MoveDistance) - (MoveDistance/2f);
            Vector3 pos = Blockers[blockerID].localPosition;
            pos.x = x;
            Blockers[blockerID].localPosition = pos;
        }
    }

    public void DoPlatformHit()
    {
        if (EventManager.InEvent)
            mgr.IntEvent2(this, 1);
    }

    public override void IntEvent2Response(int val)
    {
        PlatformHit();
    }

    void PlatformHit()
    {
        timesHit++;
    }

    public void BlockerHit(ArrowCollision c)
    {
        GameObject hitObj = c.hitObj;
        for(int i =0; i<Blockers.Length; i++)
        {
            if(Blockers[i] == hitObj.transform)
            {
                DoHit(i);
                return;
            }
        }
    }

    void DoHit(int BlockerID)
    {
        if (EventManager.InEvent)
            mgr.IntEvent1(this, BlockerID);
    }

    public override void IntEvent1Response(int val)
    {
        DamageGate(Blockers[val].position);
    }

    public void DamageGate(Vector3 startPt)
    {
        if (LaserPrefab != null)
        {
            GameObject newLaser = Instantiate(LaserPrefab, startPt, Quaternion.identity);
            newLaser.transform.SetParent(transform);
            newLaser.transform.LookAt(TargetGate.transform.position);
            newLaser.SetActive(true);
            AudioSource src = newLaser.GetComponent<AudioSource>();
            src.pitch = Random.Range(0.9f, 1.1f);
            Destroy(newLaser, 5f);
            if (LaserImpact != null)
            {
                GameObject NewImpact = Instantiate(LaserImpact, TargetGate.transform.position, Quaternion.identity);
                NewImpact.SetActive(true);
                Destroy(NewImpact, 5f);
            }
        }
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            TargetGate.GetComponent<Health>().takeDamage(LaserDamage);
    }
}
