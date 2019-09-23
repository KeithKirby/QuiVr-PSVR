using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

//#define TEST_TROPHY
namespace Sony.NP
{
    public class PS4TrophyManager : MonoBehaviour
    {
        static PS4TrophyManager _inst;

        static public PS4TrophyManager GetInst() { return _inst; }

        public PS4AchievementMap AchievementMap;

        public void UnlockTrophy(string quivrAchievementId)
        {
#if UNITY_PS4
            try
            {
                var id = AchievementMap.GetTrophyId(quivrAchievementId);
                if (-1 != id)
                {
                    Sony.NP.Trophies.UnlockTrophyRequest request = new Sony.NP.Trophies.UnlockTrophyRequest();
                    request.TrophyId = id;
                    request.UserId = _primaryUserId;

                    Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

                    // Make the async call which will return the Request Id 
                    int requestId = Sony.NP.Trophies.UnlockTrophy(request, response);
                    DLog.Log(DLFilter.Trophies, "GetUnlockedTrophies Async : Request Id = " + requestId);
                }
                else
                {
                    DLog.Log(DLFilter.Trophies, "UnlockTrophy {0} not supported on ps4 ", id);
                }
            }
            catch (Sony.NP.NpToolkitException e)
            {
                DLog.Log(DLFilter.Trophies, "Exception : " + e.ExtendedMessage);
            }
#endif
        }

#if UNITY_PS4

        void Start()
        {
            if (null != _inst)
                throw new Exception("Tried to instantiate second trophy manager");

            _inst = this;
            //AppReady.RequestReadyCallback(OnAppReady, "PS4TrophyManager");
            StartCoroutine(RegisterTrophyPack());
        }

        void OnAppReady()
        {
            //StartCoroutine(RegisterTrophyPack());
        }

        private string Join(int[] nums)
        {
            var ret = "";
            for (int i = 0; i < nums.Length; ++i)
            {
                if (i > 0)
                    ret += ",";
                ret += nums[i];
            }
            return ret;
        }

        private void DumpPadState()
        {
            var primaryHandles = new int[2];
            var secondaryHandles = new int[2];
            var ret = PS4Input.MoveGetUsersMoveHandles(2, primaryHandles, secondaryHandles);

            var txt = "";
            txt += string.Format("NumHandles {0}\n", ret);
            txt += string.Format("Primary: {0}\n", Join(primaryHandles));
            txt += string.Format("Secondary: {0}\n", Join(secondaryHandles));

            DLog.Log(DLFilter.Trophies, txt);
        }

        PS4Input.LoggedInUser? FindUserByMoveHandles(int primary, int secondary)
        {
            if (primary != -1 && secondary != -1)
            {
                for (int i = 0; i < 4; ++i)
                {
                    var user = PS4Input.GetUsersDetails(i);
                    if (user.move0Handle == primary && user.move1Handle == secondary)
                        return user;
                }
            }
            return null;
        }

        private void CheckForUserChanged()
        {
            int activeUser = -1;
            for (int i = 0; i < 4; ++i)
            {
                var user = PS4Input.GetUsersDetails(i);
                if (user.primaryUser)
                {
                    if (user.move0Handle != -1 && user.move1Handle != -1)
                    {
                        activeUser = user.userId;
                    }
                }
            }
            ActiveUserID = activeUser;
        }

        public int ActiveUserID
        {
            set
            {
                if (_primaryUserId != value)
                {
                    _primaryUserId = value;
                    DLog.Log(DLFilter.General, "Active user changed: '" + _primaryUserId + "'");
                }
            }
        }
        int _primaryUserId = -1;

