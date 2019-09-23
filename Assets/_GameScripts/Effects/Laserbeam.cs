using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laserbeam : MonoBehaviour {

    [Header("Visuals")]
    public Transform Target;
    public LineRenderer[] lasers;
    public ParticleSystem[] LineParticles;
    public ParticleSystem Begin;
    public ParticleSystem End;
    ParticleSystem.EmissionModule EndEmiss;
    public AudioSource LaserCast;
    public AudioSource LaserImpact;

    [Header("Parameters")]
    public bool LaserOn;
    public float FollowSpeed;
    public int EnemyDamage;
    public int GateDamage;
    public LayerMask HitMask;
    public LayerMask KillMask;
    public float killRadius = 0.3f;
    Vector3 TargPos;

    Creature owner;

    void Awake()
    {
        EndEmiss = End.emission;
        owner = GetComponentInParent<Creature>();
    }

    void Start()
    {
        foreach (var laser in lasers)
        {
            laser.useWorldSpace = true;
            laser.SetPosition(0, transform.position);
            laser.SetPosition(0, transform.position);
        }
        TargPos = transform.position + transform.forward;
        if(Target.parent == transform)
            Target.SetParent(null);
        ToggleLaser(LaserOn);
    }

    [AdvancedInspector.Inspect]
    public void DebugToggleLaser()
    {
        ToggleLaser(!LaserOn);
    }
    
    public void ToggleLaser(bool on)
    {
        LaserOn = on;
        foreach (var v in lasers)
        {
            v.enabled = on;
        }
        foreach (var v in LineParticles)
        {
            if (on)
                v.Play();
            else
                v.Stop();
        }
        if (on)
        {
            Begin.Play();
            End.Play();
        }
        else
        {
            Begin.Stop();
            End.Stop();
        }
        TargPos = Target.position;
    }

    float TickTime = 1f;
    float tick;
    int x;
    Vector3 lastHit;
    Vector3 lastEmit;
    ParticleSystem.EmitParams _emitParams = new ParticleSystem.EmitParams();

    float _hitDistance = 0;
    int _actualHit;
    RaycastHit[] _hitBuffer = new RaycastHit[6];

    bool RaycastNA(Vector3 origin, Vector3 direction, int layerMask)
    {
        int numHits = Physics.RaycastNonAlloc(transform.position, transform.forward, _hitBuffer, 250, HitMask);        
        _actualHit = 0;
        if (numHits > 0)
        {
            for(int i=0;i<numHits;++i)
            {
                if(_hitBuffer[i].distance < _hitDistance)
                {
                    _actualHit = i;
                    _hitDistance = _hitBuffer[i].distance;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    void Update()
    {
        if(Target != null && LaserOn)
        {
            LaserCast.volume = 1;
            x++;
            if (x == 51)
                x = 1;
            tick -= Time.deltaTime;
            TargPos = Vector3.Lerp(TargPos, Target.position, FollowSpeed * Time.deltaTime);
            transform.LookAt(TargPos);
            Vector3 hitPoint = transform.position + (transform.forward * 250);
            //RaycastHit hit;
            //Physics.RaycastNonAlloc()
            //if (Physics.Raycast(transform.position, transform.forward, out hit, 250, HitMask))

            // Raycast versus environment
            //_hitDistance = 250; // RaycastNa modifies hit distance, be warned!
            //if (RaycastNA(transform.position, transform.forward, HitMask))
            RaycastHit hit;

            if (PhysicsUtils.RaycastNonAlloc(transform.position, transform.forward, out hit, 250, HitMask))
            {
                hitPoint = hit.point;
                if (Vector3.Distance(lastHit, hitPoint) < 5 && Vector3.Distance(lastHit, hitPoint) >= 0.25f)
                {
                    _emitParams.position = hitPoint;
                    End.Emit(_emitParams, 1);
                    lastEmit = hitPoint;
                }
                End.transform.position = hitPoint;
                //EndEmiss.rateOverDistance = 3;
                EndEmiss.rateOverTime = 4;
                LaserImpact.volume = 1;
                lastHit = hitPoint;           
            }
            else
            {
                LaserImpact.volume = 0;
                EndEmiss.rateOverDistance = 0;
                EndEmiss.rateOverTime = 0;
            }

            // Raycast versus enemies
            //RaycastHit[] kill;
            //kill = Physics.SphereCastAll(transform.position + transform.forward, killRadius, transform.forward, _hitDistance, KillMask);
            //int numHits = Physics.RaycastNonAlloc(transform.position + transform.forward, transform.forward, _hitBuffer, _hitDistance, KillMask);
            int numHits = Physics.RaycastNonAlloc(transform.position + transform.forward, transform.forward, _hitBuffer, hit.distance, KillMask);
            bool hitEnemy = false;
            for (int i=0;i<numHits;++i)
            {
                var k = _hitBuffer[i];
                PlayerHead head = k.collider.GetComponent<PlayerHead>();
                if (head != null)
                    head = k.collider.GetComponentInParent<PlayerHead>();
                if (!PlayerLife.dead() && head != null && head == PlayerHead.instance)
                {
                    PlayerLife.Kill();
                }

                Creature c = k.collider.GetComponentInParent<Creature>();
                if (c != this && c != null && !c.isDead() && !c.health.invincible && tick <= 0 && (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient))
                {
                    hitEnemy = true;
                    c.health.takeDamage(EnemyDamage);
                }
                if (x % 50 == 0)
                {
                    var emit = new ParticleSystem.EmitParams();
                    emit.position = k.point;
                    End.Emit(emit, 1);
                }
                if (owner != null)
                {
                    /*
                    Gate g = k.collider.GetComponentInParent<Gate>();
                    if (g != null && g.gameObject == owner.Target)
                    {
                        g.GetComponent<Health>().takeDamage(GateDamage);
                        g.GetComponent<Health>().SetAttacker(owner.gameObject);
                    }
                    */
                }
            }
            if (hitEnemy)
                tick = TickTime;
            foreach (var laser in lasers)
            {
                laser.SetPosition(0, transform.position);
                laser.SetPosition(1, hitPoint);
            }
            if(PlayerHead.instance != null)
            {
                Vector3 closest = ClosestPointOnLine(transform.position, hitPoint, PlayerHead.instance.transform.position);
                LaserCast.transform.position = closest;
            }

            Vector3 midPt = (transform.position + hitPoint) / 2f;
            float radius = Vector3.Distance(transform.position, hitPoint) /2f;
            foreach(var v in LineParticles)
            {
                ParticleSystem.ShapeModule shape = v.shape;
                shape.radius = radius;
                v.transform.position = midPt;
                v.transform.LookAt(hitPoint);
                v.transform.Rotate(-90, 0, 90);
            }
        }
        else
        {
            LaserImpact.volume = 0;
            LaserCast.volume = 0;
        }
    }

    Vector3 ClosestPointOnLine(Vector3 v1, Vector3 v2, Vector3 point)
    {
        Vector3 vVector1 = point - v1;
        Vector3 vVector2 = (v2 - v1).normalized;

        float d = Vector3.Distance(v1, v2);
        float t = Vector3.Dot(vVector2, vVector1);
        if (t <= 0)
            return v1;
        if (t >= d)
            return v2;
        Vector3 vVector3 = vVector2 * t;
        Vector3 closest = v1 + vVector3;
        return closest;
    }

    void OnDestroy()
    {
        if(Application.isPlaying && Target != null)
        {
            Destroy(Target.gameObject);
        }
    }
}
