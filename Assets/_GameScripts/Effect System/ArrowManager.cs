using UnityEngine;
using System.Collections;
using VRTK;

public class ArrowManager : MonoBehaviour {

    public static ArrowManager instance;

    void Awake()
    {
        instance = this;
    }

    public GameObject GetNockedArrow()
    {
        if (BowAim.instance != null)
            return BowAim.instance.GetArrow();
        return null;
    }

    public GameObject GetHeldArrow()
    {
        if(SteamVR_ControllerManager.freeHand != null)
        {
            GameObject obj = SteamVR_ControllerManager.freeHand.GetComponent<VRTK_InteractGrab>().GetGrabbedObject();
            if(obj != null && obj.tag == "Arrow")
            {
                return obj.GetComponentInChildren<Arrow>().gameObject;
            }
        }
        return null;
    }

    public void AddAbility(int id, float val)
    {
        GameObject arrow = GetNockedArrow();
        if (arrow == null)
            arrow = GetHeldArrow();

        if (arrow != null)
        {
            arrow.GetComponent<ArrowEffects>().AddEffect(id, val);
        }
    }
}
