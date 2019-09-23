using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Abilities", menuName = "AbilityData", order = 1)]
public class AbilityValues : ScriptableObject {

    public ItemEffect[] Effects;

    public ItemEffect GetEffect(int effectID)
    {
        //FastPath
        if (effectID >= 0 && effectID < Effects.Length && Effects[effectID].EffectID == effectID)
            return Effects[effectID];
        for(int i=0; i<Effects.Length; i++)
        {
            if (Effects[i].EffectID == effectID)
                return Effects[i];
        }
        return Effects[0];
    }
}
