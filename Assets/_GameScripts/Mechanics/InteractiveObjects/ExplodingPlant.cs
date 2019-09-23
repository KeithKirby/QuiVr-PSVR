using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingPlant : MonoBehaviour {

    public Transform PlantObject;
    public ParticleSystem ExplodeEffect;
    public float RespawnDelay = 5f;
    public float RespawnTime = 1.5f;
    public AnimationCurve GrowbackCurve;

    public float ExpRadius;
    public float Damage;
    public EffectClassification DamageType;
    public float ExpForce;

    PlayRandomClip prc;
    AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        prc = GetComponent<PlayRandomClip>();
    }

    [AdvancedInspector.Inspect]
    public void Explode()
    {
        PlantObject.localScale = Vector3.one * 0.0025f;
        if (prc != null)
            prc.Play();
        else if (src != null)
            src.Play();
        ExplodeEffect.Play();
        StopAllCoroutines();
        StartCoroutine("Respawn");
        if(ExpRadius > 0.1f)
        {
            ExplodeForce();
        }
    }

    void ExplodeForce()
    {
        //Damage
        if(Damage > 0)
        {
            List<GameObject> enms = new List<GameObject>();
            if (GameBase.instance != null)
            {
                foreach (var v in CreatureManager.AllEnemies())
                {
                    if (v != null && Vector3.Distance(v.transform.position, transform.position) < ExpRadius)
                    {
                        Immunities im = v.GetComponent<Immunities>();
                        bool immune = false;
                        if (im != null)
                        {
                            if (im.CheckImmune(DamageType))
                                immune = true;
                        }
                        if (!immune)
                        {
                            v.SetTagged();
                            v.GetComponent<Health>().takeDamage(Damage);
                        }
                        else
                            CombatText.ShowText("Immune", v.transform.position + (Vector3.up * 2), Color.white);
                    }
                }
            }
        }
        //Physics
        Collider[] objects = UnityEngine.Physics.OverlapSphere(transform.position, ExpRadius * 1.5f);
        foreach (Collider h in objects)
        {
            Rigidbody r = h.GetComponent<Rigidbody>();
            if (r != null && !r.isKinematic)
            {
                r.AddExplosionForce(ExpForce, transform.position, ExpRadius * 2);
            }
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(RespawnDelay);
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime / RespawnTime;
            PlantObject.localScale = Vector3.one * GrowbackCurve.Evaluate(t);
            yield return true;
        }
    }

}
