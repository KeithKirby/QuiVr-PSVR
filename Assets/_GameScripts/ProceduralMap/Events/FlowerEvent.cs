using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerEvent : EventTile {

    public GameObject StartIndicator;
    public Flower flower;
    public WormAnimations FlowerAnim;
    public ParticleSystem AcidTop;
    public ParticleSystem AcidBottom;
    public Transform SpinWall;
    public Transform SpinWallHard;
    public Transform SpinWallHard2;
    public EventRoots[] Roots;
    [HideInInspector]
    public bool damaged;
    [HideInInspector]
    public bool phaseComplete;
    float cloudMoveSpeed;
    bool introComplete;
    Teleporter[] AllTPs;

    public override void Awake()
    {
        AllTPs = GetComponentsInChildren<Teleporter>();
        base.Awake();
    }

    [AdvancedInspector.Inspect]
    public override void StartIntro()
    {
        if (!startedIntro)
        {
            StartCoroutine("Intro");
            topLayer = AcidTop.transform.position;
            bottomLayer = AcidBottom.transform.position;
            //Setup Difficulty based on Game Difficulty
            if (TargetGate != null)
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
                if (PhotonNetwork.inRoom && PhotonNetwork.playerList.Length > 1)
                {
                    int numPlr = PhotonNetwork.playerList.Length;
                    if (numPlr == 2) NumAllowed += 3;
                    else if (numPlr == 3) NumAllowed += 6;
                    else if (numPlr == 4) NumAllowed += 7;
                }
                PlayerDeathDamage = (int)(gateHP / (float)NumAllowed);

                //Small Rock values
                cloudMoveSpeed = 1f;
                if (curDiff > 2500)
                    cloudMoveSpeed = 2f;
                else if (curDiff > 1500)
                    cloudMoveSpeed = 1.5f;
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
        flower.SetupSpears(Rand);
        Debug.Log("Starting Event: Flower Boss");
    }

    IEnumerator Intro()
    {
        foreach(var v in AllTPs)
        {
            if (v != StartTP)
                v.ForceDisable(true);
        }
        while (!introComplete)
        {   //Wait for hit
            yield return true;
        }
        StartEvent();
    }

    #region Boss Phases
    IEnumerator BossFight()
    {
        yield return Phase0();
        StartCoroutine("RootLoop");
        while (!checkDead())
        {
            yield return Spiral();
            yield return AcidLayers();
        }
        StopCoroutine("RootLoop");
        foreach(var v in Roots)
        {
            v.Deactivate();
        }
        flower.PlayAnimation("Death");
        flower.PlayClip("Death");
        flower.GetComponent<AudioSource>().Stop();
        yield return new WaitForSeconds(5f);
        //flower.Dissolve();
        //flower.gameObject.SetActive(false);
        EndEvent();
    }

    IEnumerator Phase0()
    {
        phaseComplete = false;
        TryStartMusic();
        foreach (var v in AllTPs)
        {
            v.ForceDisable(false);
            v.EnableTeleport();
        }
        flower.PlayAnimation("ComeAlive");
        flower.PlayClip("ComeAlive");
        flower.GetComponent<AudioSource>().Play();
        while (flower.isAnimPlaying())
            yield return true;
        flower.PlayAnimation("IdleAttacking");     
    }

    Vector3 topLayer;
    Vector3 bottomLayer;
    IEnumerator AcidLayers()
    {
        damaged = false;
        Teleporter rtp = AllTPs[GetRandomNext(0, AllTPs.Length)];
        flower.PlayAnimation("MagicStartCast");
        while (flower.isAnimPlaying())
            yield return true;
        flower.PlayAnimation("MagicLoop");
        SetupAcidLayers();
        Vector3 wantTop = topLayer;
        wantTop.y = rtp.transform.position.y + 15f;
        Vector3 wantBottom = bottomLayer;
        wantBottom.y = rtp.Positions[0].pos.transform.position.y - 8f;
        float dur = 20f;
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime / dur;
            AcidTop.transform.position = Vector3.Lerp(topLayer, wantTop, t);
            AcidBottom.transform.position = Vector3.Lerp(bottomLayer, wantBottom, t);
            yield return true;
            foreach(var v in AllTPs)
            {
                if (v.transform.position.y > AcidTop.transform.position.y)
                    v.ForceDisable(true);
                else if (v.transform.position.y + 3f < AcidBottom.transform.position.y)
                    v.ForceDisable(true);
            }
        }
        flower.canDamage = true;
        flower.ActivateRandomSpear(Rand);
        while(!damaged)
        {
            yield return true;
        }
        DisableAcidLayers();
        foreach(var v in AllTPs)
        {
            v.ForceDisable(false);
            if(TelePlayer.instance.currentNode != v)
                v.EnableTeleport();
        }
        flower.PlayAnimation("TakeDamage");
        flower.PlayClip("TakeDamage");
        while (flower.isAnimPlaying())
            yield return true;
        flower.PlayAnimation("IdleAttacking");
    }

    IEnumerator Spiral()
    {
        flower.PlayAnimation("EnterSpin");
        while (flower.isAnimPlaying())
            yield return true;
        flower.LayerSpawn.Play();
        flower.Spiral.Play();
        flower.PlayAnimation("SpinLoop");
        Transform wall = SpinWall;
        if (GameBase.instance.Difficulty > 3000)
            wall = SpinWallHard2;
        else if (GameBase.instance.Difficulty > 1800)
            wall = SpinWallHard;
        wall.localEulerAngles = new Vector3(0, 90f, 90f);
        if (wall.GetComponent<ParticleSystem>() != null)
            wall.GetComponent<ParticleSystem>().Play();
        else
            wall.GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(2f);
        if(wall.GetComponent<BoxCollider>() != null)
            wall.GetComponent<BoxCollider>().enabled = true;
        foreach(var v in wall.GetComponentsInChildren<BoxCollider>()) { v.enabled = true; }
        wall.GetComponent<AudioSource>().Play();

        float t = 0;
        float dur = 55f;
        if (GameBase.instance.Difficulty > 3000)
            dur = 35f;
        else if (GameBase.instance.Difficulty > 2000)
            dur = 45f;
        while (t < 1)
        {
            t += Time.deltaTime / dur;
            SpinWall.transform.localEulerAngles = new Vector3(0, 90 + (360f * t), 90f);
            yield return true;
        }

        if (wall.GetComponent<ParticleSystem>() != null)
            wall.GetComponent<ParticleSystem>().Stop();
        else
            wall.GetComponentInChildren<ParticleSystem>().Stop();
        if (wall.GetComponent<BoxCollider>() != null)
            wall.GetComponent<BoxCollider>().enabled = false;
        foreach (var v in wall.GetComponentsInChildren<BoxCollider>()) { v.enabled = true; }
        wall.GetComponent<AudioSource>().Stop();
        flower.LayerSpawn.Stop();
        flower.Spiral.Stop();
        flower.PlayAnimation("ExitSpin");
        while (flower.isAnimPlaying())
            yield return true;
        flower.PlayAnimation("IdleAttacking");
    }

    IEnumerator RootLoop()
    {
        float tbtw = 2f;
        if (GameBase.instance.Difficulty > 1000)
            tbtw = 1f;
        while(true)
        {
            yield return new WaitForSeconds(tbtw);
            SpawnRoot();
        }
    }
    #endregion

    bool checkDead()
    { 
        return flower.isDead();
    }

    void SetupAcidLayers()
    {
        flower.LayerSpawn.Play();
        AcidTop.transform.position = topLayer;
        AcidBottom.transform.position = bottomLayer;
        AcidTop.Play();
        AcidTop.GetComponent<Collider>().enabled = true;
        AcidBottom.Play();
        AcidBottom.GetComponent<Collider>().enabled = true;
        AcidTop.GetComponent<AudioSource>().Play();
        AcidTop.GetComponent<AudioSource>().time = Random.Range(0.3f, 1.25f);
        AcidBottom.GetComponent<AudioSource>().Play();
    }

    void DisableAcidLayers()
    {
        flower.LayerSpawn.Stop();
        //AcidTop.transform.position = topLayer;
        //AcidBottom.transform.position = bottomLayer;
        AcidTop.Stop();
        AcidTop.GetComponent<Collider>().enabled = false;
        AcidBottom.Stop();
        AcidBottom.GetComponent<Collider>().enabled = false;
        AcidTop.GetComponent<AudioSource>().Stop();
        AcidBottom.GetComponent<AudioSource>().Stop();
    }

    public void SpawnRoot()
    {
        List<Teleporter> Active = new List<Teleporter>();
        foreach (var v in AllTPs) { Active.Add(v); }
        foreach (var v in TeleporterManager.instance.Dynamics) { Active.Add(v.teleporter); }
        EventRoots unused = null;
        foreach(var v in Roots)
        {
            if (!v.active)
            {
                unused = v;
            }
            else
                Active.Remove(v.target);
        }
        if (unused == null)
            return;
        for(int i=Active.Count-1; i>=0; i--)
        {
            if (Active[i].Disabled || Active[i].ForceDisabled)
                Active.RemoveAt(i);
        }
        Teleporter selected = null;
        if(Active.Count > 0)
            selected = Active[GetRandomNext(0, Active.Count)];
        if(selected != null)
            unused.Activate(selected, 7f);
    }

    public override void OnActionTaken(string action)
    {
        if (action == "FLESH_HIT")
            EventManager.instance.IntEvent1(this, 1);
        else if (action == "SPEAR_HIT")
            EventManager.instance.IntEvent1(this, 1);
        base.OnActionTaken(action);
    }

    public override void IntEvent1Response(int val)
    {
        if (val == 1)
            introComplete = true;
    }

    public override void SkipEvent()
    {
        StopAllCoroutines();
        DisableAcidLayers();
        base.SkipEvent();
    }

}
