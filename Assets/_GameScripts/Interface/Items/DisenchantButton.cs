using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisenchantButton : MonoBehaviour {

    ItemPedestal pdstl;
    public Text label;

    public GameObject[] Buttons;

    void Awake()
    {
        pdstl = GetComponentInParent<ItemPedestal>();
    }

    int currentCost;

    void OnEnable()
    {
        if (pdstl != null && pdstl.CurrentItem != null && ItemDatabase.v != null)
        {
            currentCost = ItemDatabase.v.ResourceValues[pdstl.CurrentItem.rarity];
            Decline();
        }
    }

    public void OpenOptions()
    {
        label.text = "Sure?";
        foreach(var v in Buttons)
        {
            v.SetActive(true);
        }
    }

    public void Decline()
    {
        label.text = currentCost + "";
        foreach(var v in Buttons)
        {
            v.SetActive(false);
        }
    }

    public void TryDisenchant()
    {
        if(pdstl != null)
        {
            ArmorOption item = pdstl.CurrentItem;
            if (item != null && Armory.instance.OwnsItem(item))
            {
                int cost = ItemDatabase.v.ResourceValues[pdstl.CurrentItem.rarity];
                //Wearing Item
                if (Armory.GetEquipped(item.Type).Duplicate(item)) 
                {
                    foreach(var v in Armory.instance.GetOptions(item.Type))
                    {
                        if(!v.Duplicate(item))
                        {
                            Armory.instance.EquipItem(v);
                            ItemSelectionBase.ForceUpdate();
                            break;
                        }
                    }
                }
                Armory.instance.RemoveItem(item);
                Armory.instance.GiveResource(cost);
                if (MoteParticles.instance != null)
                    MoteParticles.instance.SpawnMotes(pdstl.transform.position, cost);
            }
            pdstl.Clear();
        }
    }
}
