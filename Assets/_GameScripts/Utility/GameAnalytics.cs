using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
//using Parse;
using Steamworks;

public class GameAnalytics : MonoBehaviour {

    BowAim Bow;

    private bool allowQuitting = false;
    bool startedQuit = false;

    public static GameAnalytics instance;

    void Awake()
    {
        instance = this;
    }

    void OnApplicationQuit()
    {
        if (!startedQuit)
        {
            startedQuit = true;
            StartCoroutine("DelayedQuit");
        }
        if (!allowQuitting)
            Application.CancelQuit();

    }

    public void SendArrowEvent(int numFired)
    {
#if !NOANALYTICS
        Analytics.CustomEvent("ArrowsFired", new Dictionary<string, object> { { "count", numFired } });
#endif
        /*
        if (ParseUser.CurrentUser != null)
        {
            if(!ParseUser.CurrentUser.ContainsKey("ArrowsFired"))
                ParseUser.CurrentUser["ArrowsFired"] = numFired;
            else
                ParseUser.CurrentUser.Increment("ArrowsFired", numFired);
            ParseUser.CurrentUser.SaveAsync();
        }  
        */
        PlayerProfile.Profile.ArrowsFired++;
        PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);

        if (SteamManager.Initialized)
        {
            SteamUserStats.SetStat("arw_f", numFired);
            SteamUserStats.StoreStats();
        }

    }

    public void SendLossEvent(int wave, int enemiesKilled)
    {
        wave -= 1;
#if !NOANALYTICS
        Analytics.CustomEvent("KeepLost", new Dictionary<string, object> { { "wave", wave } });
        Analytics.CustomEvent("EnemiesKilled", new Dictionary<string, object> { { "count", enemiesKilled } });
#endif
        /*
        if(ParseUser.CurrentUser != null)
        {
            int highestWave = 0;
            if (ParseUser.CurrentUser.ContainsKey("HighestWave"))
                highestWave = ParseUser.CurrentUser.Get<int>("HighestWave");
            if (wave > highestWave)
            {
                ParseUser.CurrentUser["HighestWave"] = wave;
                ParseUser.CurrentUser.SaveAsync();
            }
        }
        */
        PlayerProfile.Profile.HighestWave = wave;
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetStat("max_w", wave);
            SteamUserStats.StoreStats();
        }
    }

    IEnumerator DelayedQuit()
    {
        if(Bow == null)
            Bow = BowAim.instance;
        if (Bow != null)
            SendArrowEvent(Bow.GetArrowsShot());
        yield return new WaitForSeconds(1f);
        allowQuitting = true;
        Application.Quit();
    }
}
