using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.AI;
using SWS;

[RequireComponent (typeof (Creature))]
public class NetworkCreature : MonoBehaviour {

    Creature creature;

    PhotonView v;
    Vector3 pos;
    float moveCD;
    Quaternion rot;
    Vector3 lastPos;
    Quaternion lastRot;
    int animState;

    Health h;

    bool updating;

    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    public IntEvent intEvent;

    NavMeshAgent agent;

    public static float UpdateRate = 1;

    float updCD = 0;
    void Awake()
    {
        v = GetComponent<PhotonView>();
        h = GetComponent<Health>();
        creature = GetComponent<Creature>();
        pos = transform.position;
        rot = transform.rotation;
        animState = (int)creature.state;
        agent = GetComponent<NavMeshAgent>();
        if (!PhotonNetwork.inRoom)
        {
            Destroy(v);
            Destroy(this);
        }
        if (PhotonNetwork.inRoom)
            PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    public void InvokeIntEvent(int i)
    {
        v.RPC("IntEventNetwork", PhotonTargets.Others, i);
    }

    [PunRPC]
    void IntEventNetwork(int i)
    {
        intEvent.Invoke(i);
    }

    void Start()
    {
        if(PhotonNetwork.inRoom)
        {
            CreatureManager.AddCreature(creature);
        }
    }

    bool clearedNav = false;
    void Update() 
    {
        if (!v.isMine && PhotonNetwork.inRoom)
        {
            if (creature != null && creature.state != (EnemyState)animState)
                creature.SetState((EnemyState)animState);
            if (creature.state == EnemyState.attacking)
            {
                //transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 4);
                //transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 4);
                if (!clearedNav)
                { 
                    if (agent != null && agent.isOnNavMesh)
                    {
                        agent.Warp(pos);
                        agent.isStopped = true;
                        if (agent.hasPath)
                            agent.ResetPath();
                        clearedNav = true;
                    }
                }
            }
            else
                clearedNav = false;
        }
        updCD -= Time.deltaTime;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting && updCD <= 0 && !h.isDead())
        {
            updCD = UpdateRate;
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            if (creature != null)
                stream.SendNext((int)creature.state);
        }
        else if(stream.isReading)
        {
            pos = (Vector3)stream.ReceiveNext();
            rot = (Quaternion)stream.ReceiveNext();
            if (creature != null)
                animState = (int)stream.ReceiveNext();
        }
    }

    public void SetValues(bool overrideMaster=false)
    {
        if(PhotonNetwork.inRoom && (PhotonNetwork.isMasterClient || overrideMaster))
        {
            float health = GetComponent<Health>().maxHP;
            float speedMult = GetComponent<EnemyAnimations>().speedMult;
            float speed = 0;
            float walkSpeed = GetComponent<EnemyAnimations>().WalkSpeed;
            if (agent != null)
                speed = agent.speed;
            else if (GetComponent<splineMove>() != null)
                speed = GetComponent<splineMove>().speed;
            v.RPC("SetValuesNetwork", PhotonTargets.Others, health, speed, speedMult, walkSpeed);
        }
    }

    [PunRPC]
    void SetValuesNetwork(float health, float speed, float speedMult, float walkSpeed)
    {
        Health h = GetComponent<Health>();
        if (h.currentHP == h.maxHP)
            h.currentHP = health;
        h.maxHP = health;
        creature.Anims().WalkSpeed = walkSpeed;
        creature.Anims().speedMult = speedMult;
        creature.Anims().RefreshSpeeds();
        if (agent != null)
            agent.speed = speed;
        if (GetComponent<splineMove>() != null)
            GetComponent<splineMove>().speed = speed;
    }

