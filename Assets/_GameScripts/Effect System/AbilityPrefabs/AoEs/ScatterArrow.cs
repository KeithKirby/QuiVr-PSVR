using UnityEngine;
using System.Collections;

public class ScatterArrow : MonoBehaviour {

    public float StartSpeed;
    public float GravityEffect;
    public float Lifetime;
    public int MaxBounces;
    public float Damage;
    public bool StartTrackEnemy;

    AudioSource src;
    public AudioClip HitClip;

    public GameObject BaseModel;

    bool ended;

    float speed;
    Vector3 velDir;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        speed = StartSpeed*Time.deltaTime;
    }
    bool firstFrame;

    void Start()
    {
        CheckHit(false);
        CheckLook();
        Invoke("End", Lifetime);
    }

    void Update()
    {
        if(!ended && firstFrame)
        {
            CheckHit(true);
            Move();
        }
        firstFrame = true;
    }

    void Move()
    {
        transform.position += transform.forward * StartSpeed * Time.deltaTime;
        transform.position -= Vector3.up * GravityEffect * Time.deltaTime;
        Vector3 p = transform.position;
        p += transform.forward * StartSpeed * Time.deltaTime;
        p -= Vector3.up * GravityEffect * Time.deltaTime;

        speed = Vector3.Distance(p, transform.position);
        velDir = (p-transform.position).normalized;
        transform.LookAt(transform.position + velDir);
    }

    void CheckHit(bool AllowKill)
    {
        RaycastHit hit;
        float Dist = speed + 0.2f;
        Vector3 lastForward = transform.forward;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Dist))
        {
            GameObject o = hit.collider.gameObject;
            Creature c = o.GetComponent<Creature>();
            ArrowImpact ai = o.GetComponent<ArrowImpact>();
            if (c == null)
                c = o.GetComponentInParent<Creature>();
            if (c != null && AllowKill)
            {
                Health h = c.GetComponent<Health>();
                src.clip = HitClip;
                src.Play();
                if (!h.isDead() && Damage > 0)
                {
                    c.SetTagged();
                    h.takeDamage(Damage);
                }
                End();
            }
            else if(ai != null && o.GetComponent<Teleporter>() != null && ai.GetComponent<Teleporter>() != null)
            {
                if(!ai.RequiresMine || Damage > 0)
                    ai.OnImpact(new ArrowCollision(hit.point, Damage, hit.point, speed, o, false, hit.normal, true, false, 0, gameObject, null));
            }
            else
            {
                Reflect(hit.normal);
            }
        }
    }

    void Reflect(Vector3 norm)
    {
        src.Play();
        transform.forward = Vector3.Reflect(transform.forward, norm);
    }

    void CheckLook()
    {
        if(Random.Range(0, 100) < 80)
        {
            if(CreatureManager.EnemyNum() > 0)
            {
                Vector3 offset = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
                transform.LookAt(CreatureManager.AllEnemies()[Random.Range(0, CreatureManager.EnemyNum())].transform.position + offset);
            }
        }
    }

    void End()
    {
        if(!ended)
        {
            ended = true;
            BaseModel.SetActive(false);
            Destroy(gameObject, 2.5f);
        }
    }

}
