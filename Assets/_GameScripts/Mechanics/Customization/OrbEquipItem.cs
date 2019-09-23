using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.Events;

public class OrbEquipItem : MonoBehaviour {

    public ItemEvent OnActivate;
    public ParticleSystem ActivateLoc;

    void OnTriggerEnter(Collider col)
    {
        ItemOrbDisplay orb = col.GetComponent<ItemOrbDisplay>();
        if (orb != null)
        {
            if(ActivateLoc != null)
            {
                ActivateLoc.transform.position = col.transform.position;
                ActivateLoc.Play();
            }
            Armory.instance.EquipItem(orb.item);
            OnActivate.Invoke(orb.item);
            Destroy(orb.gameObject);
        }
    }
}
