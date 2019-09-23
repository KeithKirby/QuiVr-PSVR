using Sony.NP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PS4Common))]
public class PS4Plus : MonoBehaviour
{
    public List<PS4User> Users;

    PS4User GetUserByID(Sony.NP.Core.UserServiceUserId userId)
    {
        foreach (var user in Users)
        {
            if (user.UserId.Id == userId.Id)
                return user;
        }
        throw new Exception("Failed to find user " + userId);
    }

    class CheckPlusRequest
    {
        public Sony.NP.NpUtils.CheckPlusResponse Response;
        public Action<bool> Callback;
    }
    List<CheckPlusRequest> _checkPlusRequests = new List<CheckPlusRequest>();

    // Check availability requests
    class CheckAvailabilityTrack
    {
        public Sony.NP.NpUtils.CheckAvailablityRequest Request;
        public Sony.NP.Core.EmptyResponse Response;
    }
    List<CheckAvailabilityTrack> _processingAvailabilityRequests = new List<CheckAvailabilityTrack>();

    // Check parental controls requests
    class CheckParentalControlsRequest
    {
        public Sony.NP.NpUtils.GetParentalControlInfoRequest Request;
        public NpUtils.GetParentalControlInfoResponse Response;
    }
    List<CheckParentalControlsRequest> _processingParentalControlsRequests = new List<CheckParentalControlsRequest>();

    class CheckJoinPlusRequest
    {
        public Sony.NP.Core.EmptyResponse Response;
        public Action<bool> Callback;
    }
    List<CheckJoinPlusRequest> _joinPlusRequests = new List<CheckJoinPlusRequest>();

    List<Action<bool>> _checkAvailabilityCallbacks = new List<Action<bool>>();

    public bool UserPermissionsReady = false;
    public bool UserPermissionsOk = false;
    public bool VoiceChatOk = false;

    public bool IsChecking
    {
        get { return _checkingPlus || _waitingForPermissions; }
    }
    bool _checkingPlus = false;
    bool _waitingForPermissions = false;

    Sony.NP.NpUtils.CheckPlusResponse _pendingOnCheckPlusResponse;

    // Checks plus behaviour, if succeeds then will return cached success, otherwise will recheck each time
    bool _hasPlusEntitlement = false;
    bool _signalUserSignedOut = false; // User has signed out, leave multiplayer next frame

    static public PS4Plus Inst { get; private set; }

    public bool IsGamePlayActive
    {
        set
        {
            if (_gameplayActive != value)
            {
                Debug.LogFormat("IsGamePlayActive = {0}", value);
                _gameplayActive = value;                
            }
        }
    
        get
        {
            return _gameplayActive;
        }
    }
    bool _gameplayActive = false;

    public bool IsUserLoggedIn
    {
        set
        {
            if (_isUserLoggedIn != value)
            {
                _isUserLoggedIn = value;
                Debug.LogFormat("IsUserLoggedIn = {0}", _isUserLoggedIn);
            }

        }
        get
        {
            return _isUserLoggedIn;
        }
    }
    bool _isUserLoggedIn = false;

    public bool IsUserSignedIn
    {
        set
        {
            if (_isUserSignedIn != value)
            {
                _isUserSignedIn = value;
                Debug.LogFormat("IsUserSignedIn = {0}", _isUserSignedIn);
                if(!_isUserSignedIn)
                {
                    _signalUserSignedOut = true;
                }
            }

        }
        get
        {
            return _isUserSignedIn;
        }
    }
    bool _isUserSignedIn = false;

    public bool IsArcheryGameActive
    {
        set
        {
            if (_isArcheryGameActive != value)
            {
                Debug.LogFormat("IsArcheryGameActive = {0}", value);
                _isArcheryGameActive = value;
            }
        }

        get
        {
            return _isArcheryGameActive;
        }
    }
    bool _isArcheryGameActive = false;

    public bool IsMultiplePlayersActive
    {
        set
        {
            if (_isMultiplayerActive != value)
            {
                Debug.LogFormat("IsMultiplePlayersActive = {0}", value);
                _isMultiplayerActive = value;
            }
        }
        get
        {
            return _isMultiplayerActive;
        }
    }
    bool _isMultiplayerActive = false;

    public bool IsPlayingRealtime
    {
        get { return IsUserSignedIn && IsMultiplePlayersActive && (IsGamePlayActive || IsArcheryGameActive); }
    }

    float _notificationTimer = 0;
    public float NotificationInterval = 1;

    IEnumerator Start()
    {
        if (null != Inst)
            throw new System.Exception("Tried to create PS4Plus twice");
        Inst = this;

        if (!PS4NpInit.Initialised)
            yield return new WaitForEndOfFrame();
        //CheckPlus(null);
        CheckAvailabilityAll(null);
    }

    public void CheckPlus()
    {
        CheckPlus(null);
    }

