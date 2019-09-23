using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {

    [HideInInspector]
    public bool dummy;
    [HideInInspector]
    public int effectID;
    [HideInInspector]
    public float val;
    [HideInInspector]
    public ItemEffect baseEffect;

    public virtual void Setup(bool fake, int efID, float v)
    {
        dummy = fake;
        effectID = efID;
        baseEffect = ItemDatabase.GetEffect(effectID);
        if (baseEffect.randomType == RandomType.Recharge)
            val = baseEffect.VariableValue - v;
        else
            val = baseEffect.VariableValue + v;
    }

    public virtual void Setup(bool fake, float v)
    {
        dummy = fake;
        val = v;
    }
}
