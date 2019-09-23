using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class FireballCreature : GroundCreature {

    [Header("Summon Fireball")]
    int MinDistance = 10;
    public float SummonDistance;
    public float ThrowCD;
    public float SummonTime;
    bool Summoning;
    public ParticleSystem[] HandsSummoning;
    public ParticleSystem[] ExtraEffects;
    public Transform SummonOrb;
    public GameObject Shield;

    public UnityEvent OnSummonStart;

    [Header("Explode In Hand")]
    public ParticleSystem HandExplode;
    public AudioSource HandExplodeAudio;
    public AudioClip[] MadClips;

    public UnityEvent OnSummonEnd;
    Vector3 startPt;
    BallLauncher launcher;
    public override void Awake()
    {
        launcher = GetComponent<BallLauncher>();
        launcher.Damage = (int)Damage;
        SummonOrbMaxSize = SummonOrb.localScale;
        SummonDistance = Random.Range(SummonDistance - 10f, SummonDistance + 10f);
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        startPt = transform.position;
    }

    public override void Update()
    {
        if (Target != null)
        {
            if (!Summoning && agent.isOnNavMesh && Vector3.Distance(transform.position, Target.transform.position) < SummonDistance && Vector3.Distance(transform.position, startPt) > MinDistance && TimeAlive > 5f)
            {
                if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
                    EnterSummon(transform.position);
            }
            else if(Vector3.Distance(transform.position, Target.transform.position) > SummonDistance + 15)
            {
                if(Summoning)
                {
                    if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
                        ExitSummon();
                }
            }
        }
        base.Update();
    }

    [AdvancedInspector.Inspect]
    void EnterSummon()
    {
        EnterSummon(transform.position);
    }

    public void EnterSummon(Vector3 targMove)
    {
        if(!Summoning && gameObject.activeSelf)
        {
            Summoning = true;
            MovementPaused = true;
            Anims().overrideAnimations = true;
            if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            {
                Vector3 move = transform.position + new Vector3(Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f));
                NavMeshHit hit;
                if (NavMesh.SamplePosition(move, out hit, 5.0f, NavMesh.AllAreas))
                    targMove = hit.position;
            }
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                GetComponent<NetworkCreature>().ToggleSummoning(true, targMove);
            StopCoroutine("SummonLoop");
            StartCoroutine("SummonLoop", targMove);
        }       
    }

    [AdvancedInspector.Inspect]
    public void ExitSummon()
    {
        if(Summoning && gameObject.activeSelf)
        {
            StopCoroutine("SummonLoop");
            Summoning = false;
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                GetComponent<NetworkCreature>().ToggleSummoning(false, transform.position);
            MovementPaused = false;
            Anims().overrideAnimations = false;
            SetState(EnemyState.walking);
            if (agent.isOnNavMesh)
                agent.isStopped = false;
            Move();
            Movement.Resume();
            foreach (var v in HandsSummoning)
                v.Stop();
            SummonOrb.gameObject.SetActive(false);
            Shield.SetActive(false);
        }       
    }

    Vector3 SummonOrbMaxSize;
    IEnumerator SummonLoop(Vector3 targMove)
    {
        AudioSource summonAudio = SummonOrb.GetComponent<AudioSource>();
        Movement.Pause();
        if(targMove != Vector3.zero && agent.isOnNavMesh)
        {
            agent.SetDestination(targMove);
            agent.isStopped = false;
            float wt = 0;
            while (agent.pathPending && wt < 5)
            {
                wt += Time.deltaTime;
                yield return true;
            }
            while (agent.remainingDistance > 1f && wt < 5)
            {
                wt += Time.deltaTime;
                yield return true;
            }
        }       
        if(agent.isOnNavMesh)    
            agent.isStopped = true;
        Vector3 direction = (Target.transform.position - transform.position).normalized;
        if(tempTarg != null)
            direction = (tempTarg.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        foreach (var v in HandsSummoning)
        {
            v.Pause();
        }        
        OnSummonStart.Invoke();
        if (PhotonNetwork.inRoom && EnemyStream.GetRealPlayerNum() > 1)
        {
            foreach (var v in ExtraEffects)
                v.Play();
            Invoke("ShieldOn", 0.5f);
        }
        while (true)
        {        
            Anims().PlayIdle();
            float t = 0;
            while (t < ThrowCD)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4f);
                t += Time.deltaTime;
                yield return true;
            }
            foreach (var v in HandsSummoning)
                v.Play();
            Anims().PlayAnimation("SummoningLoop", 0.3f, 0.5f);
            t = 0;
            if(!PhotonNetwork.inRoom || EnemyStream.GetRealPlayerNum() < 2)
            {
                foreach (var v in ExtraEffects)
                    v.Play();
                Invoke("ShieldOn", 0.5f);
            }           
            SummonOrb.localScale = Vector3.one * 0.001f;
            SummonOrb.localRotation = Quaternion.identity;
            SummonOrb.gameObject.SetActive(true);
            summonAudio.time = 0;
            summonAudio.pitch = Random.Range(0.9f, 1.1f);
            summonAudio.Play();
            float yR = Random.Range(-200f, 200f);
            float xR = Random.Range(-200f, 200f);
            while (t < 1)
            {
                SummonOrb.localScale = SummonOrbMaxSize * t;
                SummonOrb.Rotate(transform.up, Time.deltaTime * yR);
                SummonOrb.Rotate(transform.right, Time.deltaTime * xR);
                t += Time.deltaTime / SummonTime;
                yield return true;
            }
            foreach (var v in HandsSummoning)
                v.Stop();
            t = 0;
            Anims().PlayAnimation("Throw", 1f, 0.25f);
            yield return new WaitForSeconds(0.2f);
            while (t < 0.25f)
            {
                SummonOrb.Rotate(transform.up, Time.deltaTime * yR);
                SummonOrb.Rotate(transform.right, Time.deltaTime * xR);
                t += Time.deltaTime;
                yield return true;
            }
            if (!PhotonNetwork.inRoom || EnemyStream.GetRealPlayerNum() < 2)
            {
                foreach (var v in ExtraEffects)
                    v.Stop();
                Shield.SetActive(false);
            }
            SummonOrb.gameObject.SetActive(false);
            if(tempTarg != null)
            {
                launcher.Launch(tempTarg);
                tempTarg = null;
            }
            else
                launcher.Launch();
        }
    }

    void ShieldOn()
    {
        if(Summoning)
            Shield.SetActive(true);
    }

    public Transform tempTarg;
    public void ExplodeSummon(ArrowCollision e)
    {
        health.takeDamage(25);
        ExplodeInHand(-1);
        tempTarg = PlayerHead.instance.transform;
        if (PhotonNetwork.inRoom)
            GetComponent<NetworkCreature>().ExplodeSummon(PhotonNetwork.player.ID);
    }

    public void ExplodeInHand(int explID=-1)
    {
        if(Summoning && gameObject.activeSelf)
        {
            StopCoroutine("SummonLoop");
            if(PhotonNetwork.inRoom && explID > -1)
                tempTarg = PlayerHead.GetHead(explID);
            HandExplode.Play();
            HandExplodeAudio.pitch = Random.Range(0.9f, 1.1f);
            HandExplodeAudio.Play();
            if (health.isDead())
                GetComponentInChildren<Rigidbody>().AddExplosionForce(500, SummonOrb.transform.position, 5f);
            foreach (var v in HandsSummoning)
                v.Stop();
            foreach (var v in ExtraEffects)
                v.Stop();
            Shield.SetActive(false);
            OnSummonEnd.Invoke();
            SummonOrb.gameObject.SetActive(false);
            StartCoroutine("NewLoop");
        }
    }

    IEnumerator NewLoop()
    {
        Anims().PlayAnimation("SummonStart", 0.6f, 0.25f);
        float l = Anims().anim["SummonStart"].length;
        yield return new WaitForSeconds(0.8f);
        sound.PlayRandomClip(MadClips, 1f, 8f, 0.6f);
        yield return new WaitForSeconds(l-0.8f);
        StartCoroutine("SummonLoop", Vector3.zero);
    }

    public override void Die()
    {
        StopCoroutine("SummonLoop");
        StopCoroutine("NewLoop");
        foreach (var v in HandsSummoning)
            v.Stop();
        foreach (var v in ExtraEffects)
            v.Stop();
        if(Summoning)
        {
            Summoning = false;
            OnSummonEnd.Invoke();
            Shield.SetActive(false);
        }
        SummonOrb.gameObject.SetActive(false);
        Anims().overrideAnimations = false;
        base.Die();
    }
}
