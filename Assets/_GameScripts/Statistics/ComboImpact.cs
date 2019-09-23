using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboImpact : MonoBehaviour {

	public void DoHit(ArrowCollision e)
    {
        HitMarker.ShowHit(e.impactPos, e.Critical());
        if (e.Distance() > Statistics.GetCurrentFloat("LongShot"))
        {
            Statistics.SetCurrent("LongShot", (int)e.Distance(), true);
            if (e.Distance() > Statistics.GetInt("LongestShot"))
                Statistics.SetValue("LongestShot", (int)e.Distance());
        }
        if (e.Critical())
        {
            Statistics.AddCurrent("Crit", 1);
            Statistics.AddValue("EnemyCrit", 1f);
        }
        Statistics.AddCurrent("Hit", 1);
        Statistics.AddCurrent("Combo", 1);
        int curCombo = (int)Statistics.GetCurrentFloat("Combo");
        int CurHighCombo = (int)Statistics.GetCurrentFloat("HighCombo");
        if (curCombo > CurHighCombo)
            Statistics.SetCurrent("HighCombo", curCombo, true);
        if (curCombo > Statistics.GetInt("BestCombo"))
            Statistics.SetValue("BestCombo", curCombo);
        int hit = (int)Statistics.GetCurrentFloat("Hit");
        int miss = (int)Statistics.GetCurrentFloat("ArrowsMissed");
        int accuracy = (int)((hit / (float)(hit + miss)) * 100f);
        int crits = (int)Statistics.GetCurrentFloat("Crit");
        int critPerc = (int)((crits / (float)hit) * 100f);
        Statistics.SetCurrent("Accuracy", accuracy, true);
        Statistics.SetCurrent("CritPerc", critPerc, true);
        float Distance = e.Distance();
        Statistics.AddCurrent("ArrowsHit", 1);
        /*
        Statistics.AddToBitArray("Acc100", true, 100);
        Statistics.AddToBitArray("Acc500", true, 500);
        */
        Statistics.AddValue("EnemyHit", 1f);
        if (Distance <= 25)
            Statistics.AddValue("Hit0to25", 1f);
        else if (Distance <= 50)
            Statistics.AddValue("Hit25to50", 1f);
        else if (Distance <= 75)
            Statistics.AddValue("Hit50to75", 1f);
        else if (Distance <= 100)
            Statistics.AddValue("Hit75to100", 1f);
        else
            Statistics.AddValue("HitOver100", 1f);
        if (Statistics.GetFloat("LongestShot") < e.Distance())
            Statistics.SetValue("LongestShot", e.Distance());
    }
}
