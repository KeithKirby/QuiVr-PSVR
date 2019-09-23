using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipDart : EnemyHitBase
{
    public float Speed;

    List<GameObject> ToHit;
    AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    public override void DoAction(GameObject enemy, bool dummy)
    {
        float range = val;
        ToHit = new List<GameObject>();
        ToHit.Add(enemy);
        foreach(var v in CreatureManager.AllEnemies())
        {
            if(Vector3.Distance(enemy.gameObject.transform.position, v.transform.position) <= range)
            {
                if (pvpmanager.instance != null && pvpmanager.instance.PlayingPVP && v.Team == pvpmanager.instance.myTeam)
                { }
                else
                    ToHit.Add(v.gameObject);
            }
        }
        if (TargetDummy.Dummies != null)
        {
            foreach (var v in TargetDummy.Dummies)
            {
                if (Vector3.Distance(v.transform.position, transform.position) < range)
                    ToHit.Add(v.gameObject);
            }
        }
        StartCoroutine("Zip");
    }

    IEnumerator Zip()
    {
        foreach(var v in ToHit)
        {
            if(v != null)
            {
                Vector3 vtp = v.transform.position;
                if (src != null)
                {
                    src.time = 0;
                    src.Play();
                }
                while (Vector3.Distance(transform.position, vtp) > 0.03f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, vtp, Time.deltaTime * Speed);
                    yield return true;
                }
                if (!dummy && v != null)
                {
                    Immunities im = v.GetComponent<Immunities>();
                    bool immune = false;
                    if (im != null && im.CheckImmune(EffectType))
                        immune = true;
                    if (!immune)
                    {
                        if (v.GetComponent<Health>() != null && !dummy)
                        {
                            if (v.GetComponent<Creature>() != null)
                                v.GetComponent<Creature>().SetTagged();
                            v.GetComponent<Health>().takeDamage(baseEffect.StaticValue);
                        }
                    }
                }
            }            
        }
    }
}