        public IEnumerator RegisterTrophyPack()
        {
            DLog.Log(DLFilter.Trophies, "TrophyManager waiting for move controllers");

            while (-1 == _primaryUserId)
                yield return new WaitForSeconds(1.0f);
            DLog.Log(DLFilter.Trophies, "Registering trophy pack '" + _primaryUserId + "'");

            Sony.NP.Trophies.RegisterTrophyPackRequest request = new Sony.NP.Trophies.RegisterTrophyPackRequest();
            request.UserId = _primaryUserId;

            Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

            try
            {
                // Make the async call which will return the Request Id 
                int requestId = Sony.NP.Trophies.RegisterTrophyPack(request, response);
                DLog.Log(DLFilter.Trophies, "RegisterTrophyPack Async : Request Id = " + requestId);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                DLog.Log(DLFilter.Trophies, "Exception : " + e.ExtendedMessage);
            }
        }


        public void Update()
        {
            CheckForUserChanged();
#if TEST_TROPHY
        TestTrophy();
#endif
        }

#if TEST_TROPHY
    bool _nutzDown = false;
    bool _bullzeyeDown = false;
    void TestTrophy()
    {
        if ((PS4Input.MoveGetButtons(0, 0) & (int)MoveGetButtons.Circle)!=0)
        {
            if (!_nutzDown)
            {
                _nutzDown = true;
                UnlockTrophy(DiscoveryID.Nutz);
            }
            
        }
        else
        {
            _nutzDown = false;
        }
        if ((PS4Input.MoveGetButtons(0, 0) & (int)MoveGetButtons.Square)!=0)
        {
            if (!_bullzeyeDown)
            {
                _bullzeyeDown = true;
                UnlockTrophy(DiscoveryID.Bullzeye);
            }   
        }
        else
        {
            _bullzeyeDown = false;
        }
    }
#endif

        public void SetScreenshot()
        {
            try
            {
                Sony.NP.Trophies.SetScreenshotRequest request = new Sony.NP.Trophies.SetScreenshotRequest();
                request.AssignToAllUsers = true;
                request.UserId = _primaryUserId;

                int[] ids = new int[4];

                for (int i = 0; i < ids.Length; i++)
                {
                    ids[i] = i + 1;  // Set throphy ids from 1 to 4.
                }

                request.TrophiesIds = ids;

                Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

                // Make the async call which will return the Request Id 
                int requestId = Sony.NP.Trophies.SetScreenshot(request, response);
                DLog.Log(DLFilter.Trophies, "SetScreenshot Async : Request Id = " + requestId);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                DLog.Log(DLFilter.Trophies, "Exception : " + e.ExtendedMessage);
            }
        }

        public void GetUnlockedTrophies()
        {
            try
            {
                Sony.NP.Trophies.GetUnlockedTrophiesRequest request = new Sony.NP.Trophies.GetUnlockedTrophiesRequest();
                request.UserId = _primaryUserId;

                Sony.NP.Trophies.UnlockedTrophiesResponse response = new Sony.NP.Trophies.UnlockedTrophiesResponse();

                // Make the async call which will return the Request Id 
                int requestId = Sony.NP.Trophies.GetUnlockedTrophies(request, response);
                DLog.Log(DLFilter.Trophies, "GetUnlockedTrophies Async : Request Id = " + requestId);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                DLog.Log(DLFilter.Trophies, "Exception : " + e.ExtendedMessage);
            }
        }

        public void DisplayTrophyPackDialog()
        {
            try
            {
                Sony.NP.Trophies.DisplayTrophyListDialogRequest request = new Sony.NP.Trophies.DisplayTrophyListDialogRequest();
                request.UserId = _primaryUserId;

                Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

                // Make the async call which will return the Request Id 
                int requestId = Sony.NP.Trophies.DisplayTrophyListDialog(request, response);
                DLog.Log(DLFilter.Trophies, "DisplayTrophyPackDialog Async : Request Id = " + requestId);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                DLog.Log(DLFilter.Trophies, "Exception : " + e.ExtendedMessage);
            }
        }

