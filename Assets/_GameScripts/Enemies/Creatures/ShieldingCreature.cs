using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class ShieldingCreature : GroundCreature {

    [Header("Shielding")]
    public bool Casting;
    public float ShieldRadius;
    public float CastTime;
    public ParticleSystem HandCast;
    public ParticleSystem[] CastingEffects;
    public GameObject[] CastingObjects;
    public GameObject ShieldPrefab;
    public UnityEvent OnCastStart;
    public UnityEvent OnCastEnd;
    public AudioClip ShieldStart;
    public AudioClip ShieldLoop;
    List<ShieldedAlly> Shielded;
    static List<ShieldingCreature> Shielders;
    AudioSource src;
    public GameObject CasterShield;

    Vector3 startPos;
    public float WantDistance;

    public override void Awake()
    {
        if (Shielders == null)
            Shielders = new List<ShieldingCreature>();
        Shielders.Add(this);
        Shielded = new List<ShieldedAlly>();
        src = GetComponent<AudioSource>();
        startPos = transform.position;
        WantDistance = Random.Range(25, 50);
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    public override void Update()
    {
        if (Vector3.Distance(transform.position, startPos) >= WantDistance && !Casting && state == EnemyState.walking && TimeAlive > 5f)
        {
            EnterCast();
        }
        base.Update();
    }

    [AdvancedInspector.Inspect]
    void EnterCast()
    {
        EnterCast(transform.position);
    }

    public void EnterCast(Vector3 targMove)
    {
        if(!Casting && gameObject.activeSelf)
        {
            Casting = true;
            MovementPaused = true;
            Anims().overrideAnimations = true;
            /*
            if ((!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient) && agent.isOnNavMesh)
            {
                Vector3 move = transform.position + new Vector3(Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f));
                NavMeshHit hit;
                if (NavMesh.SamplePosition(move, out hit, 5.0f, NavMesh.AllAreas))
                    targMove = hit.position;
            }
            */
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                GetComponent<NetworkCreature>().ToggleSummoning(true, targMove);
            StopCoroutine("CastLoop");
            StopCoroutine("ChangeToLoop");
            StartCoroutine("CastLoop", targMove);
        }     
    }

    [AdvancedInspector.Inspect]
    public void ExitCast()
    {
        if(Casting && gameObject.activeSelf)
        {
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                GetComponent<NetworkCreature>().ToggleSummoning(false, Vector3.zero);
            StopCoroutine("CastLoop");
            StopCoroutine("ExitSequence");
            StopCoroutine("ChangeToLoop");
            StartCoroutine("ExitSequence");
        }
    }

    IEnumerator CastLoop(Vector3 targMove)
    {
        Movement.Pause();
        /*
        if (targMove != Vector3.zero && agent.isOnNavMesh)
        {
            agent.SetDestination(targMove);
            agent.isStopped = false;
            while (agent.isOnNavMesh && agent.pathPending)
                yield return true;
            while (agent.isOnNavMesh && agent.remainingDistance > 1f)
                yield return true;
        }
        if (agent.isOnNavMesh)
            agent.isStopped = true;
        */
        src.clip = ShieldStart;
        src.time = 0;
        src.loop = false;
        src.volume = 0.8f;
        src.Play();
        StartCoroutine("ChangeToLoop");
        Anims().PlayAnimation("Channel_Start", 0.9f, 0.5f);
        float l = Anims().anim["Channel_Start"].length;
        yield return new WaitForSeconds(0.3f);
        ToggleVisuals(true);
        yield return new WaitForSeconds(l - 0.3f);
        Anims().PlayAnimation("ChannelLoop", 0.6f, 1f);
        float ctime = CastTime;
        if (PlayerHead.ClosestPlayerDistance(transform.position) > 50)
            ctime /= 3f;
        Invoke("NewPosition", ctime);
        if (CasterShield != null)
            CasterShield.SetActive(true);
        while (true)
        {
            UpdateShields();
            yield return true;
        }
    }

    IEnumerator ExitSequence()
    {
        CancelInvoke();
        ToggleVisuals(false);
        float t = 0;
        float v = src.volume;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            src.volume = v - (v * (t / 0.5f));
            yield return true;
        }
        Casting = false;
        UpdateShields();
        if (CasterShield != null)
            CasterShield.SetActive(false);
        MovementPaused = false;
        Anims().overrideAnimations = false;
        SetState(EnemyState.walking);
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            Move();
            Movement.Resume();
        }
    }

    IEnumerator ChangeToLoop()
    {
        while (src.isPlaying)
        {
            yield return true;
        }
        src.clip = ShieldLoop;
        src.loop = true;
        src.Play();
    }

    void NewPosition()
    {
        startPos = transform.position;
        WantDistance = Random.Range(15, 40);
        ExitCast();
    }

    void ToggleVisuals(bool on)
    {
        foreach(var v in CastingEffects)
        {
            if (on)
                v.Play();
            else
                v.Stop();
        }
        if (on)
        {
            OnCastStart.Invoke();
            HandCast.Play();
        }
        else
        {
            OnCastEnd.Invoke();
            HandCast.Stop();
        }
    }

    void UpdateShields(bool immediate=false)
    {
        FixLists();
        if(!Casting)
        {
            for(int i=0; i< Shielded.Count;i++)
            {
                if (Shielded[i].shield != null)
                    Shielded[i].RemoveShield(immediate);
            }
            Shielded = new List<ShieldedAlly>();
        }
        else
        {
            for(int i=Shielded.Count-1; i>=0; i--)
            {
                if(Shielded[i].creature != null)
                {
                    Shielded[i].shield.position = Shielded[i].creature.transform.position;
                    if (Vector3.Distance(transform.position, Shielded[i].creature.transform.position) > ShieldRadius)
                    {
                        Shielded[i].creature.health.invincible = false;
                        Shielded[i].RemoveShield();
                        Shielded.RemoveAt(i);
                    }
                    else
                        Shielded[i].creature.health.invincible = true;
                }               
            }
            foreach (var v in CreatureManager.AllEnemies())
            {
                if(v != null && v.GetType() != typeof(ShieldingCreature) && v.GetType() != typeof(DeathmaskCreature) && !ContainsCreature(v))
                {
                    if(Vector3.Distance(transform.position, v.transform.position) < ShieldRadius)
                    {
                        AddShielded(v);
                    }
                }
            }
        }
    }

    void FixLists()
    {
        for(int i= Shielded.Count-1; i>=0; i--)
        {
            if (Shielded[i].creature == null || Shielded[i].creature.isDead())
            {
                if (Shielded[i].shield != null)
                    Shielded[i].RemoveShield();
                Shielded.RemoveAt(i);
            }
        }
    }

    void AddShielded(Creature c)
    {
        ShieldedAlly ally = new ShieldedAlly();
        ally.creature = c;
        ally.creature.health.invincible = true;
        GameObject newShield = Instantiate(ShieldPrefab);
        newShield.SetActive(true);
        newShield.transform.position = c.transform.position;
        newShield.transform.localScale = new Vector3(1.5f, 1.3f, 1.5f) * c.modelScale;
        ally.shield = newShield.transform;
        Shielded.Add(ally);
    }

    bool ContainsCreature(Creature c)
    {
        foreach(var v in Shielded)
        {
            if (c == v.creature)
                return true;
        }
        return false;
    }

    public override void Die()
    {
        StopCoroutine("SiegeLoop");
        StopCoroutine("ExitSequence");
        CancelInvoke();
        Casting = false;
        src.volume = 0;
        src.Stop();
        if (CasterShield != null)
            CasterShield.SetActive(false);
        UpdateShields(true);
        if(Casting)
            ToggleVisuals(false);
        Anims().overrideAnimations = false;
        Shielders.Remove(this);
        base.Die();
    }

    void OnDestroy()
    {
        Shielders.Remove(this);
    }

    [System.Serializable]
    public class ShieldedAlly
    {
        public Creature creature;
        public Transform shield;

        public void RemoveShield(bool immediate = false)
        {
            if(shield != null)
            {
                if(immediate)
                {
                    Destroy(shield.gameObject);
                }
                else
                {
                    FadeInOutColorMulti[] cfade = shield.GetComponentsInChildren<FadeInOutColorMulti>();
                    foreach (var v in cfade)
                    {
                        v.FadeOut();
                    }
                    shield.GetComponentInChildren<RFX4_AudioCurves>().Reverse();
                    Collider c = shield.GetComponentInChildren<Collider>();
                    if (c != null)
                        c.enabled = false;
                    Destroy(shield.gameObject, 2.5f);
                }
            }
            if (creature != null)
                creature.health.invincible = false;
        }
    }

}
