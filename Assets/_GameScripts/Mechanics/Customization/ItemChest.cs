using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events; 
public class ItemChest : MonoBehaviour {

    public enum ChestTypeNames
    {
        Enchant,
        Disenchant
    }

    public ChestTypeNames ChestType;
    public ArmorOption CurrentItem;
    public EMOpenCloseMotion PanelMotion;
    public int curIndex;
    public Projector SpellProjector;
    public ItemAction[] Actions;

    public Text[] ActionTitle;
    public Text ActionDetail;

    //public Text Cost;

    ChestCapture capture;

    [Space]
    [Header("ItemUI")]
    public Text ItemTitle;
    public Text EffectTitle;
    public Text EffectDetail;
    public Text EffectStats;
    public GameObject[] AncientDisplay;

    public Text ShardValue;

    private bool _inReroll = false;
    private bool _canUse = false;

    void Awake()
    {
        capture = GetComponentInChildren<ChestCapture>();
    }

    void Start()
    {
        HideAll();
    }

    public void SetItem(ArmorOption item)
    {
        CurrentItem = item;
        if(PanelMotion.motionState == EMBaseMotion.MotionState.Closed || PanelMotion.motionState == EMBaseMotion.MotionState.Closing)
            PanelMotion.Open(true);
        UpdateActions();
        SetupItemDisplay(item);

        var currentCost = ItemDatabase.v.ResourceValues[CurrentItem.rarity];
        if (null != ShardValue)
            ShardValue.text = currentCost.ToString();

        EnchantmentPublisher.PublishItemSelected(this, CurrentItem, _canUse);
    }

    public void Clear()
    {
        CurrentItem = null;
        if(PanelMotion.motionState == EMBaseMotion.MotionState.Open || PanelMotion.motionState == EMBaseMotion.MotionState.Opening)
            PanelMotion.Close(true);
        HideAll();
        capture.Clear();
    }

    public void ItemRemoved()
    {
        EnchantmentPublisher.PublishItemSelected(this, null, _canUse);
        Clear();
    }
        
    void UpdateActions()
    {
        //Action Setup
        _canUse = false;
        ItemAction act = Actions[curIndex];
        SpellProjector.material.color = act.SpellColor;
        if (act.SpellTexture != null)
            SpellProjector.material.mainTexture = act.SpellTexture;
        ActionDetail.text = act.Detail;
        foreach (var t in ActionTitle)
        {
            t.text = act.Name;
        }
        foreach (var v in Actions)
        {
            if(v != act)
            {
                if (v.Components.Length > 0)
                {
                    foreach (var x in v.Components)
                    {
                        x.SetActive(false);
                    }
                }
                v.Disabled.Invoke();
            }
        }
        if (CurrentItem == null)
            return;
        if ((!act.RequireAbility || CanReroll(new EffectInstance(CurrentItem.Effect).EffectID, CurrentItem.rarity)) && CurrentItem.rarity < 5)
        {
            foreach (var x in act.Components)
            {
                x.SetActive(true);
            }
            act.Activated.Invoke();
            _canUse = true;
        }
        else if(CurrentItem.rarity >= 5)
        {
            ActionDetail.text += " <color=red>(Unavailable for Unique Items)</color>";
        }
        else if(act.RequireAbility)
        {
            ActionDetail.text += " <color=red>(Requires an Ability)</color>";
        }

    }

    public static bool CanReroll(int EffectID, int rarity)
    {
        return EffectID > 0 && rarity < 5;
    }

    public ArmorOption GetCurrent()
    {
        return CurrentItem; 
    }

    public void SetupItemDisplay(ArmorOption o)
    {
        foreach (var v in AncientDisplay)
        { v.SetActive(false); }
        if (o == null)
        {
            ItemTitle.text = "Not Equipped";
            EffectTitle.text = "";
            EffectDetail.text = "";
            EffectStats.text = "";
            return;
        }
        ItemTitle.text = o.GetName();
        ItemTitle.color = ItemDatabase.v.RarityColors[o.rarity];
        EffectInstance i = new EffectInstance(o.Effect);
        ItemEffect effect = ItemDatabase.a.GetEffect(i.EffectID);
        if (EffectTitle != null)
        {
            EffectTitle.text = effect.GetEffectName();
            EffectDetail.text = effect.GetDetailInfo(i.EffectValue);
            EffectStats.text = effect.GetStatInfo(i.EffectValue, o.ancient);
            for (int x = 0; x < AncientDisplay.Length; x++)
            {
                if (o.ancient > x)
                    AncientDisplay[x].SetActive(true);
            }
        }
    }