        public void GetTrophyPackSummary()
        {
            try
            {
                Sony.NP.Trophies.GetTrophyPackSummaryRequest request = new Sony.NP.Trophies.GetTrophyPackSummaryRequest();
                request.RetrieveTrophyPackSummaryIcon = true;
                request.UserId = _primaryUserId;

                Sony.NP.Trophies.TrophyPackSummaryResponse response = new Sony.NP.Trophies.TrophyPackSummaryResponse();

                // Make the async call which will return the Request Id 
                int requestId = Sony.NP.Trophies.GetTrophyPackSummary(request, response);
                DLog.Log(DLFilter.Trophies, "GetTrophyPackSummary Async : Request Id = " + requestId);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                DLog.Log(DLFilter.Trophies, "Exception : " + e.ExtendedMessage);
            }
        }

        public void GetTrophyPackGroup()
        {
            try
            {
                Sony.NP.Trophies.GetTrophyPackGroupRequest request = new Sony.NP.Trophies.GetTrophyPackGroupRequest();
                request.GroupId = -1;
                request.UserId = _primaryUserId;

                Sony.NP.Trophies.TrophyPackGroupResponse response = new Sony.NP.Trophies.TrophyPackGroupResponse();

                // Make the async call which will return the Request Id 
                int requestId = Sony.NP.Trophies.GetTrophyPackGroup(request, response);
                DLog.Log(DLFilter.Trophies, "GetTrophyPackGroup Async : Request Id = " + requestId);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                DLog.Log(DLFilter.Trophies, "Exception : " + e.ExtendedMessage);
            }
        }

        public void GetTrophyPackTrophy()
        {
            try
            {
                Sony.NP.Trophies.GetTrophyPackTrophyRequest request = new Sony.NP.Trophies.GetTrophyPackTrophyRequest();
                request.TrophyId = 1;
                request.RetrieveTrophyPackTrophyIcon = true;
                request.UserId = _primaryUserId;

                Sony.NP.Trophies.TrophyPackTrophyResponse response = new Sony.NP.Trophies.TrophyPackTrophyResponse();

                // Make the async call which will return the Request Id 
                int requestId = Sony.NP.Trophies.GetTrophyPackTrophy(request, response);
                DLog.Log(DLFilter.Trophies, "GetTrophyPackTrophy Async : Request Id = " + requestId);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                DLog.Log(DLFilter.Trophies, "Exception : " + e.ExtendedMessage);
            }
        }

