using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbEvent : EventTile
{
	public GOReference[] RockObjects;
    List<Teleporter> climbTPs;
    public Teleporter startCliffTP;
    public bool reachedTop;
    public GameObject BallThrower;
    public GameObject BallPrefab;
    public SequencePedestal seq;
    public Laserbeam[] Lasers;
    Transform[] LaserTargets;
    List<SimulatedLob> SmallLobs;
    int rocksPerThrow = 3;
    int numlasers;
    bool onCliff;

    List<Vector3> L1Pos;

    public override void Awake()
    {        
        LaserTargets = new Transform[Lasers.Length];
        for(int i=0; i<Lasers.Length; i++)
        {
            LaserTargets[i] = Lasers[i].Target;
        }
        L1Pos = new List<Vector3>();
        base.Awake();
    }

    public override void Update()
    {
        if(!reachedTop && EventManager.InEvent)
        {
            float minTPY = transform.position.y + 25f;
            foreach(var v in climbTPs)
            {
                if (v.transform.position.y > minTPY && !v.Disabled && !v.ForceDisabled)
                    minTPY = v.transform.position.y;
            }
            Vector3 p = BallThrower.transform.position;
            BallThrower.transform.position = Vector3.Lerp(BallThrower.transform.position, new Vector3(p.x, minTPY, p.z), Time.deltaTime);
        }     
        base.Update();
    }

    public override void StartIntro()
    {
        if (!startedIntro)
        {
            StartCoroutine("Intro");
            ArrowEffects.EffectsDisabled = true;
            int curDiff = (int)GameBase.instance.Difficulty;
            rocksPerThrow = 2;
            numlasers = 4;
            if (curDiff > 3000)
            {
                rocksPerThrow = 6;
                numlasers = 8;
            }            
            else if (curDiff > 2000)
            {
                rocksPerThrow = 5;
                numlasers = 7;
            }                
            else if (curDiff > 1000)
            {
                rocksPerThrow = 4;
                numlasers = 6;
            }                
            else if (curDiff > 500)
            {
                rocksPerThrow = 3;
                numlasers = 5;
            }               
            if (TargetGate != null)
            {
                //Damager per player death
                float gateHP = TargetGate.GetComponent<Health>().maxHP;
                int NumAllowed = 5;
                if (curDiff > 3000)
                    NumAllowed = 2;
                else if (curDiff > 2000)
                    NumAllowed = 3;
                else if (curDiff > 1000)
                    NumAllowed = 4;
                if (PhotonNetwork.inRoom && PhotonNetwork.playerList.Length > 1)
                {
                    int numPlr = PhotonNetwork.playerList.Length;
                    if (numPlr == 2) NumAllowed += 3;
                    else if (numPlr == 3) NumAllowed += 6;
                    else if (numPlr == 4) NumAllowed += 7;
                }
                PlayerDeathDamage = (int)(gateHP / (float)NumAllowed);
            }
            else
                PlayerDeathDamage = 25; //Change Based on Game Difficulty
            SetupTPs();
            base.StartIntro();
        }
    }

    void SetupTPs()
    {
        climbTPs = new List<Teleporter>();
        float chance = 33;
        if (GameBase.instance.Difficulty > 1000)
            chance = 66;
        else if (GameBase.instance.Difficulty > 1800)
            chance = 101;
        for (int i = 0; i < RockObjects.Length; i++)
        {
            Teleporter tp = RockObjects[i].GetComponentInChildren<Teleporter>();
            EventTile tile = this;
            if (tp != null)
            {
                climbTPs.Add(tp);
                GOReference gor = tp.GetComponentInParent<GOReference>();
                if(gor != null && gor.multRef.Length > 0)
                {
                    if(GetRandomNext(0, 100) < chance || tp == startCliffTP)
                    {
                        tp.OnTeleport.AddListener(delegate
                        {
                            seq.TeleportEventResponse(tp);
                            seq.Setup(Random.Range(0, int.MaxValue), rocksPerThrow);
                            seq.OnComplete.RemoveAllListeners();
                            seq.OnComplete.AddListener(delegate
                            {
                                EventManager.instance.Vec3Event2(tile, tp.transform.position);
                            });
                        });
                    }
                    else
                    {
                        tp.OnTeleport.AddListener(delegate
                        {
                            EventManager.instance.Vec3Event2(tile, tp.transform.position);
                        });
                    }
                }                               
            }
        }
    }

    [AdvancedInspector.Inspect]
    public override void StartEvent()
    {
        DeactivateAll();
        StartCoroutine("ClimbLoop");
        base.StartEvent();
        Debug.Log("Starting Event: Climbing Event");
    }

    public override void EndEvent()
    {
        ArrowEffects.EffectsDisabled = false;
        base.EndEvent();
    }

    IEnumerator Intro()
    {
        TryStartMusic();
        yield return true;
        StartEvent();
    }

    IEnumerator ClimbLoop()
    {
        //Turn Lasers On
        SetupLaserPositions();
        for(int i=0; i<numlasers; i++)
        {
            Lasers[i].ToggleLaser(true);
        }
        BallThrower.SetActive(true);
        while (!reachedTop)
        {
            yield return AttackRandom();
            yield return new WaitForSeconds(1f);
        }
        BallThrower.SetActive(false);
        foreach (var v in SmallLobs)
        {
            if (v != null)
                v.Break();
        }
        foreach (var v in Lasers)
        {
            v.ToggleLaser(false);
        }
        EndEvent();
    }

    IEnumerator AttackRandom()
    {
        yield return ThrowSmallRocks(rocksPerThrow, BallThrower.transform.position, startCliffTP);
        yield return true;
    }

    public override void OnActionTaken(string action)
    {
        if (action == "REACHED_TOP" && !reachedTop)
            EventManager.instance.IntEvent1(this, 1);
        base.OnActionTaken(action);
    }

    public override void IntEvent1Response(int val)
    {
        if(val == 1)
        {
            ReachedTop();
        }
        base.IntEvent1Response(val);
    }

    public override void Vec3Event2Response(Vector3 val)
    {
        int tpid = GetClosestTPID(val);
        if (tpid >= 0 && tpid < climbTPs.Count)
        {
            GOReference o = RockObjects[tpid].GetComponent<GOReference>();
            foreach (var v in o.multRef)
            {
                Teleporter rtp = v.GetComponentInChildren<Teleporter>();
                if (rtp != null && (rtp.Disabled || rtp.ForceDisabled))
                {
                    rtp.ForceDisable(false);
                    rtp.EnableTeleport();
                }
            }
        }
        base.Vec3Event2Response(val);
    }

    int GetClosestTPID(Vector3 pos)
    {
        float mindist = float.MaxValue;
        int tpid = -1;
        for(int i=0; i<climbTPs.Count; i++)
        {
            Teleporter tp = climbTPs[i];
            float d = Vector3.Distance(pos, tp.transform.position);
            if (d < mindist && d < 10f)
            {
                mindist = d;
                tpid = i;
            }
        }
        return tpid;
    }

    #region Attack Rocks

    IEnumerator ThrowSmallRocks(int num, Vector3 spawnLoc, Teleporter ignoreTP = null)
    {
        List<Teleporter> tps = new List<Teleporter>();
        int n = 0;
        int sanity = 0;
        while (n < num && sanity < 100)
        {
            Teleporter tp = climbTPs[GetRandomNext(0, climbTPs.Count)];
            if (!tps.Contains(tp) && tp != ignoreTP)
            {
                tps.Add(tp);
                n++;
            }
            sanity++;
        }

        foreach (var v in tps)
        {
            ThrowSmallRockAtTP(v, spawnLoc);
            yield return new WaitForSeconds(0.25f);
        }
    }
    void ThrowSmallRockAtTP(Teleporter tp, Vector3 SpawnLoc)
    {
        GameObject newRock = Instantiate(BallPrefab, SpawnLoc, Quaternion.identity);
        newRock.SetActive(true);
        newRock.GetComponent<RotateTimed>().Rotation = new Vector3(Random.Range(-60f, 60f), Random.Range(-60f, 60f), Random.Range(-60f, 60f));
        SimulatedLob lob = newRock.GetComponent<SimulatedLob>();
        CheckSmallLobs();
        SmallLobs.Add(lob);
        Vector3 target = tp.transform.position;
        if (tp.transform.parent != null)
            target = tp.transform.parent.position;
        lob.Launch(target);
        lob.OnLand.AddListener(delegate
        {
            if (tp != null && PlayerHead.instance != null)
            {
                float distance = Vector3.Distance(PlayerHead.instance.transform.position, tp.transform.position);
                float force = Mathf.Clamp((1 / distance) * 3f, 0, 1.5f);
                PlayerShake.Shake(force, 0.75f, false);
            }
        });
        lob.OnLand.AddListener(tp.KillIfUsing);
    }
    public void BreakRock(ArrowCollision imp)
    {
        EventManager.instance.Vec3Event1(this, imp.impactPos);
    }
    public override void Vec3Event1Response(Vector3 val)
    {
        CheckSmallLobs();
        if (SmallLobs != null && SmallLobs.Count > 0)
        {
            SimulatedLob closest = null;
            float minDist = float.MaxValue;
            foreach (var v in SmallLobs)
            {
                float d = Vector3.Distance(v.transform.position, val);
                if (d < minDist && d < 15)
                {
                    closest = v;
                    minDist = d;
                }
            }
            if (closest != null)
            {
                closest.Break();
            }
        }
    }
    void CheckSmallLobs()
    {
        if (SmallLobs == null)
            SmallLobs = new List<SimulatedLob>();
        for (int i = SmallLobs.Count - 1; i >= 0; i--)
        {
            if (SmallLobs[i] == null)
                SmallLobs.RemoveAt(i);
        }
    }

    #endregion

    #region Laser Methods

    AnimationCurve laserCurve;
    void SetupLaserPositions()
    {
        laserCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        Vector3 startPos = LaserTargets[0].transform.localPosition;
        for(int i=0; i<numlasers; i++)
        {
            L1Pos = new List<Vector3>();
            for (int j = 0; j < 10+i; j++)
            {
                Vector3 npt = startPos + new Vector3(GetRandomNext(0f, 99f), GetRandomNext(0f, 100f), 0);
                L1Pos.Add(npt);
            }
            LaserTargets[i].GetComponent<EventAcidCloud>().Activate(L1Pos.ToArray(), 4f);
        }    
    }

    #endregion

    public void ReachedTop()
    {
        reachedTop = true;
    }

    public void DeactivateAll()
    {
        foreach(var v in climbTPs)
        {
            v.ForceDisable(true);
            v.DisableTeleport();
        }
        startCliffTP.ForceDisable(false);
        startCliffTP.EnableTeleport();
    }
}
