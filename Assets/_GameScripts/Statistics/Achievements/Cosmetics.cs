using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
//using Parse;

public class Cosmetics : MonoBehaviour {

    public static Cosmetics instance;
    [Header("Titles")]
    public Sprite TitleIcon;
    public static List<string> Titles
    {
        get
        {
            return PlayerProfile.Profile.PlayerTitles;
        }
    }
    public string TitleColor;
    public int TitleID = -1;

    [Header("Wings")]
    public Sprite WingIcon;
    public static List<int> WingIDs
    {
        get
        {
            return PlayerProfile.Profile.WingIDs;
        }
    }
    public int WingID = -1;

    public static bool Fetched;

    [AdvancedInspector.Inspect]
    public void TestWings()
    {
        if (WingIDs == null || WingIDs.Count < 1)
        {
            AddWing(1);
            AddWing(2);
            AddWing(3);
            AddWing(4);
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //if(Titles == null)
                //Titles = new List<string>();
            //if(WingIDs == null)
                //WingIDs = new List<int>();
        }
        else
            Destroy(this);
    }

    public static void FetchFromServer()
    {
        var x = PlayerProfile.Profile.EquippedCosmetics;
        string[] vals = x.Split(',');
        if (vals.Length > 0)
        {
            int.TryParse(vals[0], out instance.TitleID);
            SetTitle(instance.TitleID);
            if (vals.Length > 1)
            {
                int.TryParse(vals[1], out instance.WingID);
                SetWings(instance.WingID);
            }
        }
        Fetched = true;
    }
    /*
    public static void FetchFromServer()
    {
        Titles = new List<string>();
        WingIDs = new List<int>();
        if (ParseUser.CurrentUser == null)
            return;
        try
        {
            //Titles
            IList<object> tt = new List<object> { };
            if (ParseUser.CurrentUser.ContainsKey("PlayerTitles"))
                tt = ParseUser.CurrentUser.Get<List<object>>("PlayerTitles");
            if (User.ArcadeMode && User.ArcadeUser != null && User.ArcadeUser.ContainsKey("PlayerTitles"))
                tt = User.ArcadeUser.Get<List<object>>("PlayerTitles");
            foreach (var v in tt)
            {
                Titles.Add(v.ToString());
            }
            //Wings
            IList<object> ww = new List<object> { };
            if (ParseUser.CurrentUser.ContainsKey("Wings"))
                ww = ParseUser.CurrentUser.Get<List<object>>("Wings");
            if (User.ArcadeMode && User.ArcadeUser != null && User.ArcadeUser.ContainsKey("Wings"))
                ww = User.ArcadeUser.Get<List<object>>("Wings");
            foreach (var v in ww)
            {
                int r = 0;
                int.TryParse(v.ToString(), out r);
                WingIDs.Add(r);
            }
            //Values
            string x = "-1,-1";
            if (ParseUser.CurrentUser.ContainsKey("EquippedCosmetics"))
                x = ParseUser.CurrentUser.Get<string>("EquippedCosmetics");
            if (User.ArcadeMode && User.ArcadeUser != null && User.ArcadeUser.ContainsKey("Wings"))
                x = User.ArcadeUser.Get<string>("EquippedCosmetics");
            string[] vals = x.Split(',');
            if(vals.Length > 0)
            {
                int.TryParse(vals[0], out instance.TitleID);
                SetTitle(instance.TitleID);
                if(vals.Length > 1)
                {
                    int.TryParse(vals[1], out instance.WingID);
                    SetWings(instance.WingID);
                }
            }
            Fetched = true;
        }
        catch(System.Exception e) { Debug.LogError("Error Getting Cosmetics: " + e.ToString());  };
    }
    */

