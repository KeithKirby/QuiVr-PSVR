using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingDamagePool : SlowingPool {

    public float OverrideDamage = 0;

    public override void Setup(bool fake, int efID, float v)
    {
        base.Setup(fake, efID, v);
        if (OverrideDamage <= 0)
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
        if (h != null && !h.isDead() && !dummy)
            h.takeDamage(OverrideDamage);
    }
}
