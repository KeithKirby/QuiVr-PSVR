using UnityEngine;
using System.Collections;
using SWS;

[RequireComponent(typeof(splineMove))]
public class FlyingCreature : Creature {

    [HideInInspector]
    public splineMove Movement;

    public Transform Base;

    public GameObject[] Bombs;
    public bool hasBomb;
    float baseSpeed;
    [HideInInspector]
    public Vector3 StartPos;

    public override void Awake()
    {
        hasBomb = (Bombs.Length > 0);
        if (Base != null)
            Base.transform.localPosition += new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        Movement = GetComponent<splineMove>();
        Movement.moveToPath = true;
        Movement.loopType = splineMove.LoopType.none;
        base.Awake();
        if(Bombs.Length > 0)
        {
            BombIgnoreSelf();
        }
    }

    public override void Start()
    {
        StartPos = transform.position;
        base.Start();
    }

    void BombIgnoreSelf()
    {
        foreach(var Bomb in Bombs)
        {
            Collider br = Bomb.GetComponent<Collider>();
            foreach (var v in GetComponentsInChildren<Collider>())
            {
                if (v != br)
                {
                    Physics.IgnoreCollision(br, v);
                }
            }
        }
    }

    PathManager nextPath;
    public void SetNewPath(PathManager p)
    {
        if (!isDead())
        {
            nextPath = p;
            Movement.Stop();
            Movement.pathContainer = null;
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
        if (type.SpeedRange.x > 0)
        {
            float sMult = 1;
            if (GameBase.instance != null)
            {
                sMult = 1 + EnemyDB.v.SpeedMultiplier.Evaluate(CreatureManager.EnemyDifficulty);
            }
            sMult = Mathf.Clamp(sMult, 1, 2f);
            ChangeSpeedAbsolute(UnityEngine.Random.Range(type.SpeedRange.x, type.SpeedRange.y) * sMult);
        }
    }

    public override void Attack()
    {
        if (hasBomb && Bombs.Length > 0)
            Kill();
        if(hasBomb && Bombs.Length > 0 && !isDead())
        {
            for(int i=0; i<Bombs.Length; i++)
            {
                BombDrop d = Bombs[i].GetComponent<BombDrop>();
                if (!d.exploded)
                {
                    ExplodeInHand(d);
                    if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
                    {
                        Health t = Target.GetComponent<Health>();
                        if (t != null)
                            t.takeDamage(CalculateDamage());
                    }
                }
            }          
        }
        else if(!isDead() && !hasBomb)
        {
            transform.position = Movement.waypoints[Movement.waypoints.Length - 1];
            Vector3 v = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
            transform.LookAt(v);
            AttackAction();
        }
    }

    public void ExplodeInHand(BombDrop b)
    {
        for(int i=0; i<Bombs.Length; i++)
        {
            BombDrop d = Bombs[i].GetComponent<BombDrop>();
            if(b == d)
            {
                if (!d.exploded)
                {
                    ExplodeBombNetwork(i);
                    if (PhotonNetwork.inRoom)
                        GetComponent<PhotonView>().RPC("ExplodeBombNetwork", PhotonTargets.Others, i);
                }
                return;
            }
        }        
    }

    [PunRPC]
    void ExplodeBombNetwork(int bombID)
    {
        if (Bombs[bombID] == null)
            return;
        BombDrop d = Bombs[bombID].GetComponent<BombDrop>();
        if (d != null && !d.exploded)
            d.Explode();
        if(!isDead())
        {
            SetTagged();
            Die();
        }
        Dissolve();
    }
   
    [HideInInspector]
    public float flapCD = 0.5f;
    float mcd = 0;
    public override void Move()
    {
        if(nextPath != null && Movement.pathContainer == null && !isDead())
        {
            Vector3 relativePos = nextPath.waypoints[0].position - transform.position;
            Quaternion rotation = transform.rotation;
            if(relativePos.magnitude > 0.0001f)
                rotation = Quaternion.LookRotation(relativePos);
            Vector3 dir = (nextPath.waypoints[0].position - transform.position).normalized;
            //transform.position += Movement.speed * dir * Time.deltaTime;
            transform.SetPositionAndRotation(transform.position + (Movement.speed * dir * Time.deltaTime), rotation);
            if(Vector3.Distance(transform.position, nextPath.waypoints[0].position) < 1f)
            {
                Movement.SetPath(nextPath);
                Movement.onStart = true;
                Movement.moveToPath = false;
                Movement.StartMove();
                nextPath = null;
            }
        }
        if (Movement.pathContainer != null && !isDead())
        {
            if (!Movement.hasStarted())
            {
                Movement.StartMove();
            }
            else if (Movement.hasFinished())
            {
                transform.position = Movement.waypoints[Movement.waypoints.Length - 1];
                SetState(EnemyState.attacking);
            }
            else
            {
                //transform.LookAt(transform.position + transform.forward);
                transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
                if(sound != null)
                    sound.ToggleMovementSound((skin == null || skin.isVisible), 1);
            }
        }
        else if (isDead() && Movement.hasStarted())
        {
            if(sound != null)
                sound.ToggleMovementSound(false, 1);
            Movement.Stop();
        }
    }

    public void ChangeSpeed(float mult, bool sync=true)
    {
        GetComponent<splineMove>().ChangeSpeed(baseSpeed*mult);
        if (Anims() != null)
        {
            Anims().speedMult = mult;
            Anims().RefreshSpeeds();
        }
        if (GetComponent<NetworkCreature>() != null && sync)
            GetComponent<NetworkCreature>().SetValues(true);
    }

    public void ChangeSpeedAbsolute(float val)
    {
        val = Mathf.Max(0.001f, val);
        float speed = GetComponent<splineMove>().speed;
        float mult = val / speed;
        GetComponent<splineMove>().ChangeSpeed(val);
        if (Anims() != null)
        {
            Anims().WalkSpeed *= mult;
            Anims().RefreshSpeeds();
        }
        baseSpeed = val;
        if (GetComponent<NetworkCreature>() != null)
            GetComponent<NetworkCreature>().SetValues(true);
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
        if(sound != null)
            sound.ToggleMovementSound(false, 1);
        GetComponent<splineMove>().enabled = false;
        Invoke("RandomForce", 0.05f);
        if(Bombs.Length > 0)
        {
            foreach(var Bomb in Bombs)
            {
                if(NonPlayerDeath)
                {
                    Bomb.SetActive(false);
                }
                else
                {
                    BombDrop d = Bomb.GetComponent<BombDrop>();
                    if (!d.exploded && !d.dropped)
                    {
                        d.Drop();
                    }
                }
            }
        }
        base.Die();
    }

    void RandomForce()
    {
        foreach (var v in GetComponentsInChildren<Rigidbody>())
        {
            if (!v.isKinematic)
                v.AddExplosionForce(300, v.transform.position+ new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), 2);
        }
    }
}
