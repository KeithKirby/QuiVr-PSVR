using UnityEngine;
using System.Collections;

public class AcidPool : AoEEffect {

    public float OverrideDamage = 0;

    public override void Setup(bool fake, int efID, float v)
    {
        base.Setup(fake, efID, v);
        if(OverrideDamage <= 0)
        {
            if (baseEffect.randomType == RandomType.Damage)
                OverrideDamage = val;
            else
                OverrideDamage = baseEffect.StaticValue;
        }
    }

    public override void Tick(GameObject obj)
    {
        if (obj == null)
            return;
        Health h = obj.GetComponent<Health>();
        if (h == null)
            h = obj.GetComponentInChildren<Health>();
        if(h != null && !h.isDead() && !dummy)
            h.takeDamage(OverrideDamage);
    }

    public override void TickFirst(GameObject obj)
    {
        Health h = obj.GetComponent<Health>();
        if (h == null)
            h = obj.GetComponentInChildren<Health>();
        if (h != null && !h.isDead() && !dummy)
        {
            if(obj.GetComponent<Creature>() != null)
            {
                obj.GetComponent<Creature>().SetTagged();
            }
            h.takeDamage(OverrideDamage);
        } 
    }
}
