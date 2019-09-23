using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathmaskCreature : FlyingCreature {

    Vector3 lastPos;
    bool ChasingPlayer;
    Transform PlayerTarget;
    PlayerHead targHead;
    public float MinDistTraveled;
    public AudioClip[] TargetAquired;
    Vector2[] Offsets;
    Vector2 DeadOffset;

    public LayerMask PlayerMask;

    public override void Awake()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), GetComponentInChildren<Rigidbody>().GetComponent<Collider>(), true);
        Offsets = new Vector2[] { new Vector2(-5, 0), new Vector2(5, 0), new Vector2(2.5f, 2.5f) , new Vector2(-2.5f, 2.5f) , new Vector2(2.5f, -2.5f),
        new Vector2(0, -5),new Vector2(0, 5),new Vector2(-2.5f, -2.5f)};
        DeadOffset = Offsets[Random.Range(0, Offsets.Length)];
        base.Awake();
    }

    public override void Start()
    {
        lastPos = transform.position;
        base.Start();
    }

    RaycastHit[] _raycastBuffer = new RaycastHit[4];

    float t = 0;
    public override void Update()
    {
        if(ChasingPlayer && !isDead())
        {
            MoveTowardPlayer();
            if (PlayerTarget != null && Vector3.Distance(PlayerTarget.position, transform.position) < 0.1f)
                KillExplode();
        }
        else if(t <= 0 && Vector3.Distance(StartPos, transform.position) >= MinDistTraveled && PlayerTarget == null)
        {
            SetPlayerTarget();
            t = 1f;
        }
        else if(PlayerTarget != null)
        {
            Vector3 toTarget = PlayerTarget.position - transform.position;
            float distToTarget = toTarget.magnitude;
            RaycastHit h;
            if(PhysicsUtils.RaycastNonAlloc(transform.position, toTarget, out h, distToTarget))
            {
                if (h.distance > distToTarget * 0.75f)
                    LockOnPlayer();
            }
        }
        t -= Time.deltaTime;
        base.Update();
    }

    bool SetPlayerTarget()
    {
        int pid = PlayerHead.GetRandomHeadID(transform.position, 150);
        if(pid > -2)
        {
            if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            {
                PlayerTarget = PlayerHead.GetHead(pid);
                targHead = PlayerTarget.GetComponent<PlayerHead>();
                if (PhotonNetwork.inRoom)
                    GetComponent<NetworkCreature>().TargetPlayer(pid);
            }
            return true;
        }
        return false;
    }

    public override void TargetPlayer(int pid)
    {
        PlayerTarget = PlayerHead.GetHead(pid);
        if(PlayerTarget != null)
            targHead = PlayerTarget.GetComponent<PlayerHead>();
        base.TargetPlayer(pid);
    }

    void LockOnPlayer()
    {
        ChasingPlayer = true;
        Movement.Pause();
        SetState(EnemyState.custom);
        if(PlayerTarget == PlayerHead.instance && TargetAquired.Length > 0)
        {
            AudioSource src = GetComponent<AudioSource>();
            if (src != null)
                src.minDistance = 4;
            VRAudio.PlayClipAtPoint(TargetAquired[Random.Range(0, TargetAquired.Length)], transform.position, 1, 1, 1, 15);
        }
    }

    float oldLerp = 1f;
    void MoveTowardPlayer()
    {
        Quaternion oldRot = transform.rotation;
        if(targHead != null && targHead.isDead())
        {
            Vector3 dir = (PlayerTarget.position - transform.position).normalized;
            Vector3 perp = Vector3.Cross(dir, DeadOffset).normalized;
            transform.LookAt(PlayerTarget.position + (perp*5f));
        }
        else
            transform.LookAt(PlayerTarget);
        Quaternion wantRot = transform.rotation;
        transform.rotation = Quaternion.Slerp(oldRot, wantRot, Time.deltaTime * 1.75f * 0.25f);
        float angle = Quaternion.Angle(oldRot, wantRot);
        angle = Mathf.Clamp(Mathf.Abs(angle), 10, 180);
        float speedDamp = 10 / angle;
        oldLerp = Mathf.Lerp(oldLerp, speedDamp, Time.deltaTime*1f);
        transform.position += transform.forward * Time.deltaTime * Movement.speed * oldLerp;
        //transform.position = Vector3.MoveTowards(transform.position, PlayerTarget.position, Time.deltaTime * Movement.speed);
    }

    void OnTriggerEnter(Collider col)
    {
        PlayerLife p = col.gameObject.GetComponentInParent<PlayerLife>();
        if (p != null && p == PlayerLife.myInstance)
            p.Die();
        if (ChasingPlayer)
            Kill(true);
    }

    void KillExplode()
    {
        PlayerLife p = PlayerTarget.GetComponentInParent<PlayerLife>();
        if (p != null && p == PlayerLife.myInstance)
            p.Die();
        Kill(true);
    }

    void LateUpdate()
    {
        //Vector3 moveDir = (transform.position - lastPos).normalized;
        //transform.LookAt(transform.position + moveDir);
        //lastPos = transform.position;
    }

}
