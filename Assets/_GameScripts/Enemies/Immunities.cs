using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immunities : MonoBehaviour {

    public EffectClassification[] ImmuneTo;

    public bool CheckImmune(EffectClassification ec)
    {
        foreach(var v in ImmuneTo)
        {
            if (v == ec)
                return true;
        }
        return false;
    }
}

public enum EffectClassification
{
    None,
    Arrow,
    Slow,
    Acid,
    Fire,
    Explosion,
    Electricity,
    Freeze,
    Death,
    Force
}
