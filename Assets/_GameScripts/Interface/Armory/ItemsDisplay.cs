using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsDisplay : ArmorDisplay {

    public TypeRequirement ValueCheck;
    public int numReq;

    public override void CheckDisplay()
    {
        if (ValueCheck == TypeRequirement.Armor || ValueCheck == TypeRequirement.Bows)
        {
            if(Armory.ValidFetch && Armory.instance != null)
            {
                if(ValueCheck == TypeRequirement.Armor && Armory.instance.Options.Count >= numReq)
                {
                    display.SetActive(true);
                    return;
                }
                else if(ValueCheck == TypeRequirement.Bows && Armory.instance.GetOptions(ItemType.BowBase).Length >= numReq)
                {
                    display.SetActive(true);
                    return;
                }
            }
        }
        else
        {
            if(Cosmetics.instance != null && Cosmetics.Fetched)
            {
                if (ValueCheck == TypeRequirement.Wings && Cosmetics.WingIDs.Count >= numReq)
                {
                    display.SetActive(true);
                    return;
                }
                else if (ValueCheck == TypeRequirement.Titles && Cosmetics.Titles.Count >= numReq)
                {
                    display.SetActive(true);
                    return;
                }
            }
        }
        display.SetActive(false);
    }

    public enum TypeRequirement
    {
        Armor,
        Bows,
        Wings,
        Titles
    }
}
