using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achieves", menuName = "Achievements", order = 1)]
public class AchieveValues : ScriptableObject
{
    public Achieve[] Achievements;

    public bool IsReal(string id)
    {
        foreach(var v in Achievements)
        {
            if (v.ID == id)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class Achieve
{
    public string Name;
    public string ID;
    public Sprite Icon;
    public string Detail;
    public RewardType rewardType;
    public string TitleReward;
    public ArmorOption ItemReward;
    public Material WingReward;

    public Achieve()
    {
        ItemReward = new ArmorOption("None", ItemType.BowBase, 0, new Color[] { }, "0,0", 6);
    }

    public override string ToString()
    {
        if (ID != null)
            return ID;
        return "None";
    }

}

public enum RewardType
{
    None,
    Item,
    Title,
    Wings
}