    public void Update()
    {
        if(_signalUserSignedOut)
        {
            _signalUserSignedOut = false;
            GameState.LeaveMultiplayer();
        }

        if (IsPlayingRealtime)
        {
            float dt = Time.unscaledDeltaTime;
            _notificationTimer -= dt;
            if (_notificationTimer < 0)
            {
                Sony.NP.NpUtils.NotifyPlusFeature(PS4Common.InitialUserId);
                _notificationTimer = NotificationInterval;
            }
        }

        UpdatePermissionsChecks();

        if (null != _pendingOnCheckPlusResponse)
        {
            _checkingPlus = false;
            int idx = _checkPlusRequests.FindIndex((r) => { return r.Response == _pendingOnCheckPlusResponse; });
            var cb = _checkPlusRequests[idx].Callback;
            _checkPlusRequests.RemoveAt(idx);
            if (_pendingOnCheckPlusResponse.Authorized)
                _hasPlusEntitlement = true;
            _pendingOnCheckPlusResponse = null;
            if (null != cb)
                cb(_hasPlusEntitlement);
            OnScreenLog.Add("PlusEntitlement:" + (_hasPlusEntitlement ? "YES" : "NO"));
        }
    }

    void UpdatePermissionsChecks()
    {
        if (_waitingForPermissions)
        {
            if (_processingAvailabilityRequests.Count == 0 && _processingParentalControlsRequests.Count == 0)
            {   
                OnScreenLog.Add(string.Format("User Permissions"));
                foreach (var user in Users)
                {
                    OnScreenLog.Add(string.Format("UserId:{0} InitialPlayer:{1} HasPermission:{2} ChatRestriction:{3} ReturnCode:{4}", user.UserId, user.IsInitialUser, user.HasPermission, user.ChatRestriction, user.ReturnCode));
                }
                UserPermissionsReady = true;
                UserPermissionsOk = false;
                VoiceChatOk = false;
                foreach (var user in Users)
                {
                    if (user.IsInitialUser)
                    {
                        UserPermissionsOk = user.HasPermission;
                        VoiceChatOk = !user.ChatRestriction;
                        OnScreenLog.Add(string.Format("UserPermissions: UserPermissionsOk:{0} VoiceChatOk:{1}", UserPermissionsOk, VoiceChatOk));
                    }
                }

                PhotonVoiceSettings.Instance.AutoConnect = VoiceChatOk; // If voice chat disabled then block autoconnect
                Debug.LogFormat("PhotonVoice Autoconnect set to {0} using voice permissions", PhotonVoiceSettings.Instance.AutoConnect);
                _waitingForPermissions = false;
                foreach (var c in _checkAvailabilityCallbacks)
                {
                    c(UserPermissionsOk);
                }
                _checkAvailabilityCallbacks.Clear();
            }            
        }
    }

    public void CheckAvailabilityAll(Action<bool> cb)
    {
        if (UserPermissionsReady && UserPermissionsOk) // If check already passed, then return ok
        {
            if (null != cb)
                cb(true);
        }
        else
        {
            if (null != cb)
                _checkAvailabilityCallbacks.Add(cb);
            if (!_waitingForPermissions)
            {
                _waitingForPermissions = true;

                //Sony.NP.UserProfiles.LocalLoginUserId foundUserId = new Sony.NP.UserProfiles.LocalLoginUserId();
                Sony.NP.UserProfiles.LocalUsers users = new Sony.NP.UserProfiles.LocalUsers();
                try
                {
                    Sony.NP.UserProfiles.GetLocalUsers(users);
                }
                catch (Sony.NP.NpToolkitException)
                {
                    // This means that one or more of the user has an error code associated with them. This might mean they are not signed in or are not signed up to an online account.
                }

                Users = new List<PS4User>();
                for (int i = 0; i < users.LocalUsersIds.Length; i++)
                {
                    Sony.NP.UserProfiles.LocalLoginUserId localUserId = users.LocalUsersIds[i];

                    if (localUserId.UserId.Id != Sony.NP.Core.UserServiceUserId.UserIdInvalid &&
                        localUserId.SceErrorCode == (int)Sony.NP.Core.ReturnCodes.SUCCESS)
                    {
                        bool isInitialUser = localUserId.UserId.Id == PS4Common.InitialUserId;
                        var newUser = new PS4User() { UserId = localUserId.UserId.Id, IsInitialUser = isInitialUser };
                        Users.Add(newUser);

                        // Check parental availability
                        try
                        {
                            Sony.NP.NpUtils.CheckAvailablityRequest caRequest = new Sony.NP.NpUtils.CheckAvailablityRequest() { UserId = newUser.UserId, Async = true };
                            var response = new Sony.NP.Core.EmptyResponse();
                            _processingAvailabilityRequests.Add(new CheckAvailabilityTrack() { Request = caRequest, Response = response });
                            Sony.NP.NpUtils.CheckAvailablity(caRequest, response);
                        }
                        catch (Sony.NP.NpToolkitException)
                        {
                            OnScreenLog.AddError("An error occured while checking the user availability");
                        }

                        // Check parental controls                    
                        try
                        {
                            Sony.NP.NpUtils.GetParentalControlInfoRequest request = new Sony.NP.NpUtils.GetParentalControlInfoRequest() { UserId = newUser.UserId, Async = true };
                            Sony.NP.NpUtils.GetParentalControlInfoResponse response = new Sony.NP.NpUtils.GetParentalControlInfoResponse();
                            _processingParentalControlsRequests.Add(new CheckParentalControlsRequest() { Request = request, Response = response });
                            int requestId = Sony.NP.NpUtils.GetParentalControlInfo(request, response);
                            OnScreenLog.Add("GetParentalControlInfo Async : Request Id = " + (UInt32)requestId);
                        }
                        catch (Sony.NP.NpToolkitException e)
                        {
                            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
                        }
                    }
                }
            }
        }
    }

