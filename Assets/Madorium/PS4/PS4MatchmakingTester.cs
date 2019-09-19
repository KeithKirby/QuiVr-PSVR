using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PS4MatchmakingTester : MonoBehaviour
{
#if UNITY_PS4
    PS4MatchmakingHelper _helper;
    PS4Invitation _invitation;
    PS4Scoreboard _score;

    // Use this for initialization
    void Start ()
    {
        _helper = GetComponent<PS4MatchmakingHelper>();
        _invitation = GameObject.FindObjectOfType<PS4Invitation>();
        _score = GameObject.FindObjectOfType<PS4Scoreboard>();
    }

    public void JoinRandomOrCreate()
    {
        _helper.JoinRandomOrCreate(OnJoinComplete);
    }

    void OnJoinComplete(bool success)
    {
        OnScreenLog.Add("JoinRandomOrCreate:" + (success ? "success" : "fail"));
    }

    void Update()
    {
        if(null!=_invitation.CurrentInvitation)
        {
            var sessionId = _invitation.CurrentInvitation.SessionId;
            _helper.JoinPSNRoom(sessionId, OnJoinComplete);
            _invitation.ClearInvitation();
        }
    }

    public void SetScore()
    {
        _score.SetScore(0, 101, "Hello");
    }

    public void ReadScores()
    {
        _score.GetTopRanks(0, null);
    }

    public void GetUserScores()
    {
        _score.GetActiveUserRank(0,
            (response, userId) =>
            {
                //OnScreenLog.Add("Score: NpId " + user.NpId + " Score " + user.ScoreValue + "Comment" + user.Comment);
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
                            OnScreenLog.Add("Score: OnlineId " + user.OnlineId.Name + " Score " + user.ScoreValue + "Comment" + user.Comment);
                        }
                        else
                        {
                            OnScreenLog.Add("No data");
                        }
                    }
                }

                OnScreenLog.Add("BoardId:" + response.BoardId + " " + response);
            });
    }

    public void GetUserScoresInvalid()
    {
        _score.GetActiveUserRank(4,
            (response, userId) =>
            {
                //OnScreenLog.Add("Score: NpId " + user.NpId + " Score " + user.ScoreValue + "Comment" + user.Comment);
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
            });
    }

    public void CheckAvailabilityAll()
    {
        PS4Plus.Inst.CheckAvailabilityAll(null);
    }
#endif
}