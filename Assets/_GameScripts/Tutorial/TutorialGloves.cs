using UnityEngine;
using System.Collections;

public class TutorialGloves : MonoBehaviour {

    public ArmorOption GloveGiven;

	public void GiveGloves()
    {
        var options = Armory.instance.GetOptions();
        bool hasItem = false;
        ArmorOption o = GloveGiven;
        foreach(var v in options)
        {
            if(v.Duplicate(GloveGiven))
            {
                hasItem = true;
                o = v;
                break;
            }
        }
        if(!hasItem)
        {
            Armory.instance.AddItem(GloveGiven, true, true);
            Armory.instance.EquipItem(GloveGiven, true);
        }
        else
        {
            Debug.Log("Had tutorial item, equipping");
            Armory.instance.EquipItem(o, true);
        }
    }
}
