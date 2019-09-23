using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallLauncher : MonoBehaviour {

    [Header("Base")]
    public GameObject BallPrefab;
    public Transform[] launchPositions;
    public float MaxDist;
    public bool AtPlayer;
    public bool AtGate;
    public int Damage;
    public bool RequireAlive;
    public bool AutoLaunch = true;
    public Vector3 TargetOffset;
    [Header("Singleplayer")]
    public float Cooldown;
    public float Offset;
    [Header("Multiplayer")]
    public float MPCooldown;
    public float MPOffset;


    Health hscr;

    float curCD = 0;

    public static List<GameObject> Projectiles;

    public int curIdx = 0;

    void Awake()
    {
        Projectiles = new List<GameObject>();
        hscr = GetComponent<Health>();
    }

    void Start()
    {
        if (launchPositions.Length == 0)
            launchPositions = new Transform[] { transform };
        curCD = Offset;
        if (EnemyStream.GetRealPlayerNum() > 1)
            curCD = MPOffset;
    }

    void Update()
    {
        curCD -= Time.deltaTime;
        if(curCD <= 0 && AutoLaunch)
        {
            if(hscr != null)
            {
                if (hscr.isDead() && RequireAlive)
                    return;
            }
            Launch();
        }
    }

    public void Launch(Transform targ=null)
    {
        SetCooldown();
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            return;
        Transform target = targ;
        if (target == null && AtPlayer && EnemyStream.GetRealPlayerNum() > 0)
            target = PlayerHead.GetRandomHead();
        else if (target == null && GameBase.instance != null)
            target = GameBase.instance.CurrentTarget.transform;
        if(target != null && Vector3.Distance(target.position, transform.position) <= MaxDist)
        {
            ShootObject(target.position + TargetOffset);
            if (PhotonNetwork.inRoom && GetComponent<PhotonView>() != null)
                GetComponent<PhotonView>().RPC("ShootObject", PhotonTargets.Others, target.position + TargetOffset);
        }
    }

    void ChangeIndex()
    {
        curIdx++;
        if (curIdx >= launchPositions.Length)
            curIdx = 0;
    }

    [PunRPC]
    void ShootObject(Vector3 targetPos)
    {
        ChangeIndex();
        GameObject o = (GameObject)Instantiate(BallPrefab, launchPositions[curIdx].position, Quaternion.identity);
        Projectiles.Add(o);
        Creature c = GetComponent<Creature>();
        if (o.GetComponent<ProjectileLob>() != null)
        {
            o.GetComponent<ProjectileLob>().SetTarget(targetPos);
            o.GetComponent<Rigidbody>().useGravity = true;
            o.GetComponent<ProjectileLob>().Shoot();
        }
        if (o.GetComponent<Fireball>() != null)
        {
            Fireball mfb = o.GetComponent<Fireball>();
            if (c != null && c.Target != null)
                mfb.targGate = c.Target.GetComponent<Health>();
            mfb.Launcher = gameObject;
            mfb.GateDamage = Damage;
            mfb.OnBreak.AddListener(Break);
        }
    }

    public void Break(GameObject o)
    {
        if (Projectiles != null)
            Projectiles.Remove(o);
        Fireball fb = o.GetComponent<Fireball>();
        if (fb != null)
            fb.OnBreak.RemoveAllListeners();
        if(PhotonNetwork.inRoom && GetComponent<PhotonView>() != null)
            GetComponent<PhotonView>().RPC("BreakNetwork", PhotonTargets.Others, o.transform.position);
    }

    [PunRPC]
    void BreakNetwork(Vector3 pos)
    {
        float minDist = float.MaxValue;
        GameObject closest = null;
        ClearNull();
        foreach(var v in Projectiles)
        {
            float dist = Vector3.Distance(pos, v.transform.position);
            if(dist < minDist && dist < 5f)
            {
                closest = v;
                minDist = dist;
            }
        }
        if(closest != null)
        {
            Fireball fb = closest.GetComponent<Fireball>();
            if (fb != null && !fb.exploded)
                fb.DeathExplode();
        }
    }

    public void BreakAll()
    {
        ClearNull();
        for(int i=0; i<Projectiles.Count; i++)
        {
            var v = Projectiles[i];
            if(v != null)
            {
                Fireball fb = v.GetComponent<Fireball>();
                if (fb != null)
                {
                    fb.DeathExplode();
                }
            }
        }
    }

    void ClearNull()
    {
        for(int i= Projectiles.Count-1; i>=0; i--)
        {
            if (Projectiles[i] == null)
                Projectiles.RemoveAt(i);
        }
    }

    void SetCooldown()
    {
        curCD = Cooldown;
        if (EnemyStream.GetRealPlayerNum() > 1)
            curCD = MPCooldown;
        if (curCD < 0.25f)
            curCD = 0.25f;
    }
}
