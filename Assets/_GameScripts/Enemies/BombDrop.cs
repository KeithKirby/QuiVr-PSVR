using UnityEngine;
using System.Collections;

public class BombDrop : MonoBehaviour {

    public float Damage = 20;
    public float ExplosionRadius = 2f;

    [HideInInspector]
    public bool exploded;
    [HideInInspector]
    public bool dropped;
    Vector3 delta;
    Vector3 lastPos;
    bool armed;

    public GameObject Pieces;
    public GameObject LQExplosion;
    public ParticleSystem Flames;

    public AudioClip[] Explodes;

    FlyingCreature fcreat;

    void Awake()
    {
        fcreat = GetComponentInParent<FlyingCreature>();
    }

    void Update()
    {
        if(!exploded && !dropped)
        {
            delta = (transform.position - lastPos)/Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        lastPos = transform.position;
    }

    public void TryCreatureExplode()
    {
        if(fcreat != null)
            fcreat.ExplodeInHand(this);
    }

	public void Explode(GameObject collider = null)
    {
        Flames.Stop();
        exploded = true;
        ExplosionEffect();
        if(collider == null || collider.GetComponentInParent<ShieldCollisionBehaviour>() == null)
            TryKillPlayer();
        TryKillCreatures();
        Invoke("ExplodePhysics", 0.1f);
    }

    [BitStrap.Button]
    public void Drop()
    {
        transform.SetParent(null);
        dropped = true;
        Rigidbody r = GetComponent<Rigidbody>();
        r.isKinematic = false;
        r.velocity = delta;
        armed = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if(armed && !exploded)
        {
            Explode(col.collider.gameObject);
            Gate g = col.gameObject.GetComponentInParent<Gate>();
            if(g != null)
            {
                /*
                Health h = g.GetComponent<Health>();
                if (h != null)
                    h.takeDamage(Damage);
                */
            }
        }
    }

    void TryKillPlayer()
    {
        if (PlayerHead.instance != null)
        {
            var vc = PlayerHead.instance;
            if(Vector3.Distance(vc.transform.position, transform.position) < ExplosionRadius)
            {
                var pl = vc.GetComponentInParent<PlayerLife>();
                if (pl != null)
                    pl.Die();
            }
        }
    }

    void TryKillCreatures()
    {
        if(GameBase.instance != null)
        {
            foreach(var v in CreatureManager.AllEnemies())
            {
                if(v != null && Vector3.Distance(transform.position, v.transform.position) < ExplosionRadius * 2.1f)
                {
                    Health h = v.GetComponent<Health>();
                    if (h != null && !h.isDead())
                    {
                        h.takeDamage(Damage);
                    }
                }
            }
        }

    }

    void ExplodePhysics()
    {
        Collider[] objects = UnityEngine.Physics.OverlapSphere(transform.position, ExplosionRadius*1.2f);
        foreach (Collider h in objects)
        {
            Rigidbody r = h.GetComponent<Rigidbody>();
            if (r != null && !r.isKinematic)
            {
                r.AddExplosionForce(800, transform.position, ExplosionRadius * 2);
            }
        }
    }

    void ExplosionEffect()
    {
        AudioSource a = GetComponent<AudioSource>();
        if (a != null)
            a.Stop();
        if (Explodes.Length > 0)
            VRAudio.PlayClipAtPoint(Explodes[Random.Range(0, Explodes.Length)], transform.position, 1* VolumeSettings.GetVolume(AudioType.Effects), 1, .99f, 10);
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        if (Settings.GetBool("ExtraFX"))
        {
            Pieces.SetActive(true);
            Destroy(Pieces, 10f);
            foreach (var v in GetComponentsInChildren<Rigidbody>())
            {
                v.transform.SetParent(null);
                Destroy(v.gameObject, 9f);
                v.velocity = GetComponent<Rigidbody>().velocity;
                v.AddExplosionForce(300, transform.position + (Vector3.down * 0.2f), 5);
            }
            Pieces.transform.SetParent(null);
        }
        else
        {
            Instantiate(LQExplosion, transform.position, Quaternion.identity);
        }
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
