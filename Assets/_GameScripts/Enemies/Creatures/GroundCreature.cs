using UnityEngine;
using System.Collections;
using System;
using SWS;
using UnityEngine.AI;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent), typeof(navMove))]
public class GroundCreature : Creature {

    [Range(0.1f, 1)]
    public float LimpThreshold = 0.5f;
    float baseSpeed;
    float limp = 1;

    float mcd = 0;
    //bool setPath;
    //bool started;
    [HideInInspector]
    public navMove Movement;
    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent agent;
    public float footstepCD = 0.5f;
    public float MoveOffset;

    [Header("Runtime")]
    public PathManager nextPath;
    public bool MovementPaused;
    float NavRadius;
    Vector3 StartPos;

    public override void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Movement = GetComponent<navMove>();
        Movement.moveToPath = true;
        Movement.loopType = navMove.LoopType.none;
        base.Awake();
    }

    public override void Start()
    {
        InvokeRepeating("CheckTargRep", 5f, 5f);
        NavRadius = agent.radius;
        StartPos = transform.position;
        MoveOffset = UnityEngine.Random.Range(-3f, 3f);
        Movement.XYOffset = MoveOffset;
        if (GetComponent<NetworkCreature>() != null)
            GetComponent<NetworkCreature>().NavOffset(Movement.XYOffset);

        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            agent.enabled = true;
        else
            StartCoroutine("CheckNav");
        base.Start();
    }

    IEnumerator CheckNav()
    {
        NavMeshHit closestHit;
        while(!NavMesh.SamplePosition(transform.position, out closestHit, 3, NavMesh.AllAreas))
        {
            yield return true;
        }
        agent.enabled = true;
    }

    public void SetNewPath(PathManager p)
    {
        if(!isDead())
        {
            nextPath = p;
            Movement.moveToPath = true;
            Movement.Stop();
            Movement.ClearFinished();
            Movement.pathContainer = null;
            //setPath = false;
            SetState(EnemyState.walking);
        }
    }

    public override void SetCreature(Enemy e, int eid)
    {
        base.SetCreature(e, eid);
        SetSpeed();
    }

    void SetSpeed()
    {
        if(type.SpeedRange.x > 0)
        {
            float sMult = 1;
            if (GameBase.instance != null)
            {
                sMult = 1 + EnemyDB.v.SpeedMultiplier.Evaluate(CreatureManager.EnemyDifficulty);
            }
            sMult = Mathf.Clamp(sMult, 1, 1.75f);
            ChangeSpeedAbsolute(UnityEngine.Random.Range(type.SpeedRange.x, type.SpeedRange.y) * sMult);
            baseSpeed = agent.speed;
        }
    }

    bool linkDataValid;
    Vector3 linkStartPos;
    public override void Move()
    {
        CheckNavRadius();
        //Need New Path
        if (nextPath != null && Movement.pathContainer == null && !isDead() && agent.isOnNavMesh)
        {
            //Set Radius for Off Mesh Links
            if (!agent.hasPath)// && !setPath)
            {
                agent.SetDestination(ClosestPointOnPath(nextPath));
            }
            else if(agent.hasPath && agent.velocity.magnitude <= 0.01f && !MovementPaused)
            {
                //started = true;
                agent.Resume();
            }
            if (agent.remainingDistance < 2f)
            {
                agent.Stop();
                Movement.Stop();
                Movement.SetPath(nextPath);
                Movement.onStart = true;
                Movement.moveToPath = true;
                Movement.ClearFinished();
                Movement.StartMove();
                nextPath = null;
            }
        }
        //Base Movement
        if (Movement.pathContainer != null && !isDead() && nextPath == null && agent.isOnNavMesh)
        {           
            if (!Movement.hasStarted())
                Movement.StartMove();
            else if (Movement.hasFinished() && !MovementPaused && !agent.pathPending && agent.hasPath && agent.remainingDistance < AttackDistance)
            {
                if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
                    SetState(EnemyState.attacking);
            }
            else
            {
                if (skin != null && skin.isVisible)
                    mcd = 0;
                mcd += Time.deltaTime;
                if(mcd > footstepCD)
                {
                    mcd = 0;
                    sound.Footstep(0);
                }
            }    
        }
        //Death Stop Movement
        else if (isDead() && Movement.hasStarted())
        {
            Movement.Stop();
        }
        //Need Swap Path or Attack
        else if(!isDead() && Movement.pathContainer == null && nextPath == null && agent.isOnNavMesh && Target != null)
        {
            if (Target != null)
            {
                if (!agent.hasPath)
                    agent.SetDestination(Target.transform.position);
                /*
                else if (!agent.pathPending && agent.hasPath && agent.remainingDistance < AttackDistance && !MovementPaused && agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    SetState(EnemyState.attacking);
                }
                */
            }
            else
                UpdateTarget();
        }
    }

    void CheckNavRadius()
    {
        if(agent.isOnNavMesh && agent.hasPath)
        {
            linkDataValid = agent.nextOffMeshLinkData.valid;
            if (linkDataValid)
                linkStartPos = agent.nextOffMeshLinkData.startPos;
            if (linkDataValid && Vector3.Distance(transform.position, linkStartPos) < 5f)
                agent.radius = 0.0001f;
            else
                agent.radius = NavRadius;
    #if UNITY_EDITOR
            Debug.DrawLine(transform.position + Vector3.up, agent.destination + Vector3.up, Color.blue);
    #endif
        }
    }

    Vector3 ClosestPointOnPath(PathManager p)
    {
        Vector3 v = p.waypoints[0].position;
        float curDist = float.MaxValue;
        foreach(var t in p.waypoints)
        {
            float dist = Vector3.Distance(t.position, transform.position);
            if(dist < curDist)
            {
                curDist = dist;
                v = t.position;
            }
        }
        return v;
    }

    void CheckTargRep()
    {
        if(!isDead() && state == EnemyState.walking && !GetComponent<UnityEngine.AI.NavMeshAgent>().hasPath)
            Invoke("CheckTarg", 1f);
    }

    void CheckTarg()
    {
        UpdateTarget();
        if (!isDead() && state == EnemyState.walking && !agent.hasPath && Target != null && agent.isOnNavMesh)
            agent.SetDestination(Target.transform.position);
        else if (!isDead() && state == EnemyState.walking && !GetComponent<UnityEngine.AI.NavMeshAgent>().hasPath)
            Debug.Log("Target is Null, can't set destination.");
    }

    public void SetLimping()
    {
        limp = 0.5f;
        ChangeSpeed(curMult);
    }

    float curMult = 1;
    public void ChangeSpeed(float mult, bool sync=true)
    {
        curMult = mult;
        agent.speed = baseSpeed*mult*limp;
        Anims().speedMult = mult*limp;
        Anims().RefreshSpeeds();
        if(GetComponent<NetworkCreature>() != null && sync)
            GetComponent<NetworkCreature>().SetValues(true);
    }

    public void ChangeBaseSpeed(float mult, bool sync=true)
    {
        baseSpeed *= mult;
        Anims().WalkSpeed *= mult;
        ChangeSpeed(curMult, sync);
    }

    public void ChangeSpeedAbsolute(float val)
    {
        val = Mathf.Max(0.001f, val);
        float speed = GetComponent<UnityEngine.AI.NavMeshAgent>().speed;
        float mult = val / speed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = val;
        Anims().WalkSpeed *= mult;
        Anims().RefreshSpeeds();
        baseSpeed = speed;
        if (GetComponent<NetworkCreature>() != null)
            GetComponent<NetworkCreature>().SetValues(true);
    }

    public override void Attack()
    {
        AttackAction();
    }

    public override void SetNewTarget(GameObject o)
    {
        base.SetNewTarget(o);
    }

    public override void Die()
    {
        Movement.Stop();
        Movement.Pause();
        Movement.enabled = false;
        agent.enabled = false;
        base.Die();
    }
}
