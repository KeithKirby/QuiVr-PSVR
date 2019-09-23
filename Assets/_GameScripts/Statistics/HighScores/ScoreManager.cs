using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using Sony.NP;
//using Parse;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;
    public int ScoreCount;
    public string ScoreClass;
    public int HighScore;
    public int CurrentScore;
    public int DaysKept = -1;
    public bool TempItemReward;
    public string ItemReason;
    public ArmorOption TempItemForTop;

    [System.Serializable]
    public class IntEvent : UnityEvent<int> { };
    [System.Serializable]
    public class ScoreEvent : UnityEvent<GenericScore[]> { };
    public ScoreEvent GotTopScores;
    public IntEvent ScoreChanged;
    public IntEvent HighScoreChanged;
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool, int> { };
    public BoolEvent Saved;

    bool newHigh;
    bool gotTops;
    GenericScore[] tops;
    public bool changeDemo;

    AAScoreUI sui;

    void Awake()
    {
        instance = this;
        sui = GetComponent<AAScoreUI>();
        if (changeDemo && AppBase.v.isDemo)
            ScoreClass += "Demo";
    }

    IEnumerator Start()
    {
        while(!PlatformSetup.instance.CompletedSetup)
        {
            yield return true;
        }
        RequestScore();
    }

	public void RequestScore()
    {
#if UNITY_PS4
        ScoreDatabase.Inst.RequestScore(this, ScoreClass, DaysKept);
        ScoreDatabase.Inst.TopList(this, ScoreClass, ScoreCount, DaysKept);
#else
        ScoreDatabase.RequestScore(this, ScoreClass, DaysKept);
        ScoreDatabase.TopList(this, ScoreClass, ScoreCount, DaysKept);
#endif
    }

    public void SetHighScore(int score)
    {
        HighScore = score;
        shouldCheck = true;
    }

    public void ChangeClass(string cl)
    {
        ScoreClass = cl;
        RequestScore();
    }

    int lhc = 0;
    int lsc = 0;
    bool shouldCheck;
    bool didSave;
    void Update()
    {
        if(lhc != HighScore)
        {
            HighScoreChanged.Invoke(HighScore);
            lhc = HighScore;
        }
        if(gotTops)
        {
            gotTops = false;
            GotTopScores.Invoke(tops);
        }
        if(shouldCheck)
        {
            shouldCheck = false;
            CheckHighScore();
        }
        if(didSave)
        {
            Saved.Invoke(didSave, HighScore);
            didSave = false;
        }
/*
        if (ScoreClass == "FFKDiff")
        {
            var timeNow = DateTime.Now;
            int score = (int)(timeNow.Ticks / 10000000000);

            if (Input.GetButtonDown("Square"))
            {
                if (PhotonNetwork.inRoom)
                    LeaveMultiplayer.Click();                
            }
            if (Input.GetButtonDown("Triangle"))
            {
                string accountId = SonyNpUserProfiles.GetLocalAccountId(PS4Common.InitialUserId).Id.ToString();
                ScoreDatabase.Inst.SaveNewScore(this, ScoreClass, score, DaysKept, "", accountId.ToString());
            }
        }
*/
        /*
        if (ScoreClass == "FFKDiff")
        {
            var timeNow = DateTime.Now;
            int score = (int)(timeNow.Ticks / 10000000000);

            if (Input.GetButtonDown("Square"))
            {
                string accountId = SonyNpUserProfiles.GetLocalAccountId(PS4Common.InitialUserId).Id.ToString();
                ScoreDatabase.Inst.SaveNewScore(this, ScoreClass, score, DaysKept, "", accountId.ToString());
            }
        }

        if (ScoreClass == "CanyonMPDiff")
        {
            var timeNow = DateTime.Now;
            int score = (int)(timeNow.Ticks / 10000000000);

            if (Input.GetButtonDown("Triangle"))
            {
                string[] vals = ScoreDatabase.Inst.GetMPIdAndComment();
                ChangeScoreMP(score, vals[0], vals[1]);
            }
        }
        */
    }

    public void SavedScores(bool success)
    {
        didSave = success;
    }

    public void AddToScore(int val)
    {
        ChangeScore(CurrentScore + val);
    }

    public void TopScoresCB(GenericScore[] scrs)
    {
        tops = scrs;
        if (TempItemReward)
            CheckTopsForReward();
        gotTops = true;
    }

    void CheckTopsForReward()
    {
        //if(ParseUser.CurrentUser != null)
        {
            //string un = ParseUser.CurrentUser.Username;
            //if (User.ArcadeMode && User.ArcadeUser != null)
            //un = User.ArcadeUser.Get<string>("Name");
#if UNITY_PS4
            var un = PS4Common.InitialUserDetals.userName;
#else
            var un = PlayerProfile.Profile.Username;
#endif
            int numScores = 10;
            if (sui != null)
                numScores = sui.TopScores.Length;
            for(int i=0; i<Mathf.Min(tops.Length, numScores); i++)
            {
                GenericScore v = tops[i];
                if (v.ID.Contains(un))
                {
                    TryAwardItem();
                    return;
                }
            }
        }
    }

    public void TryAwardItem()
    {
        if(TempItemReward && !Armory.instance.HasDuplicate(TempItemForTop))
        {
            Armory.instance.AddItem(TempItemForTop, false, false);
            Notification.Notify(new Note(TempItemForTop, true, ItemReason));
        }
    }

    public void ChangeScore(int val)
    {
        CurrentScore = val;
        ScoreChanged.Invoke(CurrentScore);
        CheckHighScore();
    }

    bool triedSave;
    void CheckHighScore()
    {
        //int localHigh = GetLocalScore();
        if(CurrentScore > HighScore)// && localHigh <= HighScore)
        {
            newHigh = true;
            SetHighScore(CurrentScore);
            //SaveLocalScore(CurrentScore);
        }
    }

    /*
    int GetLocalScore()
    {
        return 0;
        if(PlayerPrefs.HasKey(ScoreClass))
        {
            string s = Crypto.DecryptStringAES(PlayerPrefs.GetString(ScoreClass), AppBase.v.CtoStr);
            string[] parts = s.Split('|');
            int x = 0;
            int.TryParse(parts[0], out x);
            if(parts.Length > 1)
            {
                DateTime dt = Convert.ToDateTime(parts[1]);
                TimeSpan duration = DateTime.UtcNow - dt;
                if (duration.Days > DaysKept)
                {
                    x = 0;
                    //SaveLocalScore(x);
                }
            }
        }
        return 0;
    }

    void SaveLocalScore(int val)
    {
        return;
        string date = DateTime.UtcNow.ToString();
        string toSave = Crypto.EncryptStringAES(val + "|" + date, AppBase.v.CtoStr);
        PlayerPrefs.SetString(ScoreClass, toSave);
        PlayerPrefs.Save();
    }
    */

    public void SubmitIfNewHigh()
    {
        if (newHigh)
        {
            Debug.Log("Submitting New Score...");
            SaveHighScore();
            newHigh = false;
        }
    }

    public void SaveHighScore()
    {
#if UNITY_PS4
        string accountId = SonyNpUserProfiles.GetLocalAccountId(PS4Common.InitialUserId).Id.ToString();
        ScoreDatabase.Inst.SaveNewScore(this, ScoreClass, HighScore, DaysKept, "", accountId);
#else
        ScoreDatabase.SaveNewScore(this, ScoreClass, HighScore, DaysKept);
#endif
    }

#region Multiplayer

    public void ChangeScoreMP(int score, string id, string names)
    {
        if (score > HighScore)
            HighScore = score;
#if UNITY_PS4
        ScoreDatabase.Inst.SaveNewScore(this, ScoreClass, score, DaysKept, id, names);
#else
        ScoreDatabase.SaveNewScore(this, ScoreClass, score, DaysKept, id, names);
#endif
    }

#endregion

}