    public void HideAll()
    {
        ItemAction act = Actions[curIndex];
        SpellProjector.material.color = act.SpellColor;
        if (act.SpellTexture != null)
            SpellProjector.material.mainTexture = act.SpellTexture;
        foreach (var v in Actions)
        {
            if (v.Components.Length > 0)
            {
                foreach (var x in v.Components)
                {
                    x.SetActive(false);
                }
            }
        }
    }

    public void TryDisenchant()
    {
        if (_canUse)
        {
            ArmorOption item = CurrentItem;
            if (item != null && Armory.instance.OwnsItem(item))
            {
                int cost = ItemDatabase.v.ResourceValues[CurrentItem.rarity];
                //Wearing Item
                if (Armory.GetEquipped(item.Type).Duplicate(item))
                {
                    foreach (var v in Armory.instance.GetOptions(item.Type))
                    {
                        if (!v.Duplicate(item))
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
                    MoteParticles.instance.SpawnMotes(transform.position, cost);
                EnchantmentPublisher.PublishShardsChanged();
                EnchantmentPublisher.PublishItemDisenchanted(item);
            }
            EnchantmentPublisher.PublishItemSelected(this, null, _canUse);
            Clear();
        }
    }

    public void Reroll()
    {
        ArmorOption item = CurrentItem;
        if (item == null || Armory.instance == null)
            return;
        else if (item.ancient == 2)
        {
            Notification.Notify(new Note("Oops!", "Can't reroll a Primordial item"));
            return;
        }
        int shardsHeld = Armory.instance.GetResource();
        int shardsNeeded = ItemDatabase.v.ResourceValues[item.rarity];
        if (shardsHeld >= shardsNeeded)
        {
            ArmorOption newItem = ItemDatabase.Reroll(item);
            Armory.instance.EquipItem(newItem);
            Armory.instance.ReplaceItem(newItem, RerollCallback);
            //OnReroll.Invoke(newItem);
            SetupItemDisplay(newItem);
            //Cost.text = "Reforging...";
            EnchantmentPublisher.PublishShardsChanged();
        }
        else
        {
            Debug.LogFormat("Insufficient shards, held({0}) needed({1})", shardsHeld, shardsNeeded);
        }
    }

    public void RerollCallback(bool success)
    {
        _inReroll = false;
        ItemSelectionBase.ForceUpdate();
        UpdateItem();
    }

    public void UpdateItem()
    {
        ArmorOption item = CurrentItem;
        //if (item != null && ItemDatabase.v != null)
            //Cost.text = ItemDatabase.v.ResourceValues[item.rarity] + "";
        Color c = new Color(0.85f, 0.85f, 1f, 1f);
        if (item != null && ItemDatabase.v != null && Armory.instance.GetResource() < ItemDatabase.v.ResourceValues[item.rarity])
        {
            c = Color.grey;
            //var boxCollider = GetComponent<BoxCollider>();
            //boxCollider.enabled = false;
        }
        //Cost.color = c;
    }

    public void OnEnable()
    {
        EnchantmentPublisher.ItemSelected += EnchantmentPublisher_ItemSelected;
    }

    public void OnDisable()
    {
        EnchantmentPublisher.ItemSelected -= EnchantmentPublisher_ItemSelected;
    }

    private void EnchantmentPublisher_ItemSelected(ItemChest chest, ArmorOption item, bool canUse)
    {
        if(chest != this && item != null) // A valid item has been placed in a different chest, remove item in this chest if one exists
        {
            Clear();
        }
    }
}

[System.Serializable]
public class ItemAction
{
    public string Name;
    [TextArea]
    public string Detail;
    public Color SpellColor = Color.white;
    public Texture SpellTexture;
    public bool RequireAbility;
    public GameObject[] Components;
    public UnityEvent Activated;
    public UnityEvent Disabled;
}

//Allows rerolling ability values in exchange for motes.
