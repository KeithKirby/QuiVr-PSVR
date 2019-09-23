using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCheck : MonoBehaviour {

    //Check Stats for Achievements
    public static void Check()
    {
        if(GameBase.instance != null && GameBase.instance.Difficulty > 10)
        {
            HealAchievements();
        }
    }

    static void HealAchievements()
    {  
        if (Statistics.GetCurrentInt("gate_healed") > 250)
            Achievement.EarnAchievement("GATE_HEAL_1");
        if (Statistics.GetCurrentInt("gate_healed") > 500)
            Achievement.EarnAchievement("GATE_HEAL_2");
        if (Statistics.GetCurrentInt("gate_healed") > 1000)
            Achievement.EarnAchievement("GATE_HEAL_3");
    }

    public static void CheckArmor(Outfit outfit)
    {
        bool AllHollowed = true;
        foreach(var v in outfit.toList())
        {
            if (v.ancient == 2)
                Achievement.EarnAchievement("PRIMORDIAL");
            if (v.ancient == 0 && v.Type != ItemType.BowBase)
                AllHollowed = false;
        }
        if (AllHollowed)
            Achievement.EarnAchievement("ALL_HALLOWED");
    }

    public static void ArrowMiss()
    {
        Statistics.SetCurrent("LongShot_Combo", 0); 
    }
}
