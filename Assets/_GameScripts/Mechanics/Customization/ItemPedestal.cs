using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events; 
public class ItemPedestal : MonoBehaviour {

    public ArmorOption CurrentItem;
    public EMOpenCloseMotion PanelMotion;
    public int curIndex;
    public Projector SpellProjector;
    public ItemAction[] Actions;

    public Text[] ActionTitle;
    public Text ActionDetail;

    PedestalCapture capture;

    [Space]
    [Header("ItemUI")]
    public Text ItemTitle;
    public Text EffectTitle;
    public Text EffectDetail;
    public Text EffectStats;
    public GameObject[] AncientDisplay;

    void Awake()
    {
        capture = GetComponentInChildren<PedestalCapture>();
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
    }

    public void Clear()
    {
        CurrentItem = null;
        if(PanelMotion.motionState == EMBaseMotion.MotionState.Open || PanelMotion.motionState == EMBaseMotion.MotionState.Opening)
            PanelMotion.Close(true);
        HideAll();
        capture.Clear();
    }

    public void Next()
    {
        curIndex++;
        if (curIndex >= Actions.Length)
            curIndex = 0;
        UpdateActions();
    }

    public void Prev()
    {
        curIndex--;
        if (curIndex < 0)
            curIndex = Actions.Length - 1;
        UpdateActions();
    }

    void UpdateActions()
    {
        //Action Setup
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
}

//Allows rerolling ability values in exchange for motes.
