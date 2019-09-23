using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(ItemChanger))]
public class DummyArmor : MonoBehaviour {

    public Outfit currentOutfit;
    ItemChanger changer;

    void Start()
    {
        changer = GetComponent<ItemChanger>();
    }

	public void SetupArmor(string s)
    {
        List<ArmorOption> Armor = ArmorOption.ItemList(s);
        foreach(var v in Armor)
        {
            EquipItem(v);
        }
    }

    public void EquipItem(string s)
    {
        ArmorOption o = new ArmorOption(s);
        EquipItem(o);
    }

    public void EquipItem(ArmorOption item)
    {
        if (changer == null)
            changer = GetComponent<ItemChanger>();
        if (item == null)
            return;
        ItemType t = item.Type;
        if (t == ItemType.Arrow)
        {
            currentOutfit.Arrow = item;
            changer.ChangeArrow(item);
        } 
        else if (t == ItemType.Helmet)
        {
            currentOutfit.Helmet = item;
            changer.ChangeHelmet(item, false);
        }
        else if (t == ItemType.Gloves)
        {
            currentOutfit.Gloves = item;
            changer.ChangeGloves(item, true);
        } 
        else if (t == ItemType.ChestArmor)
        {
            currentOutfit.Chest = item;
            changer.ChangeChest(item, false);
        }
        else if (t == ItemType.Quiver)
        {
            currentOutfit.Quiver = item;
            changer.ChangeQuiver(item);
        }
        else if (t == ItemType.Gem)
            currentOutfit.Gem = item;
        else if (t == ItemType.BowBase)
        {
            currentOutfit.BowBase = item;
            changer.ChangeBow(item);
        }
        else if (t == ItemType.BowDetail)
            currentOutfit.BowBase = item;
    }

    public List<ArmorOption> GetOutfit()
    {
        List<ArmorOption> o = new List<ArmorOption>();
        o.Add(currentOutfit.BowBase);
        o.Add(currentOutfit.Arrow);
        o.Add(currentOutfit.Quiver);
        o.Add(currentOutfit.Chest);
        o.Add(currentOutfit.Gloves);
        o.Add(currentOutfit.Helmet);
        return o;
    }
}
