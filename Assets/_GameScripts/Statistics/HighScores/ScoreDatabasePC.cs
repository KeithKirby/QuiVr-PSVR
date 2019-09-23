using UnityEngine;
using System.Collections;
//using Parse;
using System.Collections.Generic;
using System;

public class ScoreDatabasePC : MonoBehaviour {

	public static void RequestScore(ScoreManager scr, string ScoreClass, int MaxAge, string UserID = "")
    {
        // dw - TODO: Implement leaderboards
        Debug.LogWarning("RequestScore not implemented yet, will need to hook up to psn leaderboards");
        ScoreCallback(scr, 0);
        /*
        try
        {
            if (ParseUser.CurrentUser != null && PlatformSetup.Initialized)
            {
                string id = UserID;
                if (id.Length < 5)
                {
                    id = ParseUser.CurrentUser.Username;
                    if (User.ArcadeMode && User.ArcadeUser != null)
                        id = id+"_"+User.ArcadeUser.Get<string>("Name");
                }

                var query = ParseObject.GetQuery(ScoreClass).WhereEqualTo("UserID", id);
                if(MaxAge > 0)
                    query = query.WhereGreaterThan("updatedAt", DateTime.UtcNow.AddDays(-1 * MaxAge));

                query = query.WhereEqualTo("BoardID", LeaderboardID());       
                query.FirstAsync().ContinueWith(t =>
                {
                    if (!t.IsFaulted && !t.IsCanceled)
                    {
                        var myScore = t.Result;
                        int retrScore = 0;
                        if (myScore.ContainsKey("Score"))
                            retrScore =  myScore.Get<int>("Score");
                        ScoreCallback(scr, retrScore);
                    }
                    else
                    {
                        ScoreCallback(scr, 0);
                    }
                });
            }
            else
            {
                ScoreCallback(scr, 0);
            }
        }
        catch
        {
            if (PlayerPrefs.HasKey(ScoreClass))
            {
                ScoreCallback(scr, PlayerPrefs.GetInt(ScoreClass));
            }
        }
        */
    }

    static void ScoreCallback(ScoreManager scr, int score)
    {
        scr.SetHighScore(score);
    }

    public static void TopList(ScoreManager scr, string ScoreClass, int limit, int MaxAge)
    {
        List<GenericScore> scores = new List<GenericScore>();

        Debug.LogWarning("TopList not implemented yet, will need to hook up to psn leaderboards");
        TopCallback(scr, scores.ToArray());
        /*
        try
        {
            var query = ParseObject.GetQuery(ScoreClass).OrderByDescending("Score").ThenByDescending("Username");
            if (MaxAge > 0)
                query = ParseObject.GetQuery(ScoreClass).WhereGreaterThan("updatedAt", DateTime.UtcNow.AddDays(-1*MaxAge)).OrderByDescending("Score").ThenByDescending("Username");
            query = query.WhereEqualTo("BoardID", LeaderboardID());
            query.Limit(10);
            query.FindAsync().ContinueWith(t =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    var results = t.Result;
                    foreach (var item in results)
                    {
                        if (item.ContainsKey("Score") && item.ContainsKey("Username"))
                        {
                            DateTime? time = item.UpdatedAt;
                            int daysOld = 0;
                            if (time != null)
                                daysOld = (DateTime.Now - time).Value.Days;
                            scores.Add(new GenericScore(item.Get<string>("Username"), item.Get<int>("Score"), daysOld, item.Get<string>("UserID")));
                        }
                    }
                    TopCallback(scr, scores.ToArray());
                }
                else
                {
                    foreach (var e in t.Exception.InnerExceptions)
                    {
                        ParseException parseException = (ParseException)e;
                        Debug.Log("Error getting score list: " + parseException.Message);
                    }
                    TopCallback(scr, scores.ToArray());
                }  
            });
        }
        catch
        {
            Debug.Log("Error getting parse (catch)");
            TopCallback(scr, scores.ToArray());
        }
        */
    }

    static void TopCallback(ScoreManager scr, GenericScore[] scores)
    {
        scr.TopScoresCB(scores);
    }

