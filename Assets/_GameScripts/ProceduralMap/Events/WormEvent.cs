using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormEvent : EventTile
{
    public Worm worm;
    public Transform[] emergeLocations;
    public Transform[] tpLocations;
    public Transform SpitOrigin;
    public GameObject StartIndicator;

    public ParticleSystem[] EnvEffects;
    public EventAcidCloud[] AcidClouds;
    public GameObject RockThrow;
    public GameObject SmallRockThrow;
    public GameObject AcidSpit;

    List<SimulatedLob> SmallLobs;
    int smallRocksPer;
    #region Phase Variables
    int wormLocation;
    bool tailHit;
    bool leftPoint;
    bool rightPoint;
    [HideInInspector]
    public bool damaged;
    [HideInInspector]
    public bool phaseComplete;
    #endregion

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void EventEndCleanup()
    {
        TeleporterManager.OnDynamicTPCreated -= ThrowRockAtTP;
        base.EventEndCleanup();
    }

    public override void StartIntro()
    {
        if (!startedIntro)
        {
            StartCoroutine("Intro");
            //Setup Difficulty based on Game Difficulty
            if(TargetGate != null)
            {
                int curDiff = (int)GameBase.instance.Difficulty;

                //Damager per player death
                float gateHP = TargetGate.GetComponent<Health>().maxHP;
                int NumAllowed = 5;
                if (curDiff > 3000)
                    NumAllowed = 2;
                else if (curDiff > 2000)
                    NumAllowed = 3;
                else if (curDiff > 1000)
                    NumAllowed = 4;
                if(PhotonNetwork.inRoom && PhotonNetwork.playerList.Length > 1)
                {
                    int numPlr = PhotonNetwork.playerList.Length;
                    if (numPlr == 2) NumAllowed += 3;
                    else if (numPlr == 3) NumAllowed += 6;
                    else if (numPlr == 4) NumAllowed += 7;
                }
                PlayerDeathDamage = (int)(gateHP / (float)NumAllowed);

                //Small Rock values
                smallRocksPer = 2;
                if (curDiff > 2500)
                    smallRocksPer = 4;
                else if(curDiff > 1500)
                    smallRocksPer = 3;
            }
            else
                PlayerDeathDamage = 25; //Change Based on Game Difficulty
            base.StartIntro();
        }
    }

    [AdvancedInspector.Inspect]
    public override void StartEvent()
    {
        StopCoroutine("Intro");
        StartCoroutine("BossFight");
        base.StartEvent();
        Debug.Log("Starting Event: Worm Boss");
    }

    public override void EndEvent()
    {
        base.EndEvent();
    }

    [AdvancedInspector.Inspect]
    public void TestDeath()
    {
        StopCoroutine("WormDie");
        StartCoroutine("WormDie");
    }

    IEnumerator Intro()
    {
        float t = 0;
        while (!phaseComplete)
        {
            // Wait for Tail hit
            t += Time.deltaTime;
            yield return true;
            if (t > 30 && !StartIndicator.activeSelf)
                StartIndicator.SetActive(true);
        }
        StartIndicator.SetActive(false);
        StartEvent();
    }

    #region Boss Phases
    IEnumerator BossFight()
    {
        yield return Phase0();
        yield return Phase1();
        yield return Phase2();
        yield return Phase3();
        yield return WormDie();
        yield return new WaitForSeconds(2.5f);
        worm.Dissolve();
        yield return new WaitForSeconds(2.5f);
        worm.gameObject.SetActive(false);
        EndEvent();
    }

    IEnumerator Phase0()
    {
        phaseComplete = false;
        TryStartMusic();
        worm.PlayClip("Damaged");
        worm.PlayAnimation("preSubmerge"); // Submerge
        while (worm.isAnimPlaying())
            yield return true;
        wormLocation = 0;
        MoveWorm(emergeLocations[0].position, Quaternion.LookRotation(emergeLocations[0].up));
        EnvEffects[0].Play(); // Rumble Effect
        TeleporterManager.OnDynamicTPCreated += ThrowRockAtTP;
        float wait = 0;
        while(wait < 10f)
        {
            PlayerShake.Shake(0.2f, 0.5f, false);
            wait += Time.deltaTime;
            yield return true;
        }
        phaseComplete = false;
        PlayerShake.Shake(5, 0.75f, true);
        worm.GetComponent<AudioSource>().Stop();
        worm.PlayAnimation("riseHigh"); //Erupt
        worm.RockExplosion();     
        while (worm.isAnimPlaying())
            yield return true;
        worm.PlayAnimation("roarHigh");
        float roarLength = worm.GetAnimLength("roarHigh");
        yield return new WaitForSeconds(roarLength * 0.25f);
        worm.PlayRoar();
        PlayerShake.Shake(1.25f, 5, true);
        yield return new WaitForSeconds(roarLength * 0.67f);
        worm.RoarSpittle.Stop();
        yield return new WaitForSeconds((roarLength*0.1f) - 0.5f);
        ActivateTeleporters();
    }

    IEnumerator Phase1()
    {
        Debug.Log("Worm Phase 1");
        worm.PlayAnimation("idleLow"); // Fade animation to Low Height
        yield return ThrowRocks(1);
        yield return new WaitForSeconds(1f);
        for(int i=0; i<3; i++)         // Spit/Bite x3 
        {
            yield return AttackRandom(1);
            worm.PlayAnimation("idleLow");
        }
        yield return Vulnerable(1);    // Do Glow (first set)
        ActivateTeleporters();
        worm.PlayAnimation("submergeLow");
        PlayerShake.Shake(0.8f, 1f, false);
        while (worm.isAnimPlaying())
            yield return true;
        worm.GetComponent<AudioSource>().Stop();
        worm.EnsurePhaseEnd(1);
        yield return SyncRandom();
    }

    IEnumerator Phase2()
    {
        Debug.Log("Worm Phase 2");
        for (int i = 0; i < 3; i++)
        {
            int targetID = GetRandomTarget(0);
            yield return EmergeAttackRandom(targetID);
            yield return new WaitForSeconds(1.5f);
        }
        wormLocation = GetRandomWormLocation(2); // Emerge Medium Height at random location (not center)
        MoveWorm(emergeLocations[wormLocation].position, Quaternion.LookRotation(emergeLocations[wormLocation].up));
        EnvEffects[0].Play(); // Rumble Effect
        float wait = 0;
        while (wait < 5f)
        {
            PlayerShake.Shake(1, 0.5f, false);
            wait += Time.deltaTime;
            yield return true;
        }
        PlayerShake.Shake(4, 0.75f, true);
        worm.GetComponent<AudioSource>().Play();
        worm.PlayAnimation("riseMid"); //Erupt
        worm.RockExplosion();
        yield return ThrowRocks(2); // Throw rocks to disable Nearest and Furthest TPs + X random (based on difficulty)
        while (worm.isAnimPlaying())
            yield return true;
        worm.PlayAnimation("idleMid");        
        if (!cloudsOn)
            SpawnClouds();  // Spawn Acid Clouds (number based on difficulty + multiplayer)
        for (int i = 0; i < 3; i++)         
        {
            yield return AttackRandom(2); // Spit/Bite x3
            worm.PlayAnimation("idleMid");
            //yield return new WaitForSeconds(3f);
        }
        yield return Vulnerable(2); // Do Glow (second set)
        ActivateTeleporters();
        worm.PlayAnimation("submergeMid");
        PlayerShake.Shake(0.8f, 1.5f, false);
        while (worm.isAnimPlaying())
            yield return true;
        worm.GetComponent<AudioSource>().Stop();
        worm.EnsurePhaseEnd(2);
        yield return SyncRandom();
    }

    IEnumerator Phase3()
    {
        Debug.Log("Worm Phase 3");
        for (int i = 0; i < 3; i++)
        {
            int targetID = GetRandomTarget(0);
            yield return EmergeAttackRandom(targetID);
            yield return new WaitForSeconds(1.5f);
        }
        wormLocation = GetRandomWormLocation(3);// Emerge Full Height at random location
        MoveWorm(emergeLocations[wormLocation].position, Quaternion.LookRotation(emergeLocations[wormLocation].up));
        EnvEffects[0].Play(); // Rumble Effect
        float wait = 0;
        while (wait < 5f)
        {
            PlayerShake.Shake(1, 0.5f, false);
            wait += Time.deltaTime;
            yield return true;
        }
        worm.GetComponent<AudioSource>().Play();
        PlayerShake.Shake(4, 0.75f, true);
        worm.PlayAnimation("riseHigh"); //Erupt
        worm.RockExplosion();
        yield return ThrowRocks(3); // Throw rocks to disable Nearest and Furthest TPs + X random (based on difficulty)
        while (worm.isAnimPlaying())
            yield return true;
        worm.PlayAnimation("idleHigh");
        // Begin Acid Rain
        for (int i = 0; i < 3; i++)
        {
            yield return AttackRandom(3); // Spit/Bite x3
            worm.PlayAnimation("idleHigh");
        }
        yield return Vulnerable(3); // Do Glow (third set)
        worm.EnsurePhaseEnd(3);
    }

    IEnumerator WormDie()
    {
        ActivateTeleporters();
        DespawnClouds();
        CheckSmallLobs();
        foreach(var v in SmallLobs)
        {
            v.Break();
        }
        foreach(var v in worm.WeakPoints)
        {
            v.gameObject.SetActive(false);
        }
        worm.PlayAnimation("death"); // Play Death animation
        worm.PlayClip("Death");
        PlayerShake.Shake(2.5f, 1, true);
        EnvEffects[0].Stop(); // Stop Rumble Effect
        TeleporterManager.OnDynamicTPCreated -= ThrowRockAtTP;
        float t = 0;
        float rot = 0;
        float len = worm.GetAnimLength("death")/worm.wanim.GetAnimSpeed("death");
        Quaternion startRot = worm.transform.rotation;
        Quaternion wantRot = startRot * Quaternion.Euler(Vector3.up * 15f);
        while (t < len*0.684f)
        {
            if (wormLocation != -1) // Rotate worm 15 degrees over animation [if center?]
            {
                worm.transform.rotation = Quaternion.Slerp(startRot, wantRot, worm.RotateCurve.Evaluate(rot));
            }
            rot += Time.deltaTime * 0.5f;
            t += Time.deltaTime;
            yield return true;
        }
        //Starts Fall
        while(t < len*(0.684f + 0.123f))
        {
            t += Time.deltaTime;
            yield return true;
        }
        //First Impact with Ground
        EnvEffects[3].Play();
        EnvEffects[3].GetComponent<AudioSource>().Play();
        PlayerShake.Shake(2f, 1f, true);
        while (t < len*(0.684f+0.123f+0.032f))
        {
            t += Time.deltaTime;
            yield return true;
        }
        //Mouth Impact
        EnvEffects[4].Play();
        EnvEffects[4].GetComponent<AudioSource>().Play();
        PlayerShake.Shake(2f, 1f, true);
        while (t < len*(0.684f + 0.123f + 0.032f + 0.116f))
        {
            t += Time.deltaTime;
            yield return true;
        }
        //Mouth Bounce End
        PlayerShake.Shake(1f, 0.6f, true);
        while (worm.isAnimPlaying())
            yield return true;
        worm.GetComponent<AudioSource>().Stop();
    }

    #endregion

    #region Worm Actions

    IEnumerator ThrowRocks(int phase)
    {
        yield return true;
        int numRocks = phase+1;
        List<Teleporter> tps = new List<Teleporter>();
        int minSub = 0;
        if(Vector3.Distance(worm.transform.position, emergeLocations[0].position) > 5)
        {
            minSub = 1;
            numRocks -= 1;
            tps.Add(ClosestTP());
        }

        //Set num based on difficulty

        //Get Extra Random TPS
        numRocks = Mathf.Min(numRocks, 5-minSub);
        int n = 0;
        int sanity = 0;
        while(n < numRocks && sanity < 100)
        {
            int locID = GetRandomNext(0, tpLocations.Length);
            Transform t = tpLocations[locID];
            Teleporter tp = GetTP(t);
            if(!tps.Contains(tp) && tp != KillTP && (phase != 1 || locID != 3))
            {
                tps.Add(tp);
                n++;
            }
            sanity++;
        }

        //Launch Rocks at Selected TPs
        foreach(var v in tps)
        {
            ThrowRockAtTP(v);
        }
    }

    IEnumerator AttackRandom(int phase)
    {
        yield return true;
        int targetID = GetRandomTarget(wormLocation);
        bool farLoc = isFarLocation(wormLocation, targetID, phase);
        if (farLoc)
            yield return Spit(targetID, phase);
        else
            yield return Bite(targetID, phase);
    }

    IEnumerator Bite(int targetID, int phase)
    {
        Transform targ = tpLocations[targetID]; //Rotate towards target
        yield return RotateTowards(targetID);
        string animPost = new string[] {"Low","Mid","High" }[phase-1];
        if (!damaged)
        {
            worm.PlayAnimation("bitePre" + animPost); //Rear Back
            worm.PlayClip("AttackPre", 0.8f);
        }
        while (worm.isAnimPlaying() && !damaged)
        {
            yield return true;
        }
        if (!damaged)
            worm.PlayAnimation("bite" + animPost); //Do Bite anim
        float len = worm.GetAnimLength("bite" + animPost);
        float t = 0;
        while(t < len*0.2f && !damaged)
        {
            t += Time.deltaTime;
            yield return true;
        }
        if(!damaged)
        {
            worm.PlayClip("BiteExplode");
            Teleporter tp = GetTP(targ);

            float distance = Vector3.Distance(PlayerHead.instance.transform.position, tp.transform.position);
            float force = Mathf.Clamp((1 / distance) * 30f, 0, 8f);
            PlayerShake.Shake(force, 1, false);

            if (tp != null)
                tp.KillIfUsing();

            ThrowSmallRocks(3, tp.transform.position, tp);
        }
        while (t < len && !damaged)
        {
            t += Time.deltaTime;
            yield return true;
        }
        if (!damaged)
            worm.PlayAnimation("idle" + animPost);
    }

    IEnumerator Spit(int targetID, int phase)
    {
        Transform targ = tpLocations[targetID]; //Rotate towards target
        yield return RotateTowards(targetID);        
        string animPost = new string[] { "Low", "Mid", "High" }[phase-1];
        if (!damaged)
        {
            worm.PlayAnimation("spitPre" + animPost); //Rear Back
            worm.PlayClip("AttackPre", 0.8f);
        }
        while (worm.isAnimPlaying() && !damaged)
            yield return true;
        if(!damaged)
        {
            worm.PlayAnimation("spit" + animPost); //Do Spit anim
        }
        float t = worm.GetAnimLength("spit" + animPost);
        float i = 0;
        while(i < t*0.4f && !damaged)
        {
            i += Time.deltaTime;
            yield return true;
        }
        
        //Spawn and shoot Glob
        if(!damaged)
        {
            worm.PlayClip("AcidSpit");
            PlayerShake.Shake(1, 0.5f, false);
            Teleporter tp = GetTP(targ);
            GameObject spit = Instantiate(AcidSpit, SpitOrigin.position, Quaternion.identity);
            spit.SetActive(true);
            SimulatedLob lob = spit.GetComponent<SimulatedLob>();
            if (tp != null)
            {
                lob.Launch(tp.transform.parent.position);
                lob.OnLand.AddListener(tp.KillIfUsing);
                /*
                lob.OnLand.AddListener(delegate {
                    ThrowSmallRocks(2, tp.transform.position, tp);
                });
                */
            }
        }
        while (worm.isAnimPlaying() && !damaged)
            yield return true;
        if(!damaged)
            worm.PlayAnimation("idle" + animPost); //Return to idle anim
    }

    IEnumerator RotateTowards(int targetID)
    {
        Transform targ = tpLocations[targetID]; //Rotate towards target
        Teleporter tp = GetTP(targ);
        if (GameBase.instance.Difficulty >= 2000)
            ThrowSmallRocks(smallRocksPer, worm.transform.position, tp);
        Quaternion startRot = worm.transform.rotation;
        Vector3 targetDir = (targ.position-worm.transform.position).normalized;
        Quaternion targRot = Quaternion.LookRotation(targetDir);
        float t = 0;
        float at = 0;
        bool midThrow = false;
        while (!damaged && t < 1)
        {
            t += Time.deltaTime * 0.25f;
            if (GameBase.instance.Difficulty >= 3000 && t > 0.5f & !midThrow)
            {
                midThrow = true;
                ThrowSmallRocks(smallRocksPer, worm.transform.position, tp);
            }
            worm.transform.rotation = Quaternion.Slerp(startRot, targRot, worm.RotateCurve.Evaluate(t));
            yield return true;
        }
        ThrowSmallRocks(smallRocksPer, worm.transform.position, tp);
    }

    IEnumerator EmergeAttackRandom(int targetID)
    {
        MoveWorm(tpLocations[targetID].position, Quaternion.LookRotation(tpLocations[targetID].forward));
        EnvEffects[0].Play(); // Rumble Effect
        Teleporter tp = GetTP(tpLocations[targetID]);
        if(GameBase.instance.Difficulty > 500)
            ThrowSmallRocks(smallRocksPer+1, worm.transform.position, tp);
        float wait = 0; float distance = 0; float force = 0;
        while (wait < 5f) //Change time based on difficulty?
        {
            distance = Vector3.Distance(PlayerHead.instance.transform.position, tp.transform.position);
            force = Mathf.Clamp((1 / distance) * 40f, 0, 1f);
            if(tp == TelePlayer.instance.currentNode)
                PlayerShake.Shake(force, 0.5f, false);
            wait += Time.deltaTime;
            yield return true;
        }
        worm.PlayAnimation("underBite"); // Burst Animation
        worm.PlayClip("Bite");
        float t = worm.GetAnimLength("underBite");
        yield return new WaitForSeconds(t*0.5f);
        distance = Vector3.Distance(PlayerHead.instance.transform.position, tp.transform.position);
        force = Mathf.Clamp((1 / distance) * 25f, 0, 8f);
        PlayerShake.Shake(force, 1, true);
        ThrowSmallRocks(smallRocksPer, worm.transform.position, tp);
        EnvEffects[2].Play(); //Burst Effect Single
        EnvEffects[2].GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(t*0.5f);
        EnvEffects[0].Stop();
        tp.KillIfUsing();
        while (worm.isAnimPlaying())
        {
            distance = Vector3.Distance(PlayerHead.instance.transform.position, tp.transform.position);
            force = Mathf.Clamp((1 / distance) * 40f, 0, 1f);
            PlayerShake.Shake(force, 0.3f, false);
            yield return true;
        }
    }

    IEnumerator Vulnerable(int phase)
    {
        Debug.Log("Worm Vulnerability (Phase " + phase + ")");
        worm.ActivateWeakPoint(phase);
        worm.canDamage = true;
        while (!phaseComplete) // While glow not hit -> Continue spitting every X seconds
        {
            yield return AttackRandom(phase); 
            if (damaged)
                yield return Damaged(phase); // Damage animation after each hit, glow disabled during anim
        }
        phaseComplete = false;
    }

    IEnumerator Damaged(int phase)
    {
        damaged = false;
        string animPost = new string[] { "Low", "Mid", "High" }[phase - 1];
        worm.PlayAnimation("damage" + animPost);         //Do damage animation
        worm.PlayClip("Damaged");
        PlayerShake.Shake(1.3f, 2f, false);
        ThrowSmallRocks(3, worm.transform.position);
        float t = worm.GetAnimLength("damage" + animPost);
        yield return new WaitForSeconds(t * 0.8f);
        damaged = false;
        worm.PlayAnimation("idle" + animPost);
        yield return new WaitForSeconds(1.5f);
        worm.ActivateRemainingPoints();
    }

    void ThrowRockAtTP(Teleporter tp)
    {
        GameObject newRock = Instantiate(RockThrow, worm.transform.position, Quaternion.identity);
        newRock.SetActive(true);
        newRock.GetComponent<RotateTimed>().Rotation = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f));
        SimulatedLob lob = newRock.GetComponent<SimulatedLob>();
        Vector3 target = tp.transform.position;
        if (tp.transform.parent != null)
            target = tp.transform.parent.position;
        lob.Launch(target);
        lob.OnLand.AddListener(delegate
        {
            if(tp != null && PlayerHead.instance != null)
            {
                float distance = Vector3.Distance(PlayerHead.instance.transform.position, tp.transform.position);
                float force = Mathf.Clamp((1 / distance) * 30f, 0, 8f);
                PlayerShake.Shake(force, 1f, false);
            }
        });
        lob.OnLand.AddListener(tp.KillIfUsing);
        lob.OnLand.AddListener(tp.DisableTeleport);        
    }

    void ThrowSmallRocks(int num, Vector3 spawnLoc, Teleporter ignoreTP = null)
    {
        List<Teleporter> tps = new List<Teleporter>();
        int n = 0;
        int sanity = 0;
        while (n < num && sanity < 100)
        {
            Transform t = tpLocations[GetRandomNext(0, tpLocations.Length)];
            Teleporter tp = GetTP(t);
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
        }
    }

    void ThrowSmallRockAtTP(Teleporter tp, Vector3 SpawnLoc)
    {
        GameObject newRock = Instantiate(SmallRockThrow, SpawnLoc, Quaternion.identity);
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
                float force = Mathf.Clamp((1 / distance) * 10f, 0, 1.5f);
                PlayerShake.Shake(force, 0.75f, false);
            }
        });
        lob.OnLand.AddListener(tp.KillIfUsing);
    }

    //Acid Clouds
    bool cloudsOn;
    void SpawnClouds()
    {
        cloudsOn = true;
        /*
        int cloudnum = 2;
        float cloudSpeed = 2f;
        if (GameBase.instance.Difficulty > 2000)
        {
            cloudnum += 2;
            cloudSpeed += 1f;
        }
        else if (GameBase.instance.Difficulty > 1000)
        {
            cloudnum += 1;
            cloudSpeed += 1f;
        }
        else if (GameBase.instance.Difficulty > 500)
            cloudnum += 1;
        if (PhotonNetwork.inRoom)
            cloudnum += PhotonNetwork.playerList.Length - 1;
        cloudnum = Mathf.Min(cloudnum, AcidClouds.Length);
        for(int i=0; i<cloudnum; i++)
        {
            List<Vector3> pts = new List<Vector3>();
            float y = AcidClouds[i].transform.position.y;
            Vector3 lastPt = new Vector3(worm.transform.position.x, y, worm.transform.position.z);
            pts.Add(lastPt);
            int lastPtID = -1;
            for (int k = 0; k < 12; k++)
            {
                int ptID = GetRandomNext(0, tpLocations.Length);
                if ((ptID == 3 && lastPtID == 0) || (ptID == 0 && lastPtID == 3) || (lastPtID == -1 && ptID == 0) || ptID == 4) { }
                else
                {
                    Vector3 nxt = tpLocations[ptID].position;
                    nxt.y = y;
                    Vector3 dir = (nxt - lastPt).normalized;
                    nxt += dir * 10f;
                    lastPt = new Vector3(nxt.x, y, nxt.z);
                    pts.Add(lastPt);
                    lastPtID = ptID;
                }               
            }
            AcidClouds[i].Activate(pts.ToArray(), cloudSpeed);
        }
        */
    }

    void DespawnClouds()
    {
        foreach(var v in AcidClouds)
        {
            v.Deactivate();
        }
        cloudsOn = false;
    }

    //Acid Rain (Phase 3)
    #endregion

    public override void OnActionTaken(string action)
    {
        if (action == "TAIL_HIT")
            EventManager.instance.IntEvent1(this, 3);
        else if (action == "LEFT_POINT")
            EventManager.instance.IntEvent1(this, 1);
        else if (action == "RIGHT_POINT")
            EventManager.instance.IntEvent1(this, 2);
        base.OnActionTaken(action);
    }

    public void BreakRock(ArrowCollision imp)
    {
        EventManager.instance.Vec3Event1(this, imp.impactPos);
    }

    #region Worm Utilities

    void ActivateTeleporters()
    {
        foreach (var v in tpLocations)
        {
            Teleporter t = GetTP(v);
            t.ForceDisable(false);
            t.EnableTeleport();
        }
    }

    public int GetRandomTarget(int locationID)
    {
        List<int> opts = new List<int>();
        for (int i = 0; i < tpLocations.Length; i++)
            opts.Add(i);
        //Remove Invalids
        if (locationID == 1) { opts.Remove(1); } //Back Loc
        else if (locationID == 2) { opts.Remove(2); } //Left Loc
        else if (locationID == 3) { opts.Remove(0); } //Right Loc
        else if (locationID == 4) { opts.Remove(3); } //Top Loc

        //Remove disabled
        for (int i = opts.Count - 1; i >= 0; i--)
        {
            Teleporter tp = GetTP(tpLocations[opts[i]]);
            if (tp != null && (tp.Disabled || tp.ForceDisabled) && opts.Count > 1)
                opts.RemoveAt(i);
        }

        //Remove unused
        for (int i=opts.Count-1; i>=0; i--)
        {
            Teleporter tp = GetTP(tpLocations[opts[i]]);
            if (tp != null && !tp.hasPlayers() && opts.Count > 1)
                opts.RemoveAt(i);
        }

        return opts[GetRandomNext(0, opts.Count)];
    }

    bool isFarLocation(int wloc, int tloc, int phase)
    {
        if (wloc == 0 && phase == 1)
        {
            if (tloc == 1 || tloc == 3)
                return true;
            else
                return false;
        }
        else if (wloc == 1 && (tloc == 5 || tloc == 7))
            return false;
        else if (wloc == 2 && (tloc == 6 || tloc == 7))
            return false;
        else if (wloc == 3 && (tloc == 5 || tloc == 4))
            return false;
        else if (wloc == 4 && (tloc == 4 || tloc == 6))
            return false;
        return true;
    }

    public int GetRandomWormLocation(int phase)
    {
        List<int> locs = new List<int>();
        for (int i = 0; i < emergeLocations.Length; i++)
            locs.Add(i);
        if (phase == 2)
            locs.RemoveAt(0);
        return locs[GetRandomNext(0, locs.Count)];
    }

    public void MoveWorm(Vector3 pos, Quaternion rot)
    {
        worm.Move(pos, rot);
    }

    public Teleporter GetTP(Transform tpLoc)
    {
        return tpLoc.GetComponent<GOReference>().goref.GetComponentInChildren<Teleporter>();
    }

    Teleporter FarthestTP()
    {
        float max = 0;
        Teleporter t = GetTP(tpLocations[0]);
        foreach(var v in tpLocations)
        {
            float d = Vector3.Distance(v.position, worm.transform.position);
            if(d > max)
            {
                max = d;
                t = GetTP(v);
            }
        }
        return t;
    }

    Teleporter ClosestTP()
    {
        float min = float.MaxValue;
        Teleporter t = GetTP(tpLocations[0]);
        foreach (var v in tpLocations)
        {
            float d = Vector3.Distance(v.position, worm.transform.position);
            if (d < min)
            {
                min = d;
                t = GetTP(v);
            }
        }
        return t;
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

    #region Network Methods

    public override void Vec3Event1Response(Vector3 val)
    {
        CheckSmallLobs();
        if(SmallLobs != null && SmallLobs.Count > 0)
        {
            SimulatedLob closest = null;
            float minDist = float.MaxValue;
            foreach(var v in SmallLobs)
            {
                float d = Vector3.Distance(v.transform.position, val);
                if(d < minDist && d < 15)
                {
                    closest = v;
                    minDist = d;
                }
            }
            if(closest != null)
            {
                closest.Break();
            }
        }
    }

    public override void IntEvent1Response(int val)
    {
        if (val == 1)
            worm.BreakPoint(0);
        else if (val == 2)
            worm.BreakPoint(1);
        else if (val == 3)
            phaseComplete = true;
    }

    //Change worm position
    public override void QVEvent1Response(Vector3 val, Quaternion rot)
    {
        worm.Move(val, rot);
    }

    #endregion

    public string DebugAction;

    [AdvancedInspector.Inspect]
    public void DoDebugAction()
    {
        OnActionTaken(DebugAction);
    }

    public override void OnDestroy()
    {
        TeleporterManager.OnDynamicTPCreated -= ThrowRockAtTP;
        base.OnDestroy();
    }
}
