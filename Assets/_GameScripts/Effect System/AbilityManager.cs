using UnityEngine;
using System.Collections;

public class AbilityManager : MonoBehaviour {

    public static AbilityManager instance;

    void Awake()
    {
        instance = this;
    }

    public void UseAbility(int id, Vector3 pos, bool dummy=false, float value=0)
    {
        if (!dummy)
        {
            if (PhotonNetwork.inRoom)
            {
                if(PlayerSync.myInstance != null)
                    PlayerSync.myInstance.UseAbility(id, pos, Vector3.forward, value);
                else if(MobilePlayerSync.myInstance != null)
                    MobilePlayerSync.myInstance.UseAbility(id, pos, Vector3.forward, value);
            }
                
        }
        if (ItemDatabase.GetEffect(id).EffectActivation != null)
        {
            GameObject effect = (GameObject)Instantiate(ItemDatabase.GetEffect(id).EffectActivation, pos, Quaternion.identity);
            if (effect.GetComponent<AbilitySetup>() != null)
            {
                effect.GetComponent<AbilitySetup>().Setup(id, value, dummy);
            }

        }
    }

    public void UseAbility(int id, Vector3 pos, Vector3 lookDir, bool dummy = false, float value = 0)
    {
        if (!dummy)
        {
            if (PhotonNetwork.inRoom)
                PlayerSync.myInstance.UseAbility(id, pos, lookDir, value);
        }
        if (ItemDatabase.GetEffect(id).EffectActivation != null)
        {
            GameObject effect = (GameObject)Instantiate(ItemDatabase.GetEffect(id).EffectActivation, pos, Quaternion.identity);
            effect.transform.LookAt(effect.transform.position + lookDir);
            if (effect.GetComponent<AbilitySetup>() != null)
            {
                effect.GetComponent<AbilitySetup>().Setup(id, value, dummy);
            }

        }
    }
}
