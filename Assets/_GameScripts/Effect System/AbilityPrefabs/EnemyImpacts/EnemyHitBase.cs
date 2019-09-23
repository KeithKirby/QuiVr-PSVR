using UnityEngine;
using System.Collections;

public class EnemyHitBase : Effect {

    public EffectClassification EffectType;

    void Awake()
    {
        Destroy(gameObject, 5f);
    }

    public int testID;
    public float testVal;
    [AdvancedInspector.Inspect]
    public void DevSetup()
    {
        Setup(false, testID, testVal);
    }

	public override void Setup(bool fake, int efID, float value)
    {
        base.Setup(fake, efID, value);
        bool didAction = false;
        if(CreatureManager.EnemyNum() > 0 && CreatureManager.AllEnemies()[0] != null)
        {
            GameObject closestEnemy = CreatureManager.AllEnemies()[0].gameObject;
            float dist = float.MaxValue;
            foreach(var v in CreatureManager.AllEnemies())
            {
                if(v != null)
                {
                    Health h = v.GetComponent<Health>();
                    if (Vector3.Distance(v.transform.position, transform.position) < dist)
                    {
                        closestEnemy = v.gameObject;
                        dist = Vector3.Distance(v.transform.position, transform.position);
                    }
                }
            }
            if (dist < 5) //Sanity Check
            {
                Immunities im = closestEnemy.GetComponent<Immunities>();
                if (im != null)
                {
                    if (im.CheckImmune(EffectType))
                    {
                        CombatText.ShowText("Immune", closestEnemy.transform.position + (Vector3.up * 2), Color.white);
                        return;
                    }
                }
                DoAction(closestEnemy, dummy);
                didAction = true;
            }  
        }
        if(!didAction && TargetDummy.Dummies != null && TargetDummy.Dummies[0] != null)
        {
            GameObject closestEnemy = TargetDummy.Dummies[0].gameObject;
            float dist = float.MaxValue;
            foreach (var v in TargetDummy.Dummies)
            {
                if (v != null)
                {
                    if (Vector3.Distance(v.transform.position, transform.position) < dist)
                    {
                        closestEnemy = v.gameObject;
                        dist = Vector3.Distance(v.transform.position, transform.position);
                    }
                }
            }
            if (dist < 5) //Sanity Check
            {
                DoAction(closestEnemy, dummy);
            }
        }
    }

    public float GetValue()
    {
        if(baseEffect.randomType == RandomType.Damage || baseEffect.randomType == RandomType.Healing)
            return val;
        return baseEffect.StaticValue;
    }

    public virtual void DoAction(GameObject enemy, bool dummy)
    {}
}
