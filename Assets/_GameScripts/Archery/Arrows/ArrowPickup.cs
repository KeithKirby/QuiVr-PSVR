using UnityEngine;
using System.Collections;
using VRTK;
using System.Collections.Generic;
public class ArrowPickup : MonoBehaviour {

    Arrow a;

    void Awake()
    {
        a = GetComponent<Arrow>();
        GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += OnUse;
    }

	public void OnUse(object o, InteractableObjectEventArgs e)
    {
        if(a.WasFired() && Quiver.instance != null)
        {
            ArrowEffects eff = GetComponent<ArrowEffects>();
            List<int> EffectIDs = new List<int>();
            if(eff != null)
            {
                foreach(var v in eff.effects)
                {
                    ItemEffect eft = ItemDatabase.GetEffect(v.EffectID);
                    //Test for used up
                    if(eft.activation != ActivationType.HitAnything && eft.activation != ActivationType.HitObject && eft.activation != ActivationType.Fire)
                    {
                        if(!EffectIDs.Contains(v.EffectID))
                            EffectIDs.Add(v.EffectID);
                    }
                }
            }
            Quiver.instance.TryGiveArrow(EffectIDs.ToArray());
            if (PlayerSync.myInstance != null)
                PlayerSync.myInstance.DestroyFiredArrow(transform.position);
            Destroy(gameObject);
        }
    }
}
