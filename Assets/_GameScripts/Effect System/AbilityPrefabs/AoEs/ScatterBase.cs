using UnityEngine;
using System.Collections;

public class ScatterBase : Effect {

    void Start()
    {
        Destroy(gameObject, 10f);
    }

    public override void Setup(bool fake, int efID, float v)
    {
        base.Setup(fake, efID, v);
        foreach(var s in GetComponentsInChildren<ScatterArrow>())
        {
            if (fake)
                s.Damage = 0;
            else if (baseEffect.randomType == RandomType.Damage)
                s.Damage = v;
            else
                s.Damage = baseEffect.StaticValue;
        }
    }
}
