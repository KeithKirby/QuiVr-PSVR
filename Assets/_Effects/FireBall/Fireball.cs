using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Fireball : MonoBehaviour {

    public float Speed = 3f;
    Vector3 dir;
    bool hasTarget;
    public bool exploded = false;
    public GameObject pieces;
    public GameObject LQExplode;
    float Speedup = 1f;
    public int GateDamage;

    public bool spin;
    public Transform spinOverride;
    public bool spinRandom;
    public Vector3 spinValue;

    public bool lookDirection;

    public ParticleSystem[] Flames;

    public AudioClip[] Explodes;
    AudioSource baseSrc;

    public AudioClip SpawnSound;
    [Range(0,1)]
    public float Spawn3D;
    public float spawnVolume;

    public float hitDistance = 1.5f;

    public MeshRenderer baseRend;
    public Collider baseCol;

    [Range(0, 1)]
    public float Explode3D;
    public float explodeVolume = 0.75f;

    public float DestroyTime = 20f;
	public float destroyAfterExplode = 2f;
    public ParticleSystem trail;

    [System.Serializable]
    public class GOEvent : UnityEvent<GameObject> { };
    public GOEvent OnBreak;

    public float InvincibleTime = 0.5f;

    void Start()
    {
        if (SpawnSound != null)
            VRAudio.PlayClipAtPoint(SpawnSound, transform.position, spawnVolume* VolumeSettings.GetVolume(AudioType.Effects), 1, Spawn3D, 15);
        baseSrc = GetComponent<AudioSource>();
        Destroy(gameObject, DestroyTime);
        if(spinRandom)
        {
            spinValue = new Vector3(Random.Range(-1 * spinValue.x, spinValue.x), Random.Range(-1 * spinValue.y, spinValue.y), Random.Range(-1 * spinValue.z, spinValue.z));
        }
        trail.transform.SetParent(null);
    }

	public void Init(Vector3 targ)
    {
        dir = (targ-transform.position).normalized;
        hasTarget = true;
    }

    Vector3 lastPos;
    void Update()
    {
        InvincibleTime -= Time.deltaTime;
        if(hasTarget)
        {
            transform.position += dir * Speed * Time.deltaTime * Speedup;
        }
        trail.transform.position = transform.position;
        Speedup += Time.deltaTime * 0.25f;
        if (spin)
        {
            if(spinOverride != null)
            {
                spinOverride.Rotate(spinValue * Time.deltaTime, Space.Self);
            }
            else
                transform.eulerAngles += spinValue * Time.deltaTime;
        }
            
        if(lookDirection)
        {
            Vector3 mDir = transform.position - lastPos;
            transform.LookAt(transform.position+mDir.normalized);
        }
        lastPos = transform.position;
    }

    [HideInInspector]
    public Health targGate;
    [HideInInspector]
    public GameObject Launcher;
    void OnCollisionEnter(Collision col)
    {
        if (exploded || col.gameObject.GetComponentInParent<Fireball>() != null)
            return;
        bool playerImpact = false;
        if (col.collider.tag == "Player")
            playerImpact = true;
        else
        {
            Transform tRoot = col.transform.root;
            if(tRoot.tag == "Player")
            {
                if (tRoot.GetComponent<PhotonView>() == null || tRoot.GetComponent<PhotonView>().isMine)
                    playerImpact = true;
            }
        }
        if(GateDamage > 0 && targGate != null)
        {
            if(col.collider.GetComponentInParent<Health>() == targGate || Vector3.Distance(col.contacts[0].point, targGate.transform.position) < 4f)
            {
                targGate.SetAttacker(Launcher);
                targGate.takeDamage(GateDamage, false);
            }
        }
        if(playerImpact || InvincibleTime <= 0)
            Explode(col.collider.gameObject.GetComponentInParent<ShieldCollisionBehaviour>() == null, playerImpact);
    }

    public void DeathExplode()
    {
        Explode(false, false);
    }

    public void Explode(bool doDamage, bool forceDamage)
    {
        if (exploded)
            return;
        exploded = true;
        if (baseSrc != null)
            baseSrc.Stop();
        if(Explodes.Length > 0)
        {
            VRAudio.PlayClipAtPoint(Explodes[Random.Range(0, Explodes.Length)], transform.position, explodeVolume* VolumeSettings.GetVolume(AudioType.Effects), 1, Explode3D);
        }
        Destroy(trail.gameObject, 5);
        trail.Stop();
        trail.gameObject.transform.SetParent(null);
        baseCol.enabled = false;
        baseRend.enabled = false;
        OnBreak.Invoke(gameObject);
        if ((Settings.GetBool("ExtraFX") && pieces != null) || LQExplode == null)
        {
            pieces.SetActive(true);
            Destroy(pieces, 10f);
            foreach (var v in GetComponentsInChildren<Rigidbody>())
            {
                v.transform.SetParent(null);
                Destroy(v.gameObject, 10f);
                v.velocity = dir * Speed * Speedup;
                v.AddExplosionForce(300, transform.position + (Vector3.down * 0.2f), 5);
            }
            pieces.transform.SetParent(null);
        }
        else if(LQExplode != null)
        {
            Instantiate(LQExplode, transform.position, Quaternion.identity);
        }
		//pieces.transform.eulerAngles = Vector3.zero;
        if(PlayerHead.instance != null && doDamage)
        {
            var vc = PlayerHead.instance;
            if (forceDamage || Vector3.Distance(vc.transform.position, transform.position) < hitDistance)
            {
                if (vc == null)
                    return;
                //Disable Player for Time
                var pl = vc.GetComponentInParent<PlayerLife>();
                if (pl != null)
                    pl.Die();
            }
        }
		Destroy(gameObject, destroyAfterExplode);
    }
}