        public void OnAsyncEvent(Sony.NP.NpCallbackEvent callbackEvent)
        {
            if (callbackEvent.Service == Sony.NP.ServiceTypes.Trophy)
            {
                switch (callbackEvent.ApiCalled)
                {
                    case Sony.NP.FunctionTypes.TrophyRegisterTrophyPack:
                        {
                            User user = User.FindUser(callbackEvent.UserId);

                            if (user != null)
                            {
                                user.trophyPackRegistered = true;
                            }

                            OutputRegisterTrophyPack(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                        }
                        break;
                    case Sony.NP.FunctionTypes.TrophySetScreenshot:
                        OutputSetScreenshot(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                        break;
                    case Sony.NP.FunctionTypes.TrophyUnlock:
                        OutputTrophyUnlock(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                        break;
                    case Sony.NP.FunctionTypes.TrophyGetUnlockedTrophies:
                        OutputUnlockedTrophies(callbackEvent.Response as Sony.NP.Trophies.UnlockedTrophiesResponse);
                        break;
                    case Sony.NP.FunctionTypes.TrophyDisplayTrophyListDialog:
                        OutputDisplayTrophyListDialog(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                        break;
                    case Sony.NP.FunctionTypes.TrophyGetTrophyPackSummary:
                        OutputGetTrophyPackSummary(callbackEvent.Response as Sony.NP.Trophies.TrophyPackSummaryResponse);
                        break;
                    case Sony.NP.FunctionTypes.TrophyGetTrophyPackGroup:
                        OutputGetTrophyPackGroup(callbackEvent.Response as Sony.NP.Trophies.TrophyPackGroupResponse);
                        break;
                    case Sony.NP.FunctionTypes.TrophyGetTrophyPackTrophy:
                        OutputGetTrophyPackTrophy(callbackEvent.Response as Sony.NP.Trophies.TrophyPackTrophyResponse);
                        break;
                    default:
                        break;
                }
            }
        }

        private void OutputRegisterTrophyPack(Sony.NP.Core.EmptyResponse response)
        {
            if (response == null) return;

            DLog.Log(DLFilter.Trophies, "RegisterTrophyPack Empty Response");

            if (response.Locked == false)
            {
            }
        }

        private void OutputSetScreenshot(Sony.NP.Core.EmptyResponse response)
        {
            if (response == null) return;

            DLog.Log(DLFilter.Trophies, "SetScreenshot Empty Response");

            if (response.Locked == false)
            {

            }
        }

        private void OutputTrophyUnlock(Sony.NP.Core.EmptyResponse response)
        {
            if (response == null) return;

            DLog.Log(DLFilter.Trophies, "TrophyUnlock Empty Response");

            if (response.Locked == false)
            {

            }
        }

        private void OutputUnlockedTrophies(Sony.NP.Trophies.UnlockedTrophiesResponse response)
        {
            if (response == null) return;

            DLog.Log(DLFilter.Trophies, "GetUnlockedTrophies Response");

            if (response.Locked == false)
            {
                if (response.TrophyIds != null)
                {
                    DLog.Log(DLFilter.Trophies, "Number Unlocked Trophys = " + response.TrophyIds.Length);
                    for (int i = 0; i < response.TrophyIds.Length; i++)
                    {
                        DLog.Log(DLFilter.Trophies, "   : " + response.TrophyIds[i]);
                    }
                }
            }
        }

        private void OutputDisplayTrophyListDialog(Sony.NP.Core.EmptyResponse response)
        {
            if (response == null) return;

            DLog.Log(DLFilter.Trophies, "DisplayTrophyListDialog Empty Response");

            if (response.Locked == false)
            {

            }
        }

        private void OutputGetTrophyPackSummary(Sony.NP.Trophies.TrophyPackSummaryResponse response)
        {
            if (response == null) return;

            DLog.Log(DLFilter.Trophies, "TrophyPackSummaryResponse Response");

            if (response.Locked == false)
            {
                SonyNpMain.SetIconTexture(response.Icon);

                DLog.Log(DLFilter.Trophies, "Static Configuration");

                DLog.Log(DLFilter.Trophies, "   # Groups : " + response.StaticConfiguration.NumGroups);
                DLog.Log(DLFilter.Trophies, "   # Trophies : " + response.StaticConfiguration.NumTrophies);
                DLog.Log(DLFilter.Trophies, "   # Platinum : " + response.StaticConfiguration.NumPlatinum);
                DLog.Log(DLFilter.Trophies, "   # Gold : " + response.StaticConfiguration.NumGold);
                DLog.Log(DLFilter.Trophies, "   # Silver : " + response.StaticConfiguration.NumSilver);
                DLog.Log(DLFilter.Trophies, "   # Bronze : " + response.StaticConfiguration.NumBronze);
                DLog.Log(DLFilter.Trophies, "   Title : " + response.StaticConfiguration.Title);
                DLog.Log(DLFilter.Trophies, "   Description : " + response.StaticConfiguration.Description);

                DLog.Log(DLFilter.Trophies, "User Progress");

                DLog.Log(DLFilter.Trophies, "   Unlocked Trophies : " + response.UserProgress.UnlockedTrophies);
                DLog.Log(DLFilter.Trophies, "   Unlocked Platinum : " + response.UserProgress.UnlockedPlatinum);
                DLog.Log(DLFilter.Trophies, "   Unlocked Gold : " + response.UserProgress.UnlockedGold);
                DLog.Log(DLFilter.Trophies, "   Unlocked Silver : " + response.UserProgress.UnlockedSilver);
                DLog.Log(DLFilter.Trophies, "   Unlocked Bronze : " + response.UserProgress.UnlockedBronze);
                DLog.Log(DLFilter.Trophies, "   Progress % : " + response.UserProgress.ProgressPercentage);
            }
        }

        private void OutputGetTrophyPackGroup(Sony.NP.Trophies.TrophyPackGroupResponse response)
        {
            if (response == null) return;

            DLog.Log(DLFilter.Trophies, "TrophyPackGroupResponse Response");

            if (response.Locked == false)
            {
                SonyNpMain.SetIconTexture(response.Icon);

                DLog.Log(DLFilter.Trophies, "Static Configuration");

                DLog.Log(DLFilter.Trophies, "   Group Id : " + response.StaticConfiguration.GroupId);
                DLog.Log(DLFilter.Trophies, "   # Trophies : " + response.StaticConfiguration.NumTrophies);
                DLog.Log(DLFilter.Trophies, "   # Platinum : " + response.StaticConfiguration.NumPlatinum);
                DLog.Log(DLFilter.Trophies, "   # Gold : " + response.StaticConfiguration.NumGold);
                DLog.Log(DLFilter.Trophies, "   # Silver : " + response.StaticConfiguration.NumSilver);
                DLog.Log(DLFilter.Trophies, "   # Bronze : " + response.StaticConfiguration.NumBronze);
                DLog.Log(DLFilter.Trophies, "   Title : " + response.StaticConfiguration.Title);
                DLog.Log(DLFilter.Trophies, "   Description : " + response.StaticConfiguration.Description);

                DLog.Log(DLFilter.Trophies, "User Progress");

                DLog.Log(DLFilter.Trophies, "   Group Id : " + response.UserProgress.GroupId);
                DLog.Log(DLFilter.Trophies, "   Unlocked Trophies : " + response.UserProgress.UnlockedTrophies);
                DLog.Log(DLFilter.Trophies, "   Unlocked Platinum : " + response.UserProgress.UnlockedPlatinum);
                DLog.Log(DLFilter.Trophies, "   Unlocked Gold : " + response.UserProgress.UnlockedGold);
                DLog.Log(DLFilter.Trophies, "   Unlocked Silver : " + response.UserProgress.UnlockedSilver);
                DLog.Log(DLFilter.Trophies, "   Unlocked Bronze : " + response.UserProgress.UnlockedBronze);
                DLog.Log(DLFilter.Trophies, "   Progress % : " + response.UserProgress.ProgressPercentage);
            }
        }

        private void OutputGetTrophyPackTrophy(Sony.NP.Trophies.TrophyPackTrophyResponse response)
        {
            if (response == null) return;

            DLog.Log(DLFilter.Trophies, "TrophyPackTrophyResponse Response");

            if (response.Locked == false)
            {
                SonyNpMain.SetIconTexture(response.Icon);

                DLog.Log(DLFilter.Trophies, "Static Configuration");

                DLog.Log(DLFilter.Trophies, "   Trophy Id : " + response.StaticConfiguration.TrophyId);
                DLog.Log(DLFilter.Trophies, "   Trophy Grade : " + response.StaticConfiguration.TrophyGrade);
                DLog.Log(DLFilter.Trophies, "   Group Id : " + response.StaticConfiguration.GroupId);
                DLog.Log(DLFilter.Trophies, "   Hidden : " + response.StaticConfiguration.Hidden);
                DLog.Log(DLFilter.Trophies, "   Name : " + response.StaticConfiguration.Name);
                DLog.Log(DLFilter.Trophies, "   Description : " + response.StaticConfiguration.Description);

                DLog.Log(DLFilter.Trophies, "User Progress");

                DLog.Log(DLFilter.Trophies, "   Trophy Id : " + response.UserProgress.TrophyId);
                DLog.Log(DLFilter.Trophies, "   Unlocked : " + response.UserProgress.Unlocked);
                DLog.Log(DLFilter.Trophies, "   Date Stamp : " + response.UserProgress.Timestamp.ToString());
            }
        }
#endif
    }
}