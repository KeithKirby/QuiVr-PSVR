using UnityEngine;
using System.Collections;

public class ItemRandomizer : MonoBehaviour {

    float cooldown = 1f;
    float cd = 0;

    void Update()
    {
        if (cd >= 0)
            cd -= Time.deltaTime;
    }

	public void Randomize()
    {
        if(cd <= 0)
        {
            cd = cooldown;
            ArmorOption[] Helmets = Armory.instance.GetOptions(ItemType.Helmet);
            ArmorOption[] Chests = Armory.instance.GetOptions(ItemType.ChestArmor);
            ArmorOption[] Arrows = Armory.instance.GetOptions(ItemType.Arrow);
            if (Helmets.Length > 0)
                Armory.instance.EquipItem(Helmets[Random.Range(0, Helmets.Length)]);
            if (Chests.Length > 0)
                Armory.instance.EquipItem(Chests[Random.Range(0, Chests.Length)]);
            if (Arrows.Length > 0)
                Armory.instance.EquipItem(Arrows[Random.Range(0, Arrows.Length)]);
        }
    }
}
