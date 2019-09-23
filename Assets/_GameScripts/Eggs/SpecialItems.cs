using UnityEngine;
using System.Collections;

public class SpecialItems : MonoBehaviour {

	public static bool HasItem(ArmorOption item)
    {
        if(IsReady())
        {
            foreach(var v in Armory.instance.GetOptions())
            {
                if (v.Duplicate(item))
                    return true;
            }
        }
        return false;
    }

    public static bool IsReady()
    {
        return (Armory.instance != null && Armory.ValidFetch);
    }

    public static void GiveItem(ArmorOption item, bool ShowDisplay=false)
    {
        if(IsReady() && !HasItem(item))
        {
            Armory.instance.AddItem(item);
            if(ShowDisplay && ItemReward.instance != null)
            {
                ItemReward.instance.Reset();
                ItemReward.instance.SetupReward(item);
            }
        }
    }
}
