using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
//using Parse;

public class AAScoreUI : MonoBehaviour {

    public Text ExtraInfo;
    public Text Current;
    public Text Best;
    public string ScorePrefix = "Score";

    public bool haveTop;

    public List<GenericScore> scoreValues;
    public ScoreObjectUI[] TopScores;

    IEnumerator Start()
    {
        while(!PlatformSetup.instance.CompletedSetup)
        {
            yield return true;
        }
        if(ExtraInfo != null)
        {
            /*
            string versionReq = null;
            bool gotVer = false;
            if (AppBase.v.isBeta)
            {
                gotVer = ParseConfig.CurrentConfig.TryGetValue("BetaScoreVersion", out versionReq);
                versionReq = versionReq;
            }
            else
                gotVer = ParseConfig.CurrentConfig.TryGetValue("ScoreVersion", out versionReq);
            if (gotVer)
                ExtraInfo.text = versionReq + " - " + Application.version + "  [<color=#abcdef>" + ScoreDatabase.LeaderboardID() + "</color>]";*/
#if UNITY_PS4
#else
            ExtraInfo.text = Application.version + "  [<color=#abcdef>" + ScoreDatabase.LeaderboardID() + "</color>]";
#endif
        }
    }

	public void ChangeCurrent(int newScore)
    {
        Current.text = ScorePrefix + ": " + newScore;
    }

    public void ChangeHigh(int newScore)
    {
        Best.text = "Best: " + newScore;
        CheckTopScores(newScore);
    }

    public void LoadTopScores(GenericScore[] tops)
    {
        haveTop = true;
        scoreValues = new List<GenericScore>();
        foreach(var v in tops)
        {
            scoreValues.Add(v);
        }
        SortScores();
    }

    void SortScores()
    {
        scoreValues.Sort((x,y) => {
            if (x.score != y.score)
                return y.score.CompareTo(x.score);
            return y.name.CompareTo(x.name);
        });
        int maxVal = Mathf.Min(TopScores.Length, scoreValues.Count);
        for (int i = 0; i < maxVal; i++)
        {
            TopScores[i].Setup(i+1, scoreValues[i]);
        }
    }

    public void CheckTopMP(bool success, int scr)
    {
        if(success)
        {
#if UNITY_PS4
            string[] vals = ScoreDatabase.Inst.GetMPIdAndComment();
#else
            string[] vals = ScoreDatabase.GetMPValues();            
#endif
            CheckTopScores(scr, vals[1], vals[0]);
        }
    }

    void CheckTopScores(int scr, string username="", string id="")
    {
        //Setup User Score
        string un = username;
        if (un.Length < 1)
            un = PlatformSetup.instance.DisplayName;
        string ID = id;
        if(ID.Length < 1 && PlatformSetup.instance != null)
        {
            object sid = "";
            ID = PlatformSetup.instance.UserID;
        }

        //Update Score
        if(haveTop)
        {
            bool newTop = false;
            foreach(var v in scoreValues)
            {
                if (v.ID == ID)
                {
                    if (scr > v.score)
                    {
                        v.score = scr;
                        SortScores();
                    }
                    return;
                }
                else if (scr > v.score)
                {
                    newTop = true;
                }
            }
            if (scoreValues.Count < TopScores.Length)
                newTop = true;
            if(newTop)
            {
                Debug.Log("New Top Score");
                ScoreManager mgr = GetComponent<ScoreManager>();
                if(mgr != null)
                    mgr.TryAwardItem();
                if(scoreValues.Count >= TopScores.Length)
                {
                    scoreValues[scoreValues.Count - 1].name = un;
                    scoreValues[scoreValues.Count - 1].score = scr;
                }
                else
                {
                    GenericScore s = new GenericScore(PlatformSetup.instance.DisplayName, scr, 0, ID);
                    scoreValues.Add(s);
                }
            }
            SortScores();
        }
    }
}
