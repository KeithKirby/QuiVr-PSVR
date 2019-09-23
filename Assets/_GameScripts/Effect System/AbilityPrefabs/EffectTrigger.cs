using UnityEngine;
using System.Collections;

public class EffectTrigger : MonoBehaviour {

    public bool DestroyOnTrigger;
    public bool CreateAtTarget;

    public bool UseID;
    public int EffectID;
    public GameObject EffectOverride;

    public void TriggerEffect(GameObject target, bool dummy)
    {
        GameObject prefab = EffectOverride;
        Vector3 pos = transform.position;
        if (CreateAtTarget)
            pos = target.transform.position;
        if (UseID && prefab == null)
        {
            AbilityManager.instance.UseAbility(EffectID, pos, dummy);
        }
        else if(prefab != null)
        {
            GameObject inst = (GameObject)Instantiate(prefab, pos, transform.rotation);
            AbilitySetup setup = inst.GetComponent<AbilitySetup>();
            if (setup != null)
                setup.Setup(EffectID, 0, dummy);
        }
        if (DestroyOnTrigger)
            Destroy(gameObject);
    }
}
