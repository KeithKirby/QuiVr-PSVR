using UnityEngine;
using System.Collections;

public class ItemDatabase : MonoBehaviour {

    public DataValues DataSet;
    public AbilityValues AbilitySet;

    public LayerMask EquippedGlovesMask;

    public static DataValues v = null;
    public static AbilityValues a = null;
    public static LayerMask EquippedGlovesMaskStatic;

    void Awake()
    {
        if (v == null)
            v = DataSet;
        if (a == null)
            a = AbilitySet;
        if (0 == EquippedGlovesMaskStatic)
            EquippedGlovesMaskStatic = EquippedGlovesMask;
    }

    public static string RandomEffect(int effectID, int ancient = 0, float preValue = -1f)
    {
        string s = "";
        if (a == null)
            return s;
        foreach(var e in a.Effects)
        {
            if (e.EffectID == effectID)
            {
                float value = e.GenerateValue(preValue, ancient);
                s = effectID + "^" + value;
            }
        }
        return s;
    }

    static public bool IsValidDrop(ItemDrop drop)
    {
        foreach (var e in ItemDatabase.a.Effects)
        {
            if (e.EffectID == drop.EffectID)
            {
                return true;
            }
        }
        return false;
    }

    public static int RollAncient()
    {
        if(Random.Range(0, 100) < 10)
        {
            if (Random.Range(0, 100) < 5)
                return 2;
            return 1;
        }
        return 0;
    }

    public static ArmorOption Reroll(ArmorOption obj)
    {
        Armory.instance.RemoveResource(v.ResourceValues[obj.rarity], false);
        EffectInstance e = new EffectInstance(obj.Effect);
        obj.ancient = RollAncient();
        obj.Effect = RandomEffect(e.EffectID, obj.ancient, e.EffectValue);
        obj.eft = new EffectInstance(obj.Effect);
        return obj;
    }

    public static GameObject GetDisplay(ItemType type, int displayID)
    {
        if (type == ItemType.Arrow)
            return v.Arrows[(displayID < v.Arrows.Length) ? displayID : 0];
        if (type == ItemType.Quiver)
            return v.Quivers[(displayID < v.Quivers.Length) ? displayID : 0];
        if (type == ItemType.ChestArmor)
            return v.Chestplates[(displayID < v.Chestplates.Length) ? displayID : 0];
        if (type == ItemType.Helmet)
            return v.Helmets[(displayID < v.Helmets.Length) ? displayID : 0];
        if (type == ItemType.Gloves)
            return v.Gloves[(displayID < v.Gloves.Length) ? displayID : 0];
        if(type == ItemType.BowBase)
            return v.Bows[(displayID < v.Bows.Length) ? displayID : 0];
        return null;
    }

    public static ItemEffect GetEffect(int id)
    {
        if(v != null)
        {
            foreach (var e in a.Effects)
            {
                if (e.EffectID == id)
                    return e;
            }
        }
        return a.Effects[0];
    }
}
