using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(ItemChanger))]
public class PlayerArmor : MonoBehaviour {

    public static PlayerArmor instance;
    public List<EffectInstance> CurrentEffects;
    ItemChanger changer;

    IEnumerator Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
            
        changer = GetComponent<ItemChanger>();
        while(Armory.instance == null || !Armory.instance.updated)
        {
            yield return true;
        }
        SetupArmor();
    }

    public void SetupArmor()
    {
        foreach(var v in Armory.CurrentOutfitList())
        {
            Equip(v);
        }
        if (PhotonNetwork.inRoom)
            PlayerSync.myInstance.SendArmor(Armory.CurrentOutfit(), PhotonTargets.Others);
        CurrentEffects = Armory.ArmorEffects();
    }

    public void ForceRefresh()
    {
        Equip(Armory.currentOutfit.Gloves);
        Equip(Armory.currentOutfit.Chest);
        Equip(Armory.currentOutfit.Helmet);
        Equip(Armory.currentOutfit.Arrow);
        Equip(Armory.currentOutfit.Quiver);
    }

    public void Equip(ArmorOption item)
    {
        if (PhotonNetwork.inRoom)
            GetComponentInParent<PlayerSync>().EquipItem(item.ToString());
        CurrentEffects = Armory.ArmorEffects();

        if (item.Type == ItemType.Helmet)
            changer.ChangeHelmet(item, true);
        if (item.Type == ItemType.ChestArmor)
            changer.ChangeChest(item, true);
        if (item.Type == ItemType.Quiver)
            changer.ChangeQuiver(item);
        if (item.Type == ItemType.BowBase)
            changer.ChangeBow(item);
        if (item.Type == ItemType.Gloves)
            changer.ChangeGloves(item, false);
        if (item.Type == ItemType.Arrow)
            changer.ChangeArrow(item);
    }

    public void RemoveEffect(EffectInstance e)
    {
        CurrentEffects.Remove(e);
    }
}
