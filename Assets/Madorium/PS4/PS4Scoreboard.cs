using Sony.NP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//using UnityEngine.PS4;

[RequireComponent(typeof(PS4Common))]
public class PS4Scoreboard : MonoBehaviour
{
#if UNITY_PS4    
    static PS4Scoreboard _inst;

    public static PS4Scoreboard Inst { get { return _inst; } }

    // Use this for initialization
    IEnumerator Start()
    {
        if (_inst == null)
        {
            _inst = this;
            if (!PS4NpInit.Initialised)
                yield return new WaitForEndOfFrame();
        }
    }

    public void SetScore(uint scoreboard, long score, string comment)
    {
        try
        {
            Debug.LogFormat("SetScore({0},{1},{2})", scoreboard, score, comment);
            Sony.NP.Ranking.SetScoreRequest request = new Sony.NP.Ranking.SetScoreRequest();

            request.UserId = PS4Common.InitialUserId;
            request.Score = score;
            request.Comment = "";
            request.GameInfoData = Encoding.UTF8.GetBytes(comment);
            request.BoardId = scoreboard;

            Sony.NP.Ranking.TempRankResponse response = new Sony.NP.Ranking.TempRankResponse();
            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Ranking.SetScore(request, response);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    RequestCallbacks<Sony.NP.Ranking.RangeOfRanksResponse> _readScoresReq = new RequestCallbacks<Sony.NP.Ranking.RangeOfRanksResponse>();

    // Gets the top ten ranks
    public void GetTopRanks(uint scoreboard, Action<Sony.NP.Ranking.RangeOfRanksResponse> callback, uint firstRank = 1, uint range = 10)
    {
        try
        {
            Sony.NP.Ranking.GetRangeOfRanksRequest request = new Sony.NP.Ranking.GetRangeOfRanksRequest();

            request.UserId = PS4Common.InitialUserId;
            request.BoardId = scoreboard;
            request.Range = range;
            request.StartRank = firstRank;

            Sony.NP.Ranking.RangeOfRanksResponse response = new Sony.NP.Ranking.RangeOfRanksResponse();

            // Make the async call which will return the Request Id 
            _readScoresReq.Add(callback, response);
            int requestId = Sony.NP.Ranking.GetRangeOfRanks(request, response);
            OnScreenLog.Add("GetRangeOfRanks Async : Request Id = " + requestId);            
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    void OnRangeOfRanks(Sony.NP.Ranking.RangeOfRanksResponse response)
    {
        _readScoresReq.Complete(response);
        if (response.RankData != null)
        {
            OnScreenLog.Add("Dump ranks");
            for (UInt32 i = 0; i < response.RankData.Length; i++)
            {
                var d = response.RankData[i];
                string gameInfo = "";
                if(d.GameInfo!=null)
                {
                    gameInfo = "GameInfoLength:" + d.GameInfo.Length;
                }
                else
                {
                    gameInfo = "NoGameInfo";
                }
                OnScreenLog.Add(string.Format("{0} {1} {2} {3} {4} hasGameData:{5} gameInfo:{6}", d.Comment, d.OnlineId.Name, d.Rank, d.ScoreValue, d.RecordDate, d.HasGameData, gameInfo) );
            }
        }
    }

    class GetUsersEntry
    {
        public Action<Sony.NP.Ranking.UsersRanksResponse, Sony.NP.Core.UserServiceUserId> Complete;
        public Sony.NP.Ranking.UsersRanksResponse Response;
    }

    List<GetUsersEntry> _getUsersRequests = new List<GetUsersEntry>();

    // Get rank of active user to set own high score
    public void GetActiveUserRank(uint boardId, Action<Sony.NP.Ranking.UsersRanksResponse, Sony.NP.Core.UserServiceUserId> onComplete)
    {
        try
        {
            Sony.NP.Ranking.GetUsersRanksRequest request = new Sony.NP.Ranking.GetUsersRanksRequest();
            request.UserId = PS4Common.InitialUserId;
            request.BoardId = boardId;
            
            List<Sony.NP.Ranking.ScoreAccountIdPcId> scoreAccounts = new List<Sony.NP.Ranking.ScoreAccountIdPcId>();
            Sony.NP.Ranking.ScoreAccountIdPcId newId;
            newId.accountId = SonyNpUserProfiles.GetLocalAccountId(PS4Common.InitialUserId);
            newId.pcId = Sony.NP.Ranking.MIN_PCID;
            scoreAccounts.Add(newId);

            // Assign the array of ids, to lookup on the Ranking server.
            request.Users = scoreAccounts.ToArray();

            Sony.NP.Ranking.UsersRanksResponse response = new Sony.NP.Ranking.UsersRanksResponse();

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Ranking.GetUsersRanks(request, response);
            OnScreenLog.Add("GetActiveUserRank Async : Request Id = " + requestId);
            _getUsersRequests.Add(new GetUsersEntry() { Complete = onComplete, Response = response });
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void OnGetUsersRanks(Sony.NP.Ranking.UsersRanksResponse response, Sony.NP.Core.UserServiceUserId serviceUserId)
    {
        var req = _getUsersRequests.Find((entry) => { return entry.Response.Equals(response); });
        _getUsersRequests.Remove(req);
        if (null != req.Complete)
            req.Complete(req.Response, serviceUserId);
    }

    void OnSetScore(Sony.NP.Ranking.TempRankResponse resp)
    {
        Debug.LogFormat("RankingSetScore response {0}", resp.TempRank);
        if (resp == null)
            return;

        OnScreenLog.Add("OutputSetScore Response");

        if (resp.Locked == false)
        {
            OnScreenLog.Add("Temporary Rank : " + resp.TempRank);
            if (resp.ReturnCodeValue >= 0)
            {
                OnScreenLog.Add("No error");
            }
            else
            {
                switch(resp.ReturnCode)
                {
                    case Sony.NP.Core.ReturnCodes.NP_COMMUNITY_SERVER_ERROR_NOT_BEST_SCORE:
                        break;
                    default:
                        Sony.PS4.Dialog.Common.ShowErrorMessage((uint)resp.ReturnCodeValue);
                        break;
                }
            }
        }
    }

    public void OnAsyncEvent(Sony.NP.NpCallbackEvent callbackEvent)
    {
        if (callbackEvent.Service == Sony.NP.ServiceTypes.Ranking)
        {
            switch (callbackEvent.ApiCalled)
            {
                case Sony.NP.FunctionTypes.RankingSetScore:
                    OnSetScore(callbackEvent.Response as Sony.NP.Ranking.TempRankResponse);
                    //OutputSetScore(callbackEvent.Response as Sony.NP.Ranking.TempRankResponse);
                    break;
                case Sony.NP.FunctionTypes.RankingGetRangeOfRanks:
                    //OutputRangeOfRanks(callbackEvent.Response as Sony.NP.Ranking.RangeOfRanksResponse);
                    OnRangeOfRanks(callbackEvent.Response as Sony.NP.Ranking.RangeOfRanksResponse);
                    break;
                case Sony.NP.FunctionTypes.RankingGetFriendsRanks:
                    //OutputFriendsRank(callbackEvent.Response as Sony.NP.Ranking.FriendsRanksResponse);
                    break;
                case Sony.NP.FunctionTypes.RankingGetUsersRanks:
                    //OutputUsersRank(callbackEvent.Response as Sony.NP.Ranking.UsersRanksResponse, callbackEvent.UserId);
                    OnGetUsersRanks(callbackEvent.Response as Sony.NP.Ranking.UsersRanksResponse, callbackEvent.UserId);
                    break;
                case Sony.NP.FunctionTypes.RankingSetGameData:
                    //OutputSetGameDataResult(callbackEvent.Response as Sony.NP.Ranking.SetGameDataResultResponse);
                    break;
                case Sony.NP.FunctionTypes.RankingGetGameData:
                    //OutputGetGameDataResult(callbackEvent.Response as Sony.NP.Ranking.GetGameDataResultResponse);
                    break;
                default:
                    break;
            }
        }
    }

#endif
}