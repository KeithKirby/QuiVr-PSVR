using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LaserCreature : GroundCreature {

    [Header("Laser System")]
    public Laserbeam laser;
    public Transform LaserTarget;
    public Transform HeadHolder;
    public float LaserSpeed;
    public float SwipeRadius;
    public Transform PlayerTarget;
    public bool SiegeMode;

    [Header("End Explosion")]
    public GameObject ExplodeCharge;
    public GameObject Explosion;
    public float ExplodeChargeTime;
    bool firstLaser;

    Vector3 startPos;
    float WantDistance;

    public override void Awake()
    {
        if(PlayerHead.instance != null)
            NewPlayerTarget();
        Invoke("NewPlayerTarget", 25f);
        WantDistance = Random.Range(30f, 60f);
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    bool needHead;
    public override void Update()
    {
        if (PlayerHead.instance == null)
            needHead = true;
        else if(PlayerHead.instance != null && needHead)
        {
            NewPlayerTarget();
            needHead = false;
        }
        if(laser.LaserOn)
            HeadHolder.LookAt(LaserTarget);
        if (!Exploding && TimeAlive > 5f && Vector3.Distance(startPos, transform.position) > WantDistance)//(PlayerTarget != null && Vector3.Distance(transform.position, Target.transform.position) < 100 && !Exploding) //Check LOS to player
        {
            if (!SiegeMode && !isDead())
            {
                /*
                RaycastHit hit;
                bool LOS = true;
                if(Physics.SphereCast(HeadHolder.position, 1, DirToTarg(), out hit, 100))
                {
                    if (hit.distance < DistToTarg() * 0.9f)
                        LOS = false;
                }
                if (LOS && (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient))
                */
                    EnterSiege(transform.position);
            }
        }
        /*
        else if (PlayerTarget == null || (PlayerTarget != null && Vector3.Distance(transform.position, Target.transform.position) > 100 + 15))
        {
            if (SiegeMode && finishedLoop)
            {
                if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
                    ExitSiege();
            }
        }
        */
        base.Update();
    }

    public int plrsTargeted;
    void NewPlayerTarget()
    {
        if (Exploding)
            return;
        CancelInvoke();
        Invoke("NewPlayerTarget", 25f);
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            plrsTargeted++;
            //if (plrsTargeted % 3 != 0 || !firstLaser)
            PlayerTarget = PlayerHead.GetRandomHead();
           // else
              //  PlayerTarget = null;
        }
        else if (!PhotonNetwork.inRoom && PlayerHead.instance != null)
        {
            plrsTargeted++;
            //if (plrsTargeted % 2 == 1 || !firstLaser)
            PlayerTarget = PlayerHead.instance.transform;
           // else
                //PlayerTarget = null;
        }
        //Debug.Log("Target Set: " + PlayerTarget);
        if (PlayerTarget != null)
            headTarg = PlayerTarget.GetComponent<PlayerHead>();
    }

    [AdvancedInspector.Inspect]
    public void EnterSiege()
    {
        EnterSiege(transform.position);
    }

    public void EnterSiege(Vector3 targMove)
    {
        SiegeMode = true;
        MovementPaused = true;
        Anims().overrideAnimations = true;
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            Vector3 move = transform.position + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
            NavMeshHit hit;
            if (NavMesh.SamplePosition(move, out hit, 5.0f, NavMesh.AllAreas))
                targMove = hit.position;
        }
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            GetComponent<NetworkCreature>().ToggleSummoning(true, targMove);
        StopCoroutine("SiegeLoop");
        StopCoroutine("ExitSequence");
        StartCoroutine("SiegeLoop", targMove);
    }

    bool finishedLoop;
    PlayerHead headTarg;
    IEnumerator SiegeLoop(Vector3 targMove)
    {
        Movement.Pause();
        if (targMove != Vector3.zero && agent.isOnNavMesh)
        {
            agent.SetDestination(targMove);
            agent.isStopped = false;
            while (agent.pathPending)
                yield return true;
            while (agent.remainingDistance > 1f)
                yield return true;
        }
        if(agent.isOnNavMesh)
            agent.isStopped = true;
        Quaternion lookRotation = Quaternion.LookRotation(DirToTarg());
        float x = 999;
        while(x > 20)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);
            x = Mathf.Abs(Quaternion.Angle(transform.rotation, lookRotation));
            yield return true;
        }
        Anims().PlayAnimation("Enter_Siege_Mode", 0.5f, 0.25f);
        float l = Anims().anim["Enter_Siege_Mode"].length;
        yield return new WaitForSeconds(l);
        Anims().PlayAnimation("Idle_Siege_Mode", 0.8f, 0.25f);
        yield return new WaitForSeconds(0.3f);
        LaserTarget.transform.position = transform.position + transform.forward * 2f;
        laser.ToggleLaser(true);
        while (true)
        {
            yield return true;
            firstLaser = true;
            finishedLoop = false;
            GenerateSwipe();
            while(Vector3.Distance(LaserTarget.position, PStart) > 0.1f)
            {
                LaserTarget.position = Vector3.MoveTowards(LaserTarget.position, PStart, Time.deltaTime * LaserSpeed * 5f);
                yield return true;
            }
            yield return new WaitForSeconds(1f);
            while (Vector3.Distance(LaserTarget.position, PEnd) > 0.1f)
            {
                if (headTarg != null && headTarg.isDead())
                    LaserTarget.position = Vector3.MoveTowards(LaserTarget.position, PStart, Time.deltaTime * LaserSpeed * 5f);
                else
                    LaserTarget.position = Vector3.MoveTowards(LaserTarget.position, PEnd, Time.deltaTime * LaserSpeed);
                yield return true;
            }
            yield return new WaitForSeconds(1f);
            finishedLoop = true;
            //NewPlayerTarget();
        }
    }

    Vector3 PStart;
    Vector3 PEnd;
    Vector3 TargPoint;
    int side = 1;
    void GenerateSwipe()
    {
        side *= -1;
        TargPoint = HeadHolder.position + HeadHolder.forward * 25 + HeadHolder.up * 10f;
        Vector3 dir = (TargPoint - HeadHolder.position).normalized;
        if (PlayerTarget != null)
        {
            dir = (PlayerTarget.position - HeadHolder.position).normalized;
            TargPoint = PlayerTarget.position;
        }
        Vector3 perp = Vector3.Cross(dir, new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f))).normalized;//Vector3.Cross(dir, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
        if (side < 0)
            perp = Vector3.Cross(dir, new Vector3(Random.Range(-1f, 0), Random.Range(-1f, 0), Random.Range(-1f, 0))).normalized;
        PStart = TargPoint + (perp * SwipeRadius);
        perp = Quaternion.AngleAxis(180, dir) * perp;
        PEnd = TargPoint + (perp * SwipeRadius);
        Debug.DrawLine(HeadHolder.position, TargPoint, Color.blue, 8f);
        Debug.DrawLine(TargPoint, PStart, Color.red, 8f);
        Debug.DrawLine(TargPoint, PEnd, Color.yellow, 8f);
    }

    Vector3 DirToTarg()
    {
        if(PlayerTarget != null)
            return (PlayerTarget.position - HeadHolder.position).normalized;
        return HeadHolder.forward;
    }

    float DistToTarg()
    {
        if (PlayerTarget != null)
            return (PlayerTarget.position - HeadHolder.position).magnitude;
        return 25;
    }

    [AdvancedInspector.Inspect]
    public void ExitSiege()
    {
        SiegeMode = false;
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            GetComponent<NetworkCreature>().ToggleSummoning(false, Vector3.zero);
        StopCoroutine("SiegeLoop");
        StopCoroutine("ExitSequence");
        StartCoroutine("ExitSequence");       
    }

    public bool Exploding;
    public override void Attack()
    {
        if(!Exploding)
        {
            Exploding = true;
            MovementPaused = true;
            Anims().overrideAnimations = true;
            StartCoroutine("ExplodeEnd");
        }
    }

    IEnumerator ExitSequence()
    {
        laser.ToggleLaser(false);
        Anims().PlayAnimation("Exit_Siege_Mode", 0.6f, 0.25f);
        float l = Anims().anim["Exit_Siege_Mode"].length;
        yield return new WaitForSeconds(l);
        MovementPaused = false;
        Anims().overrideAnimations = false;
        SetState(EnemyState.walking);
        if(agent.isOnNavMesh)
        {
            agent.isStopped = false;
            Move();
            Movement.Resume();
        }
    }

    IEnumerator ExplodeEnd()
    {
        Movement.Pause();
        Anims().PlayAnimation("Enter_Siege_Mode", 0.5f, 0.25f);
        float l = Anims().anim["Enter_Siege_Mode"].length;
        yield return new WaitForSeconds(l);
        Anims().PlayAnimation("Idle_Siege_Mode", 0.8f, 0.25f);
        ExplodeCharge.SetActive(true);
        yield return new WaitForSeconds(ExplodeChargeTime);
        ExplodeCharge.SetActive(false);
        Explosion.SetActive(true);
        Explosion.transform.SetParent(null);
        Health t = Target.GetComponent<Health>();
        if (t != null)
            t.takeDamage(Damage);
        Kill();
    }

    public override void Die()
    {
        StopCoroutine("SiegeLoop");
        StopCoroutine("ExitSequence");
        StopCoroutine("ExplodeEnd");
        laser.ToggleLaser(false);
        Anims().overrideAnimations = false;
        base.Die();
    }
}