    public void NavOffset(float offst)
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            v.RPC("NavOffsetNetwork", PhotonTargets.Others, offst);
        }
    }

    [PunRPC]
    void NavOffsetNetwork(float offset)
    {
        navMove nm = GetComponent<navMove>();
        if (nm != null)
            nm.XYOffset = offset;
    }

    public void IncreaseDeathEnergy(int energy)
    {
        v.RPC("NetworkIncreaseDeathEnergy", PhotonTargets.MasterClient);
    }

    [PunRPC]
    void NetworkIncreaseDeathEnergy(int energy)
    {
        if (creature.isDead())
            GameOrb.GiveEnergy(creature.RootBone.position, energy);
        else
            creature.ModDeathEnergy(energy);
    }

    public void Kill()
    {
        v.RPC("KillNetwork", PhotonTargets.Others);
    }

    [PunRPC]
    void KillNetwork()
    {
        creature.Kill();
    }

    public void IgnoreArrow(PunTeams.Team team)
    {
        v.RPC("NetworkIgnoreArrow", PhotonTargets.Others, (int)team);
    }

    [PunRPC]
    void NetworkIgnoreArrow(int team)
    {
        PunTeams.Team t = (PunTeams.Team)team;
        creature.IgnoreArrows(t, false);
    }

    public void Explode()
    {
        v.RPC("ExplodeNetwork", PhotonTargets.Others);
    }

    [PunRPC]
    void ExplodeNetwork()
    {
        var bc = GetComponent<BomberCreature>();
        if (bc != null)
        {
            bc.ExplodeDisplay();
        }
        bc.Die();
    }

    public void ChangePosition(Vector3 pos, int waypointID = -1)
    {
        if (PhotonNetwork.inRoom)
            v.RPC("ChangePositionNetwork", PhotonTargets.Others, pos, waypointID);
    }

    [PunRPC]
    void ChangePositionNetwork(Vector3 pos, int waypointID = -1)
    {
        if (agent != null)
            agent.Warp(pos);
        else
            transform.position = pos;
        //Ground Creature
        SWS.navMove nm = GetComponent<SWS.navMove>();
        if (waypointID > -1 && nm != null && agent != null && agent.isOnNavMesh && nm.waypoints.Length > 0)
            agent.SetDestination(nm.waypoints[Mathf.Clamp(waypointID, 0, nm.waypoints.Length-1)].position);
        //Flying Creature
        //Todo
    }

    void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        if (PhotonNetwork.isMasterClient)
        {
            v.RPC("SetNow", other, transform.position, transform.rotation); 
        }           
    }

    [PunRPC]
    void SetNow(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    public void TargetPlayer(int pid)
    {
        if (PhotonNetwork.isMasterClient)
        {
            v.RPC("TargetPlayerNetwork", PhotonTargets.Others, pid);
        }
    }

    [PunRPC]
    void TargetPlayerNetwork(int pid)
    {
        creature.TargetPlayer(pid);
    }

    public void CatchFire(float dmg)
    {
        if(PhotonNetwork.inRoom)
        {
            v.RPC("CatchFireNetwork", PhotonTargets.Others, dmg);
        }
    }

    [PunRPC]
    void CatchFireNetwork(float dmg)
    {
        creature.DoCatchFire(dmg);
    }

    #region Creature Specific
    //Teleporter
    public void TeleporterTrigger()
    {
        if (PhotonNetwork.inRoom)
            v.RPC("NetworkTPTrigger", PhotonTargets.Others);
    }

    [PunRPC]
    void NetworkTPTrigger()
    {
        if(creature != null && creature.GetType() == typeof(TeleportingCreature))
        {
            (creature as TeleportingCreature).TeleportForward(false);
        }
    }


    //Fireball Enemy
    public void ToggleSummoning(bool val, Vector3 moveTo)
    {
        if (PhotonNetwork.inRoom)
            v.RPC("NetworkToggleSummoning", PhotonTargets.Others, val, moveTo);
    }

    [PunRPC]
    void NetworkToggleSummoning(bool val, Vector3 moveTo)
    {
        if(val)
        {
            if(creature is FireballCreature)
                ((FireballCreature)creature).EnterSummon(moveTo);
            else if (creature is ShieldingCreature)
                ((ShieldingCreature)creature).EnterCast(moveTo);
            else if(creature is LaserCreature)
                ((LaserCreature)creature).EnterSiege(moveTo);
        }
        else
        {
            if(creature is FireballCreature)
                ((FireballCreature)creature).ExitSummon();
            else if(creature is ShieldingCreature)
                ((ShieldingCreature)creature).ExitCast();
            else if(creature is LaserCreature)
                ((LaserCreature)creature).ExitSiege();
        }

    }

    public void ExplodeSummon(int exploderID)
    {
        if (PhotonNetwork.inRoom)
            v.RPC("NetworkExplodeSummon", PhotonTargets.Others, exploderID);
    }

    [PunRPC]
    void NetworkExplodeSummon(int exploderID)
    {
        ((FireballCreature)creature).ExplodeInHand(exploderID);
    }

    #endregion
}
