using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChainLightning : EnemyHitBase {

    public GameObject LightningPrefab;
    public float overrideDist;
    public float overrideDmg;

    public override void DoAction(GameObject enemy, bool dummy)
    {
        List<Creature> enems = CreatureManager.AllEnemies();
        List<GameObject> CloseEnemies = new List<GameObject>();
        float dist = 15f;
        float dmg = GetValue();
        if (overrideDmg > 0) dmg = overrideDmg;
        if (overrideDist > 0) dist = overrideDist;
        foreach (var v in enems)
        {
            if(v != enemy && Vector3.Distance(v.transform.position, transform.position) < dist)
            {
                Immunities im = v.GetComponent<Immunities>();
                bool immune = false;
                if (im != null)
                {
                    if (im.CheckImmune(EffectType))
                        immune = true;
                }
                if (pvpmanager.instance != null && pvpmanager.instance.PlayingPVP && v.Team == pvpmanager.instance.myTeam)
                    immune = true;
                if(!immune)
                    CloseEnemies.Add(v.gameObject);
            }
        }
        if(TargetDummy.Dummies != null)
        {
            foreach (var v in TargetDummy.Dummies)
            {
                if (Vector3.Distance(v.transform.position, transform.position) < dist)
                    CloseEnemies.Add(v.gameObject);
            }
        }
        foreach(var v in CloseEnemies)
        {
            GameObject lp = (GameObject)Instantiate(LightningPrefab, transform);
            lp.GetComponent<LightningBolt>().Setup(v, enemy);
            if (v.GetComponent<Health>() != null && !dummy)
            {
                if(v.GetComponent<Creature>() != null)
                    v.GetComponent<Creature>().SetTagged();
                v.GetComponent<Health>().takeDamage(dmg);
                dmg = Mathf.Max(dmg * 0.75f, 3f);
            }
        }
        if(!dummy)
            enemy.GetComponent<Health>().takeDamage(dmg);
        base.DoAction(enemy, dummy);
    }
}
