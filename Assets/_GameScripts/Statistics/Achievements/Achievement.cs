using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Sony.NP;
using System.Linq;
//using Parse;

public class Achievement : MonoBehaviour {

    private static CGameID gameID;
    private static bool SteamInit;

    public static void Setup(CGameID gid)
    {
        gameID = gid;
        SteamInit = true;
    }

    public static void CheckAllAchievements()
    {
        foreach(var v in AppBase.a.Achievements)
        {
            if (HasAchievement(v.ID))
            {
                EarnAchievement(v.ID);
            }
        }
    }

	public static bool HasAchievement(string id)
    {
        return GetEarned().Contains(id);
    }

    public static bool HasSteamEarned(string id)
    {
        bool achieved = false;
        if(SteamInit)
        {
            SteamUserStats.GetAchievement(id, out achieved);
        }
        return achieved;
    }

    public static void EarnAchievement(string id, bool itemDisplay=false)
    {
        if(!AppBase.v.cheated && AppBase.a.IsReal(id))
        {
            var tm = GameObject.FindObjectOfType<PS4TrophyManager>();
            if (null != tm)
            {
                tm.UnlockTrophy(id);
            }
            else
            {
                Debug.Log("Failed to find PS4TrophyManager");
            }
            UnlockSteamAchievement(id);
            UnlockAccountAchievement(id, itemDisplay);
        }
    }

    private static void UnlockAccountAchievement(string id, bool itemDisplay=false)
    {
        if(!HasAchievement(id))
        {
            /*if(User.ArcadeMode && User.ArcadeUser != null)
            {
                ShowNotification(id);
                User.ArcadeUser.AddUniqueToList("EarnedAch", id);
                User.ArcadeUser.SaveAsync();
            }
            else
            {
                ShowNotification(id);
                ParseUser.CurrentUser.AddUniqueToList("EarnedAch", id);
                ParseUser.CurrentUser.SaveAsync();
            }*/
            PlayerProfile.Profile.Achievements.Add(id);
        }
        Achieve ach = AppBase.GetAchievement(id);
        if (ach != null && ach.rewardType == RewardType.Item && !SpecialItems.HasItem(ach.ItemReward))
        {
            SpecialItems.GiveItem(ach.ItemReward, itemDisplay);
        }
        else if(ach != null && ach.rewardType == RewardType.Title && !Cosmetics.OwnsTitle(ach.TitleReward))
        {
            Cosmetics.AddTitle(ach.TitleReward);
        }
        else if(ach != null && ach.rewardType == RewardType.Wings && !Cosmetics.HasWings(ach.WingReward))
        {
            Cosmetics.AddWing(ach.WingReward);
        }
    }

    private static void UnlockSteamAchievement(string id)
    {
        if(SteamInit && !HasSteamEarned(id))
        {
            try
            {
                SteamUserStats.SetAchievement(id);
                SteamUserStats.StoreStats();
            }
            catch { }
        }
    }

    public static List<string> GetEarned()
    {
        //IList<object> e = new List<object> { };
        //if (ParseUser.CurrentUser.ContainsKey("EarnedAch"))
        //e = ParseUser.CurrentUser.Get<List<object>>("EarnedAch");

        //if (User.ArcadeMode && User.ArcadeUser != null && User.ArcadeUser.ContainsKey("EarnedAch"))
        //{
        //e = User.ArcadeUser.Get<List<object>>("EarnedAch");
        //}
        //List<string> achs = new List<string>();        
        //foreach (var v in e)
        //{
        //achs.Add(v.ToString());
        //}
        //return achs;

        var achs = PlayerProfile.Profile.Achievements.ToList();
        return achs;
    }

    static void ShowNotification(string id)
    {
        Note n = new Note();
        Achieve ach = AppBase.GetAchievement(id);
        if(ach != null)
        {
            n.icon = ach.Icon;
            n.title = "Achievement Unlocked!";
            n.body = ach.Name;
        }
        Notification.Notify(n);
    }
}
