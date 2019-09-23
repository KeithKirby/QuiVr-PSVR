using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Parse;
using System.Linq;
using System.Text.RegularExpressions;

public class Statistics : MonoBehaviour
{
    public static Dictionary<string, object> CurrentStats;
    //static ParseObject currentUser;
    static PlayerProfile currentUser;

    public static void SetUser()
    {
        try
        {
            //if (User.ArcadeMode && User.ArcadeUser != null)
            //currentUser = User.ArcadeUser;
            //else
            //currentUser = ParseUser.CurrentUser;
            currentUser = PlayerProfile.Profile;
            //Stats = new Dictionary<string, object>();
            CurrentStats = new Dictionary<string, object>();
        }
        catch
        {
            Debug.LogError("User File corrupted...");
        }
    }

    public static void SetValue(string id, float value, bool saveNetwork = false)
    {
        EnsureStats();
        PlayerProfile.Profile.Stats[id] = value;
        CurrentStats[id] = value;
        if (saveNetwork)
            SetNetworkValue();
    }

    public static void SetCurrent(string id, float value, bool saveNetwork = false)
    {
        EnsureStats();
        CurrentStats[id] = value;
        if (saveNetwork)
            SetNetworkValue();
        StatCheck.Check();
    }

    public static void AddValue(string id, int val, bool saveNetwork=false)
    {
        EnsureStats();
        int prev = GetInt(id);
        PlayerProfile.Profile.Stats[id] = prev + val;
        int cV = 0;
        if(GetCurrent(id) != null)
        {
            int.TryParse(GetCurrent(id).ToString(), out cV);
        }
        CurrentStats[id] = cV + val;
        if(saveNetwork)
            SetNetworkValue();
    }

    public static void AddValue(string id, float val, bool saveNetwork = false)
    {
        float prev = GetFloat(id);
        PlayerProfile.Profile.Stats[id] = prev + val;
        float cV = 0;
        if (GetCurrent(id) != null)
        {
            float.TryParse(GetCurrent(id).ToString(), out cV);
        }
        CurrentStats[id] = cV + val;
        if(saveNetwork)
            SetNetworkValue();
    }

    public static int GetInt(string id, int defaultValue = 0)
    {
        object o = Get(id);
        if (o != null)
        {
            int x = defaultValue;
            string s = o.ToString();
            if(s.IndexOf(".") >= 0)
                s = s.Split('.')[0];
            int.TryParse(s, out x);
            return x;
        }
        return defaultValue;
    }

    public static void ClearCurrent()
    {
        CurrentStats = new Dictionary<string, object>();
        SetNetworkValue();
    }

    public static float GetCurrentFloat(string id)
    {
        EnsureStats();
        float c = 0;
        if (CurrentStats.ContainsKey(id))
            float.TryParse(GetCurrent(id).ToString(), out c);
        return c;
    }

    public static void AddCurrent(string id, int num, bool saveNetwork = false)
    {
        int c = 0;
        if (CurrentStats.ContainsKey(id))
            c = GetCurrentInt(id);
        CurrentStats[id] = c + num;
        if(saveNetwork)
            SetNetworkValue();
        StatCheck.Check();
    }

    public static object GetCurrent(string id)
    {
        if (!CurrentStats.ContainsKey(id))
            CurrentStats[id] = null;
        return CurrentStats[id];
    }

    public static int GetCurrentInt(string id)
    {
        int x = 0;
        object o = GetCurrent(id);
        if(o != null)
        {
            string s = o.ToString();
            if (s.IndexOf(".") >= 0)
                s = s.Split('.')[0];
            int.TryParse(s, out x);
        }
        return x;
    }
    
    public static float GetFloat(string id, float defaultValue = 0)
    {
        object o = Get(id);
        if (o != null)
        {
            float x = defaultValue;
            float.TryParse(o.ToString(), out x);
            return x;
        }
        return defaultValue;
    }

    static object Get(string id)
    {
        EnsureStats();
        if(!PlayerProfile.Profile.Stats.ContainsKey(id))
            PlayerProfile.Profile.Stats[id] = 0;
        return PlayerProfile.Profile.Stats[id];
    }

    public static Dictionary<string, float> GetStats(string[] keys)
    {
        EnsureStats();
        if (currentUser != null)
        {
            Dictionary<string, float> subset = new Dictionary<string, float>();
            foreach(var v in keys)
            {
                subset.Add(v, PlayerProfile.Profile.Stats[v]);
            }
            return subset;
        }
        return null;
    }

    static ExitGames.Client.Photon.Hashtable currentProps;
    public static void SetNetworkValue()
    {
        if(PhotonNetwork.inRoom)
        {
            currentProps = new ExitGames.Client.Photon.Hashtable();
            foreach(KeyValuePair<string, object> v in CurrentStats)
            {
                currentProps.Add(v.Key, v.Value);
            }
        }
    }

    public static void UpdateNetworkValues()
    {
        if(PhotonNetwork.inRoom && currentProps != null)
        {
            PhotonNetwork.player.SetCustomProperties(currentProps);
        }
    }

    public static void SaveStatistics()
    {
        //EnsureStats();
        try
        {
            /*
            if(User.ArcadeMode && User.ArcadeUser != null)
            {
                foreach (var v in Stats)
                {
                    User.ArcadeUser[v.Key] = v.Value;
                }
                User.TrySave();
            }
            else if (ParseUser.CurrentUser != null)
            {
                foreach (var v in Stats)
                {
                    ParseUser.CurrentUser[v.Key] = v.Value;
                }
                ParseUser.CurrentUser.SaveAsync();
            }
            */
            //PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);
        }
        catch
        {
        }
    }

    static void EnsureStats()
    {
        //if(Stats == null)
        //{
            //Stats = new Dictionary<string, object>();
        //}
        if(CurrentStats == null)
        {
            CurrentStats = new Dictionary<string, object>();
        }
    }

}
