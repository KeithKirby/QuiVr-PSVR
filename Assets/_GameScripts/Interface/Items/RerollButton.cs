using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class RerollButton : MonoBehaviour {

    public Text Cost;
    bool p2;
    public GameObject confirm;
    public GameObject decline;
    ItemChangeUI ic;

    ItemPedestal ped;

    ArmorOption item;

    public ItemEvent OnReroll;

    bool inReroll;

    void Awake()
    {
        ic = GetComponentInParent<ItemChangeUI>();
        ped = GetComponentInParent<ItemPedestal>();
        if (ic != null)
            OnReroll.AddListener(ic.Setup);
        if (ped != null)
            OnReroll.AddListener(ped.SetItem);
    }

    public void Reset()
    {
        p2 = false;
        confirm.SetActive(false);
        decline.SetActive(false);
        item = null;
        if (ic != null && ic.GetCurrent() != null)
            item = ic.GetCurrent();
        else if (ped != null && ped.GetCurrent() != null)
            item = ped.GetCurrent();
        if (item != null && ItemDatabase.v != null)
            Cost.text = ItemDatabase.v.ResourceValues[item.rarity] + "";
        Color c = new Color(0.85f, 0.85f, 1f, 1f);
        if (item != null && ItemDatabase.v != null && Armory.instance.GetResource() < ItemDatabase.v.ResourceValues[item.rarity])
        {
            c = Color.grey;
            GetComponent<BoxCollider>().enabled = false;
        }
        Cost.color = c;
        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().color = c;
        else if (GetComponent<Image>() != null)
            GetComponent<Image>().color = c;
    }

    public void Click()
    {
        if(!inReroll)
        {
            Cost.text = "Sure?";
            p2 = true;
            confirm.SetActive(true);
            decline.SetActive(true);
        }
    }

    public void Confirm()
    {
        if(p2)
        {
            Reroll();
        }

    }

    public void Decline()
    {
        if(p2)
            Reset();
    }

    public void Reroll()
    {
        if (item == null || Armory.instance == null)
            return;
        else if(item.ancient == 2)
        {
            Notification.Notify(new Note("Oops!", "Can't reroll a Primordial item"));
            return;
        }
        if (Armory.instance.GetResource() >= ItemDatabase.v.ResourceValues[item.rarity])
        {
            inReroll = true;
            ArmorOption newItem = ItemDatabase.Reroll(item);
            Armory.instance.EquipItem(newItem);
            Armory.instance.ReplaceItem(newItem, RerollCallback);
            OnReroll.Invoke(newItem);
            confirm.SetActive(false);
            decline.SetActive(false);
            Cost.text = "Reforging...";
        }
    }

    public void RerollCallback(bool success)
    {
        inReroll = false;
        ItemSelectionBase.ForceUpdate();
        Reset();
    }
}
