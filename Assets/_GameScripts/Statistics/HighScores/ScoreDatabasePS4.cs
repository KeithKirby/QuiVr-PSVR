using UnityEngine;
using System.Collections;
//using Parse;
using System.Collections.Generic;
using System;
using Sony.NP;
using System.Text;

public class ScoreDatabasePS4 : ScoreDatabase
{
    public enum ScoreClass
    {
        SinglePlayer = 0,
        Multiplayer = 1,
        ArcheryRange = 2,
        OwlInvaders = 3
    }

    static Dictionary<string, ScoreClass> NameToClass = new Dictionary<string, ScoreClass>
        {
            {"FFKDiff", ScoreClass.SinglePlayer },
            {"CanyonMPDiff", ScoreClass.Multiplayer },
            {"ArcheryRange01", ScoreClass.ArcheryRange },
            {"OwlInvaders", ScoreClass.OwlInvaders }
        };

    public class ScoreClassName
    {
        public string Name;
        public ScoreClass Class;
    }

    long[] _highScores = new long[] { 0, 0, 0, 0 };
    List<Action> _pendingActions = new List<Action>();

    void Awake()
    {
        if (ScoreDatabase.Inst)
            throw new Exception("ScoreDatabase created twice");        
        ScoreDatabase.Inst = this;
        if (!PS4Scoreboard.Inst)
            return;
    }

    void OnGotUserScore(Sony.NP.Ranking.UsersRanksResponse response, Sony.NP.Core.UserServiceUserId userId)
    {
        if (Sony.NP.Core.ReturnCodes.SUCCESS == response.ReturnCode)
        {
            if (response.IsCrossSaveInformation == true)
            {
                for (ulong i = 0; i < response.NumUsers; i++)
                {
                    var user = response.UsersForCrossSave[i];
                    if (user.HasData)
                    {
                        OnScreenLog.Add("Score: NpId " + user.NpId + " Score " + user.ScoreValue + "Comment" + user.Comment);
                    }
                    else
                    {
                        OnScreenLog.Add("No data");
                    }
                }
            }
            else
            {
                for (ulong i = 0; i < response.NumUsers; i++)
                {
                    var user = response.Users[i];
                    if (user.HasData)
                    {   
                        _highScores[response.BoardId] = user.ScoreValue;
                        OnScreenLog.Add("Score: OnlineId " + user.OnlineId.Name + " Score " + user.ScoreValue + "Comment" + user.Comment);
                    }
                    else
                    {
                        OnScreenLog.Add("No data");
                    }
                }
            }
        }
        OnScreenLog.Add("BoardId:" + response.BoardId + " " + response);
    }

    override public void RequestScore(ScoreManager scr, string ScoreClass, int MaxAge, string UserID = "")
    {
        var uid = UserID.Length > 0 ? UserID : PS4Common.InitialUserDetals.userName;
        Debug.Log("RequestScore");
        if (NameToClass.ContainsKey(ScoreClass))
        {
            var sc = NameToClass[ScoreClass];
            PS4Scoreboard.Inst.GetActiveUserRank((uint)sc,
                (Sony.NP.Ranking.UsersRanksResponse response, Sony.NP.Core.UserServiceUserId serviceUserId) =>
                {
                    OnGotUserScore(response, serviceUserId);
                    List<GenericScore> scores = new List<GenericScore>();
                    if(response.NumUsers==1)
                    {
                        var user = response.Users[0];
                        if (user.HasData)
                        {
                            _pendingActions.Add(() =>
                            {
                                Debug.LogFormat("RequestScore - OnlineId.Name:{0} Score:{1} Rank:{2}", user.OnlineId.Name, user.ScoreValue, user.Rank);
                                scr.SetHighScore((int)user.ScoreValue);
                            }); // Callback in update to prevent errors accessing objects on main thread
                            _highScores[response.BoardId] = user.ScoreValue;                       
                        }
                        else
                        {
                            Debug.LogFormat("RequestScore - No data");
                        }
                    }
                    else
                    {
                        Debug.LogFormat("There is no score for this user {0}", response.NumUsers);
                        _pendingActions.Add(() =>
                        {
                            scr.SetHighScore((int)0);
                        }); // Callback in update to prevent errors accessing objects on main thread
                        _highScores[response.BoardId] = 0;
                    }
                });
        }
        else
        {
            Debug.LogError("Key not found in NameToClass:" + ScoreClass);
        }
    }

    private void Update()
    {
        if (_pendingActions.Count > 0)
        {
            foreach(var a in _pendingActions)
                a();
        }
        _pendingActions.Clear();
    }

    List<Sony.NP.Core.NpAccountId> GetAccountIds(Ranking.ScoreRankData[] rankData)
    {
        HashSet<ulong> foundIds = new HashSet<ulong>();
        List<Sony.NP.Core.NpAccountId> accountIds = new List<Sony.NP.Core.NpAccountId>();
        for (UInt32 i = 0; i < rankData.Length; i++)
        {
            var rd = rankData[i];
            if (rd.GameInfo != null && rd.GameInfo.Length > 0)
            {
                var fullName = Encoding.UTF8.GetString(rd.GameInfo);
                var ids = fullName.Split(',');
                foreach (var id in ids)
                {
                    ulong ulid = ulong.Parse(id);
                    if (foundIds.Add(ulid))
                    {
                        accountIds.Add(new Sony.NP.Core.NpAccountId() { Id = ulid });
                    }
                }
            }
        }
        return accountIds;
    }

