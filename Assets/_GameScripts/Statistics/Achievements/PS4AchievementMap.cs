using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PS4AchievementMapData", menuName = "PS4AchievementMap", order = 1)]
public class PS4AchievementMap : ScriptableObject
{
    public QuivrToPS4[] Achievements;
    public int GetTrophyId(string quivrAchievementId)
    {
        for(int i = 0;i<Achievements.Length;++i)
        {
            if (Achievements[i].ID == quivrAchievementId)
                return i;
        }
        return -1;
    }
}

[System.Serializable]
public class QuivrToPS4
{
    public string ID;
}