    public void OnCheckAvailability(Sony.NP.Core.EmptyResponse response)
    {
        int idx = _processingAvailabilityRequests.FindIndex((r) => { return r.Response == response; });
        if (idx != -1)
        {
            if (null != response)
            {
                if (response.Locked == false)
                {
                    var req = _processingAvailabilityRequests[idx];
                    var user = GetUserByID(req.Request.UserId);
                    user.HasPermission = response.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS;
                    user.ReturnCode = response.ReturnCode;
                    _processingAvailabilityRequests.RemoveAt(idx);
                }
                else
                {
                    Debug.LogError("Failed to get parental control info, locked");
                }
            }
            else
            {
                Debug.LogError("Failed to get parental control info, null response");
            }
        }
        else
        {
            Debug.LogError("PS4Plus - response not found");
        }
    }

    public void OnGetParentalControlInfo(Sony.NP.NpUtils.GetParentalControlInfoResponse response)
    {
        int idx = _processingParentalControlsRequests.FindIndex((r) => { return r.Response == response; });
        if (idx != -1)
        {
            if (null != response)
            {
                if (response.Locked == false)
                {
                    var req = _processingParentalControlsRequests[idx];
                    var user = GetUserByID(req.Request.UserId);
                    user.Age = response.Age;
                    user.ChatRestriction = response.ChatRestriction;
                    user.UGCRestriction = response.UGCRestriction;
                    _processingParentalControlsRequests.RemoveAt(idx);
                }
                else
                {
                    Debug.LogError("Failed to get parental control info, locked");
                }
            }
            else
            {
                Debug.LogError("Failed to get parental control info, null response");
            }
        }
        else
        {
            Debug.LogError("OnGetParentalControlInfo - response not found");
        }
    }

    public void CheckPlus(Action<bool> success)
    {
        if (_hasPlusEntitlement) // Has PSN Plus, don't check again
        {
            if(null!=success)
                success(true);
        }
        else
        {
            try
            {
                Sony.NP.NpUtils.CheckPlusRequest request = new Sony.NP.NpUtils.CheckPlusRequest();
                request.UserId = PS4Common.InitialUserId;
                request.Async = true;
                Sony.NP.NpUtils.CheckPlusResponse response = new Sony.NP.NpUtils.CheckPlusResponse();

                // Make the async call which will return the Request Id 
                int requestId = Sony.NP.NpUtils.CheckPlus(request, response);
                _checkPlusRequests.Add(new CheckPlusRequest() { Response = response, Callback = success });
                _checkingPlus = true;
            }
            catch (Sony.NP.NpToolkitException e)
            {
                OnScreenLog.AddError("Exception : " + e.ExtendedMessage);                
            }
        }
    }

    public void OnCheckPlus(Sony.NP.NpUtils.CheckPlusResponse response)
    {
        _pendingOnCheckPlusResponse = response;        
    }

    public void DisplayChatRestriction()
    {
        Sony.PS4.Dialog.Common.ShowSystemMessage(Sony.PS4.Dialog.Common.SystemMessageType.TRC_PSN_CHAT_RESTRICTION, PS4Common.InitialUserId);
    }

    public void DisplayJoinPlusDialog()
    {
        DisplayJoinPlusDialog(null);
    }

    public void DisplayJoinPlusDialog(Action<bool> success = null)
    {
        try
        {
            Sony.NP.Commerce.DisplayJoinPlusDialogRequest request = new Sony.NP.Commerce.DisplayJoinPlusDialogRequest();

            Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

            int requestId = Sony.NP.Commerce.DisplayJoinPlusDialog(request, response);
            OnScreenLog.Add("DisplayJoinPlusDialog Async : Request Id = " + requestId);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void OnJoinPlus(Sony.NP.Core.EmptyResponse response)
    {
        int idx = _joinPlusRequests.FindIndex((r)=> { return r.Response == response; });
        var cb = _joinPlusRequests[idx].Callback;
        _joinPlusRequests.RemoveAt(idx);
        if (null != cb)
            cb(response.ReturnCode == Sony.NP.Core.ReturnCodes.DIALOG_RESULT_USER_PURCHASED);
        OnScreenLog.Add("ReturnCode:" + response.ReturnCode.ToString());
    }
}