using UnityEngine;
using System.Collections;

public class LifeLeech : Effect {

    public float HealAmt;

    void Awake()
    {
        if (GameBase.instance != null && GameBase.instance.CurrentTarget != null)
        {
            GetComponent<EffectSettings>().Target = GameBase.instance.CurrentTarget.gameObject;
        }
        else if(pvpmanager.instance != null)
        {
            Health hp = pvpmanager.instance.GetMyGate();
            if (hp != null)
                GetComponent<EffectSettings>().Target = hp.gameObject;
        }

    }

    IEnumerator Start()
    {
        yield return true;
        HealAmt = val;
        if(!dummy && NetworkEffects.instance != null)//GameBase.instance != null && GameBase.instance.CurrentTarget != null)
        {
            //GameBase.instance.CurrentTarget.takeDamage(-1 * val);
            NetworkEffects.instance.HealCurTarget(val);
            Statistics.AddCurrent("gate_healed", (int)val);
        }
    }

}
