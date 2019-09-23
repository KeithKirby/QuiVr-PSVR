using UnityEngine;
using System.Collections;

public class Mannequin : MonoBehaviour {

    DummyArmor amr;
    public bool mine = true;
    bool initialized;
	IEnumerator Start()
    {
        amr = GetComponent<DummyArmor>();
        while(Armory.instance == null || !Armory.instance.updated)
        {
            yield return true;
        }
        if(mine)
        {
            foreach (var o in Armory.CurrentOutfitList())
            {
                EquipItem(o);
            }
            Armory.instance.OnEquip.AddListener(EquipItem);
        }
        initialized = true;
    }

    void OnEnable()
    {
        if(initialized)
        {
            foreach (var o in Armory.CurrentOutfitList())
            {
                EquipItem(o);
            }
        }
    }

    public void EquipItem(ArmorOption item)
    {
        amr.EquipItem(item.ToString());
    }
}
