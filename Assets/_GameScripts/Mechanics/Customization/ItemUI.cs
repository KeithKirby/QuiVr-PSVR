using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour {

    public Text ItemTitle;
    public Text EffectTitle;
    public Text EffectDetail;
    public Text EffectStats;
    public GameObject[] AncientDisplays;

    public void SetupItemDisplay(ArmorOption o)
    {
        foreach (var v in AncientDisplays)
        { v.SetActive(false); }
        if (o == null)
            if (o == null)
        {
            ItemTitle.text = "Not Equipped";
            EffectTitle.text = "";
            EffectDetail.text = "";
            EffectStats.text = "";
            return;
        }
        for (int x = 0; x < AncientDisplays.Length; x++)
        {
            if (o.ancient > x)
                AncientDisplays[x].SetActive(true);
        }
        ItemTitle.text = o.ItemName;
        ItemTitle.color = ItemDatabase.v.RarityColors[o.rarity];
        EffectInstance i = new EffectInstance(o.Effect);
        ItemEffect effect = ItemDatabase.a.GetEffect(i.EffectID);
        if (EffectTitle != null)
        {
            EffectTitle.text = effect.EffectName;
            EffectDetail.text = effect.GetDetailInfo(i.EffectValue);
            EffectStats.text = effect.GetStatInfo(i.EffectValue);
        }
    }
}
