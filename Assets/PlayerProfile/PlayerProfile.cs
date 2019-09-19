using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class VersionInfo
{
    public string Version = "";
    public int Sessions = 1;
    public float TimePlayed = 10;
}

public class PlayerProfile // : IEnumerable<KeyValuePair<string, object>>
{
    static public PlayerProfile Profile;

    public enum SaveCategory
    {
        Default,
        Analytics // Don't save these on PS4 otherwise we'll be saving all the time
    }

    // Player state
    public string Username = "Unnamed";
    public string Outfit = "";
    public int ArmourResource;
    public int MatchmakingRank = 800;

    public float RespawnTime = 0;
    public float InvincibleTime = 5f;

    // Stats
    public int GamesPlayed = 1;
    public Dictionary<string, float> Stats = new Dictionary<string, float>();

    // Arcade
    public string ArcadeName = "Unnamed";
    public string Pin = "";

    // Items
    public List<string> Items = new List<string>();
    public List<ArmorOption> ArmorOptions = new List<ArmorOption>();
    
    public HashSet<string> Achievements = new HashSet<string>();

    public VersionInfo Version = new VersionInfo();
    public Dictionary<string, int> VersionValue = new Dictionary<string, int>();

    public int ArrowsFired = 0;
    public int HighestWave = 0;
    public float DifficultyMultiplier = 1;

    public List<string> PlayerTitles = new List<string>();
    public List<int> WingIDs = new List<int>();
    public string EquippedCosmetics = "";
    public string HitStats = "";

    static public bool Ready = false;

    public static void Init()
    {
        if(null==Profile)
        {
            Debug.Log("Init PlayerProfile");
            Profile = new PlayerProfile();
            Profile.Load();
        }
    }

    public void Load()
    {
        var pp = PlayerPrefs.GetString("PlayerProfile", null);
        if (null != pp)
            JsonUtility.FromJsonOverwrite(pp, this);
        Ready = true;
    }

    public void Save(SaveCategory sc)
    {
        // Player state
        var profileData = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PlayerProfile", profileData);

        if (ShouldSave(sc))
        {
            PlayerPrefs.Save();
        }
    }

    public void ResetProfile()
    {
        if (PlayerPrefs.HasKey("PlayerProfile"))
        {
            WingIDs.Clear();
            PlayerTitles.Clear();
            PlayerPrefs.DeleteKey("PlayerProfile");
            PlayerPrefs.Save();
            Debug.Log(">>>> Player profile has been cleared");
        }
        else
        {
            Debug.Log(">>>> Could not clear player profile");
        }
    }

    bool ShouldSave(SaveCategory sc)
    {
#if UNITY_PS4
        switch(sc)
        {
            case SaveCategory.Default:
                return true;
            case SaveCategory.Analytics:
            default:
                return false;

        }
#else
        return true;
#endif
    }
    
    private readonly IDictionary<string, object> _data = new Dictionary<string, object>();
    /*
    /// <summary>
    /// Gets or sets a value on the object. It is recommended to name
    /// keys in partialCamelCaseLikeThis.
    /// </summary>
    /// <param name="key">The key for the object. Keys must be alphanumeric plus underscore
    /// and start with a letter.</param>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">The property is
    /// retrieved and <paramref name="key"/> is not found.</exception>
    /// <returns>The value for the key.</returns>
    virtual public object this[string key]
    {
        get
        {
            var value = _data[key];
            return value;
        }
        set
        {
            Set(key, value);
        }
    }

    /// <summary>
    /// Perform Set internally which is not gated by mutability check.
    /// </summary>
    /// <param name="key">key for the object.</param>
    /// <param name="value">the value for the key.</param>
    internal void Set(string key, object value)
    {
        _data[key] = value;
    }
    
    /// <summary>
    /// Returns whether this object has a particular key.
    /// </summary>
    /// <param name="key">The key to check for</param>
    public bool ContainsKey(string key)
    {
        return _data.ContainsKey(key);
    }
    
    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
    {
        return _data.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
    }

    /// <summary>
    /// Gets a value for the key of a particular type.
    /// <typeparam name="T">The type to convert the value to. Supported types are
    /// ParseObject and its descendents, Parse types such as ParseRelation and ParseGeopoint,
    /// primitive types,IList&lt;T&gt;, IDictionary&lt;string, T&gt;, and strings.</typeparam>
    /// <param name="key">The key of the element to get.</param>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">The property is
    /// retrieved and <paramref name="key"/> is not found.</exception>
    /// </summary>
    public T Get<T>(string key)
    {
        return (T)(this[key]);
    }

    public bool TryGetValue<T>(string key, out T result)
    {
        if (ContainsKey(key))
        {
            try
            {
                var temp = Get<T>(key);
                result = temp;
                return true;
            }
            catch (InvalidCastException ex)
            {
                result = default(T);
                return false;
            }
        }
        result = default(T);
        return false;
    }
    */
}