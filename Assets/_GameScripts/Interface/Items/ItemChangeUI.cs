using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
[RequireComponent (typeof(EMOpenCloseMotion))]
public class ItemChangeUI : MonoBehaviour {

    public Text Title;
    public Text EffectTitle;
    public Text EffectDetail;
    public Text EffectStats;
    public Text ItemNum;
    public Text Artist;
    public GameObject[] AncientDisplays;
    public Text[] ExtraTitles;
    public Text[] ExtraNums;

    public ItemType type;
    EMOpenCloseMotion motion;
    ArmorOption curItem;
    //RerollButton reroll;
    GameObject NextButton;
    GameObject PrevButton;
    int curIndex;
    int opNum;

    bool Shown;

    ItemSelectionBase items;

    public bool ignoreWorn;

    void Awake()
    {
        items = GetComponentInParent<ItemSelectionBase>();
        //reroll = GetComponentInChildren<RerollButton>();
        motion = GetComponentInChildren<EMOpenCloseMotion>();
        foreach(var v in GetComponentsInChildren<BoxCollider>())
        {
            if (v.name == "NextButton")
                NextButton = v.gameObject;
            else if (v.name == "PrevButton")
                PrevButton = v.gameObject;
        }
        if (NextButton != null)
            NextButton.SetActive(false);
        if (PrevButton != null)
            PrevButton.SetActive(false);
        if(!ignoreWorn)
            StartCoroutine("SetupInit");
    }

    IEnumerator SetupInit()
    {
        //reroll.gameObject.SetActive(false);
        while (Armory.instance == null || !Armory.instance.updated)
        {
            yield return true;
        }
        yield return new WaitForSeconds(0.5f);
        ArmorOption[] opts = Armory.instance.GetOptions(type);
        opNum = opts.Length;
        Setup(Armory.instance.Worn(type));
        curIndex = Armory.instance.CurrentIndex(type);
    }

    public void Show()
    {
        if(null  == Armory.instance)
        {
            Debug.Log("Armory not initialised, skipping.");
            return;
        }

        int optLength = Armory.instance.GetOptions(type).Length;
        if (optLength < 1)
            return;
        if (motion && (motion.motionState == EMBaseMotion.MotionState.Closed || motion.motionState == EMBaseMotion.MotionState.Closing))
        {
            if(optLength < 2)
            {
                if (NextButton != null)
                    NextButton.SetActive(false);
                if (PrevButton != null)
                    PrevButton.SetActive(false);
            }
            else
            {
                if (NextButton != null)
                    NextButton.SetActive(true);
                if (PrevButton != null)
                    PrevButton.SetActive(true);
            }
            motion.Open(true);
        } 
    }

    public void Hide()
    {
        if (motion == null)
            motion = GetComponentInChildren<EMOpenCloseMotion>();
        else if(motion.motionState == EMBaseMotion.MotionState.Opening || motion.motionState == EMBaseMotion.MotionState.Open)
            motion.Close(true);
    }

    public ArmorOption GetCurrent()
    {
        return curItem;
    }

    public void Setup(ArmorOption o)
    {
        foreach (var v in AncientDisplays)
        { v.SetActive(false); }
        if (o == null)
        {
            Title.text = I2.Loc.ScriptLocalization.Get("NotEquipped");
            EffectTitle.text = "";
            EffectDetail.text = "";
            EffectStats.text = "";
            ItemNum.text = "0/0";
            foreach(var v in ExtraTitles)
            { v.text = Title.text; }
            return;
        }
        curItem = o;
        Title.text = o.GetName();
        for(int x=0; x<AncientDisplays.Length; x++)
        {
            if (curItem.ancient > x)
                AncientDisplays[x].SetActive(true);
        }
        Title.color = ItemDatabase.v.RarityColors[o.rarity];
        foreach (var v in ExtraTitles)
        { v.text = Title.text; v.color = Title.color; }
        EffectInstance i = o.GetEffect();
        ItemEffect effect = ItemDatabase.a.GetEffect(i.EffectID);
        ItemNum.text = (curIndex+1) + "/" + opNum;
        if (Artist != null)
        {
            string a = ItemDatabase.v.GetArtist(ItemDatabase.v.GetItem(o.ObjectID, o.Type));
            if (a.Length > 0)
                Artist.text = I2.Loc.ScriptLocalization.Get("Artist") + ": " + a;
            else
                Artist.text = "";
        }
        if (EffectTitle != null)
        {
            EffectTitle.text = effect.GetEffectName();
            EffectDetail.text = effect.GetDetailInfo(i.EffectValue);
            EffectStats.text = effect.GetStatInfo(i.EffectValue, curItem.ancient);
        }
    }

    public void Refresh()
    {
        Setup(Armory.GetEquipped(type));
    }

    bool buttonsDisabled;

    [AdvancedInspector.Inspect]
    public void Next()
    {
        if(!buttonsDisabled)
        {
            buttonsDisabled = true;
            ArmorOption[] opts = Armory.instance.GetOptions(type);
            opNum = opts.Length;
            if (opts.Length > 0)
            {
                int idx = curIndex+1;
                if (idx >= opts.Length)
                    idx = 0;
                curIndex = idx;
                //Debug.Log("Changing Equipped Item: From [" + old.GetName() + "] to [" + opts[idx].GetName() + "]");
                Armory.instance.EquipItem(opts[idx]);
                Setup(opts[idx]);
                items.PlayChangeItem();
            }
        }
        StartCoroutine("ResetButton");
    }

    [AdvancedInspector.Inspect]
    public void Prev()
    {
        if (!buttonsDisabled)
        {
            buttonsDisabled = true;
            ArmorOption[] opts = Armory.instance.GetOptions(type);
            opNum = opts.Length;
            if (opts.Length > 0)
            {
                int idx = curIndex - 1;
                if (idx < 0)
                    idx = opts.Length - 1;
                curIndex = idx;
                Armory.instance.EquipItem(opts[idx]);
                Setup(opts[idx]);
                items.PlayChangeItem();
            }
        }
        StartCoroutine("ResetButton");
    }

    IEnumerator ResetButton()
    {
        yield return true;
        yield return true;
        buttonsDisabled = false;
    }

}

[System.Serializable]
public class ItemEvent : UnityEvent<ArmorOption> { }