    Dictionary<ulong, string> BuildProfileMap(Profiles.Profile[] profs)
    {
        var dict = new Dictionary<ulong, string>();
        Debug.Log("BuildProfileMap");
        foreach (var p in profs)
        {
            Debug.LogFormat("Add {0} {1}", p.OnlineUser.AccountId.Id, p.OnlineUser.OnlineID.Name);
            dict.Add(p.OnlineUser.AccountId.Id, p.OnlineUser.OnlineID.Name);
        }
        return dict;
    }

    override public void TopList(ScoreManager scr, string ScoreClass, int limit, int MaxAge)
    {
        if (!PS4Scoreboard.Inst)
            return;
        
        if (NameToClass.ContainsKey(ScoreClass))
        {            
            PS4Scoreboard.Inst.GetTopRanks(
                (uint)NameToClass[ScoreClass],
                (response) =>
                {
                    if (response.RankData != null)
                    {
                        var now = DateTime.Now;
                        List<GenericScore> scores = new List<GenericScore>();
                        List<Sony.NP.Core.NpAccountId> accountIds = GetAccountIds(response.RankData);                        
                        if (accountIds.Count > 0)
                        {
                            PS4Profiles.Inst.RequestNpProfiles(accountIds,
                                (profilesResponse) =>
                                {
                                    var profileToName = BuildProfileMap(profilesResponse.Profiles);
                                    for (UInt32 i = 0; i < response.RankData.Length; i++)
                                    {
                                        var d = response.RankData[i];
                                        if(d.GameInfo!=null && d.GameInfo.Length > 0)
                                        {
                                            string text = Encoding.UTF8.GetString(d.GameInfo);                                            
                                            var aids = text.Split(',');
                                            var names = new List<string>();
                                            // Sort - the posting account is always first
                                            string posterAccountId = d.AccountId.Id.ToString();
                                            Array.Sort<String>(aids,
                                                (a,b) =>
                                                {
                                                    if (a == posterAccountId)
                                                        return -1;
                                                    if (b == posterAccountId)
                                                        return 1;
                                                    return a.CompareTo(b);
                                                });
                                            foreach (var a in aids)
                                                names.Add(profileToName[ulong.Parse(a)]);
                                            var scoreText = string.Join("\n", names.ToArray());                                            
                                            var score = new GenericScore(scoreText, (int)d.ScoreValue, (now - d.RecordDate).Days, d.OnlineId.Name);
                                            scores.Add(score);                                            
                                        }
                                    }
                                    scr.TopScoresCB(scores.ToArray());
                                });
                        }
                        else
                        {
                            scr.TopScoresCB(scores.ToArray());
                        }
                        
                    }
                });
        }
        else
        {
            Debug.LogError("Key not found in NameToClass:" + ScoreClass);
        }
    }

    override public void SaveNewScore(ScoreManager scr, string ScoreClass, int score, int MaxAge, string UserID="", string ScoreName="")
    {
        Debug.LogFormat("SaveNewScore ScoreClass:{0} score:{1} MaxAge:{2} UserID:{3} ScoreName:{4}", ScoreClass, score, MaxAge, UserID, ScoreName);
        if (PS4Scoreboard.Inst != null)
        {
            if (ScoreName.Length == 0)
            {
                throw new Exception("Score name not set");
            }

            if (NameToClass.ContainsKey(ScoreClass))
            {
                ScoreClass sc = NameToClass[ScoreClass];
                if (score > _highScores[(int)sc])
                {
                    _highScores[(int)sc] = score;
                    PS4Scoreboard.Inst.SetScore((uint)sc, score, ScoreName);
                }
            }
            else
            {
                Debug.LogError("Key not found in NameToClass:" + ScoreClass);
            }
        }
        else
        {
            Debug.LogFormat("SaveNewScore failed, no PS4Scoreboard");
        }
    }

    // Returns array of string
    // 0 - All IDs munged together
    // 1 - Display names comma separated
    override public string[] GetMPIdAndComment()
    {
        string[] vals = new string[2] { "", "" };
        if (!PhotonNetwork.inRoom)
            return vals;
        List<string> AccountIDs = new List<string>();
        List<string> Names = new List<string>();
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            object sid = "";
            string UserID = (string)p.CustomProperties["UserID"];
            string AccountID = (string)p.CustomProperties["AccountID"];
            string Name = (string)p.CustomProperties["DisplayName"];
            AccountIDs.Add(AccountID);
            Names.Add(Name);
        }
        if (AccountIDs.Count > 0)
        {
            AccountIDs.Sort();
            Names.Sort();
            string fullID = "";
            string fullName = "";

            string commaSepAccountIds = string.Join(",", AccountIDs.ToArray());
            for (int i=0;i<Names.Count;++i)
            {
                var n = Names[i];
                fullName += n;
                if (i < Names.Count - 1)
                    fullName += "\n";
            }
            vals[0] = fullID;
            vals[1] = commaSepAccountIds;
        }
        return vals;
    }
}