    #region Titles
    public static void AddTitle(string title)
    {
        //Titles.Add(title);
        /*
        if (User.ArcadeMode && User.ArcadeUser != null)
        {
            ShowTitleNotification(title);
            User.ArcadeUser.AddUniqueToList("PlayerTitles", title);
            User.ArcadeUser.SaveAsync();
        }
        else
        {
            ShowTitleNotification(title);
            ParseUser.CurrentUser.AddUniqueToList("PlayerTitles", title);
            ParseUser.CurrentUser.SaveAsync();
        }
        */
        ShowTitleNotification(title);
        if (!PlayerProfile.Profile.PlayerTitles.Contains(title))
        {
            PlayerProfile.Profile.PlayerTitles.Add(title);
            PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);
        }
    }

    static void ShowTitleNotification(string title)
    {
        Note n = new Note();
        n.icon = instance.TitleIcon;
        n.title = "New Title!";
        n.body = ParseTitle(title, true);
        Notification.Notify(n);
    }

    public static void SetTitle(string title)
    {
        if(instance != null)
        {
            if(title == "")
            {
                SetTitle(-1);
                return;
            }
            for (int i = 0; i < Titles.Count; i++)
            {
                if (Titles[i] == title)
                {
                    SetTitle(i);
                    return;
                }
            }
        }
    }

    public static void SetTitle(int id)
    {
        if (instance != null && Titles != null && id < Titles.Count)
        {
            instance.TitleID = id;
            if(PhotonNetwork.connected)
            {
                var currentProps = new ExitGames.Client.Photon.Hashtable();
                currentProps["fullname"] = GetFullName(true);
                PhotonNetwork.player.SetCustomProperties(currentProps);
            }
            instance.SaveCosmetics();
        }
    }

    public static string GetFullName(bool richtext)
    {
        string baseName = "Player";
        if (PlatformSetup.instance != null)
        {
            baseName = PlatformSetup.instance.DisplayName;
        }
        if(instance != null && instance.TitleID >= 0 && Titles != null && instance.TitleID < Titles.Count)
        {
            string title = Titles[instance.TitleID];
            if (richtext)
                title = RT(title, instance.TitleColor);
            return title.Replace("%", baseName);
        }
        return baseName;
    }

    static string RT(string t, string color)
    {
        string[] pts = Regex.Split(t, @"([%])");
        string comb = "";
        for(int i=0; i<pts.Length; i++)
        {
            if (pts[i].Length > 1)
            {
                comb += "<color=#" + color + ">" + pts[i] + "</color>";
            }
            else
                comb += pts[i];
        }
        return comb;
    }

    public static string ParseTitle(string title, bool richText)
    {
        if (richText)
        {
            if (instance != null)
                title = RT(title, instance.TitleColor);
            else
                title = RT(title, "00ABCD");
        }       
        if(PlatformSetup.instance != null)
            return title.Replace("%", PlatformSetup.instance.DisplayName);
        return title.Replace("%", "_Name_");
    }

    public static bool OwnsTitle(string title)
    {
        if(Titles != null && Titles.Count > 0)
        {
            return Titles.Contains(title);
        }
        return false;
    }
    #endregion

    #region Wings
    public static void AddWing(int id)
    {
        if(!WingIDs.Contains(id) && id >= 0)
        {
            //WingIDs.Add(id);
            /*
            if (User.ArcadeMode && User.ArcadeUser != null)
            {
                ShowWingNotification(id);
                User.ArcadeUser.AddUniqueToList("Wings", id);
                User.ArcadeUser.SaveAsync();
            }
            else
            {
                ShowWingNotification(id);
                ParseUser.CurrentUser.AddUniqueToList("Wings", id);
                ParseUser.CurrentUser.SaveAsync();
            }
            */
            PlayerProfile.Profile.WingIDs.Add(id);
            PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);
        }
    }

    public static void AddWing(Material WingMat)
    {
        AddWing(ItemDatabase.v.GetWingID(WingMat));
    }

    static void ShowWingNotification(int wingID)
    {
        Note n = new Note();
        n.icon = instance.WingIcon;
        n.title = "New Wings!";
        n.body = ItemDatabase.v.WingOptions[wingID].ToString();
        Notification.Notify(n);
    }

    public static void SetWings(Wings wing)
    {
        int id = -1;
        if (wing != null)
            id = ItemDatabase.v.GetWingID(wing);
        string s = "None";
        if (wing != null)
            s = wing.WingMat.name;
        SetWings(id, true);
    }

    public static void SetWings(int id, bool globalID = false)
    {
        if (globalID && id >= 0)
            id = localWingID(id);
        if (instance != null && WingIDs != null && id < WingIDs.Count)
        {
            instance.WingID = id;
            instance.SaveCosmetics();
        }
        else
        {
            Debug.Log("Issue setting wing: " + (instance == null) + "," + (WingIDs == null) + "," + (id < WingIDs.Count));
        }
    }

    static int localWingID(int id)
    {
        for(int i=0; i<WingIDs.Count; i++)
        {
            if (WingIDs[i] == id)
                return i;
        }
        return -1;
    }

    public static bool HasWings(Wings wing)
    {
        int id = -1;
        if (wing != null)
            id = ItemDatabase.v.GetWingID(wing);
        if (WingIDs != null && WingIDs.Count > 0)
            return WingIDs.Contains(id);
        return false;
    }

    public static bool HasWings(Material wingMat)
    {
        int id = -1;
        if (wingMat != null && ItemDatabase.v != null)
            id = ItemDatabase.v.GetWingID(wingMat);
        if (WingIDs != null && WingIDs.Count > 0)
            return WingIDs.Contains(id);
        return false;
    }
    #endregion

    public void SaveCosmetics()
    {
        string cosmeticsStr = TitleID + "," + WingID;
        /*
        if (User.ArcadeMode && User.ArcadeUser != null)
        {
            User.ArcadeUser["EquippedCosmetics"] = x;
            User.TrySave();
        }
        else if (ParseUser.CurrentUser != null)
        {
            ParseUser.CurrentUser["EquippedCosmetics"] = x;
            ParseUser.CurrentUser.SaveAsync();
        }*/
        PlayerProfile.Profile.EquippedCosmetics = cosmeticsStr;
    }
}