    public static void SaveNewScore(ScoreManager scr, string ScoreClass, int score, int MaxAge, string UserID="", string ScoreName="")
    {
        Debug.LogWarning("SaveNewScore not implemented yet, will need to hook up to psn leaderboards");
        scr.SavedScores(false);
        /*
        try
        {
            string curVersion = Application.version;
            if (ParseUser.CurrentUser != null // && SteamManager.Initialized
                )
            {
                string id = UserID;
                if (id.Length < 5)
                {
                    id = ParseUser.CurrentUser.Username;
                    if (User.ArcadeMode && User.ArcadeUser != null)
                        id = id+"_"+User.ArcadeUser.Get<string>("Name");
                }  
                string sname = ScoreName;
                if (sname.Length < 1)
                {
                    sname = ParseUser.CurrentUser.Get<string>("name");
                    if (User.ArcadeMode && User.ArcadeUser != null)
                        sname = User.ArcadeUser.Get<string>("Name");
                }
                var query = ParseObject.GetQuery(ScoreClass).WhereEqualTo("UserID", id);
                query = query.WhereEqualTo("BoardID", LeaderboardID());
                query.FirstAsync().ContinueWith(t =>
                {
                    if (!t.IsFaulted && !t.IsCanceled)
                    {
                        var myScore = t.Result;
                        int highscore = 0;
                        string versionEarned = "0.0.0";
                        if (myScore.ContainsKey("Score"))
                            highscore = myScore.Get<int>("Score");
                        if (myScore.ContainsKey("VersionEarned"))
                            versionEarned = myScore.Get<string>("VersionEarned");
                        DateTime? updatedAt = myScore.UpdatedAt;
                        bool outDated = false;
                        if (MaxAge > 0)
                            outDated = (DateTime.UtcNow.AddDays(-1 * MaxAge) > updatedAt);
                        if (score > highscore || outDated)
                        {
                            myScore["Score"] = score;
                            myScore["Username"] = sname;
                            myScore["VersionEarned"] = curVersion;
                            myScore["BoardID"] = LeaderboardID();
                            myScore.SaveAsync().ContinueWith(q =>
                            {
                                if (!q.IsFaulted && !q.IsCanceled)
                                {
                                    scr.SavedScores(true);
                                }
                                else
                                {
                                    scr.SavedScores(false);
                                    foreach (var e in q.Exception.InnerExceptions)
                                    {
                                        ParseException parseException = (ParseException)e;
                                        Debug.LogError("Error Saving Score: " + parseException.Message);
                                    }
                                }
                            });
                        }
                        else
                        {
                            Debug.Log("Score did not fit criteria to overwrite");
                            scr.SavedScores(true);
                        }
                    }
                    else //Create new entry and try saving
                    {
                        ParseObject gameScore = new ParseObject(ScoreClass);
                        gameScore["Score"] = score;
                        gameScore["UserID"] = id;
                        gameScore["Username"] = sname;
                        gameScore["VersionEarned"] = curVersion;
                        gameScore["BoardID"] = LeaderboardID();
                        gameScore.SaveAsync().ContinueWith(s =>
                        {
                            if (!s.IsFaulted && !s.IsCanceled) //Success
                                scr.SavedScores(true);
                            else
                            {
                                foreach (var e in s.Exception.InnerExceptions)
                                {
                                    ParseException parseException = (ParseException)e;
                                    Debug.LogError("Error saving new score: " + parseException.Message);
                                }
                                scr.SavedScores(false);
                            }
                        });
                    }
                });
            }
            else
            {
                Debug.LogError("User not initialized, could not save score");
                scr.SavedScores(false);
            }
        }
        catch
        {
            scr.SavedScores(false);
            Debug.LogError("Error getting scores from parse (Catch)");
        }
        */
    }

    public static string[] GetMPValues()
    {
        string[] vals = new string[2] { "", "" };
        if (!PhotonNetwork.inRoom)
            return vals;
        List<string> UserIDs = new List<string>();
        List<string> Names = new List<string>();
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            object sid = "";
            p.customProperties.TryGetValue("UserID", out sid);
            if(sid != null)
            {
                string UserID = sid.ToString();
                if (UserID.Length > 0)
                {
                    UserIDs.Add(UserID);
                    Names.Add(p.name);
                }
            }
        }
        if (UserIDs.Count > 0)
        {
            UserIDs.Sort();
            Names.Sort();
            string fullID = "";
            string fullName = "";
            foreach (var v in UserIDs)
            {
                fullID += v;
            }
            foreach(var v in Names)
            {
                fullName += v + ", ";
            }
            vals[0] = fullID;
            if(fullName.Length > 3)
                vals[1] = fullName.Substring(0, fullName.Length-2);
        }
        return vals;
    }

    public static string LeaderboardID()
    {
        if(AppBase.v != null)
        {
            string s = AppBase.v.LeaderboardID;
            if (s.Length > 0)
                return s;
            else
                return AppBase.v.isBeta ? "Beta" : "Main";
        }
        return "None";
    }
}