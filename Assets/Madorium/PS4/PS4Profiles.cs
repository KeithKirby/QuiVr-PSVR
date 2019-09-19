﻿using Sony.NP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.PS4;

[RequireComponent(typeof(PS4Common))]
public class PS4Profiles : MonoBehaviour
{
#if UNITY_PS4    
    static PS4Profiles _inst;

    public static PS4Profiles Inst { get { return _inst; } }

    RequestCallbacks<Sony.NP.UserProfiles.NpProfilesResponse> _getProfilesRequest = new RequestCallbacks<Sony.NP.UserProfiles.NpProfilesResponse>();

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

    Sony.NP.UserProfiles.NpProfilesResponse currentKnownProfiles = null;
    
    /// <summary>
    /// Helper function to get local user's account id
    /// </summary>
    public static Sony.NP.Core.NpAccountId GetLocalAccountId(int localUserId)
    {
        Sony.NP.UserProfiles.LocalUsers users = new Sony.NP.UserProfiles.LocalUsers();
        try
        {
            Sony.NP.UserProfiles.GetLocalUsers(users);
        }
        catch (Sony.NP.NpToolkitException)
        {
            // This means that one or more of the user has an error code associated with them. This might mean they are not signed in or are not signed up to an online account.
        }

        try
        {
            for (int i = 0; i < users.LocalUsersIds.Length; i++)
            {
                if (users.LocalUsersIds[i].UserId.Id == localUserId)
                {
                    return users.LocalUsersIds[i].AccountId;
                }
            }
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }

        return 0;
    }

    /// <summary>
    /// Helper function to get test if local user is signed in
    /// </summary>
    public static bool IsLocalUserSignedId(int localUserId)
    {
        Sony.NP.UserProfiles.LocalUsers users = new Sony.NP.UserProfiles.LocalUsers();
        try
        {
            Sony.NP.UserProfiles.GetLocalUsers(users);
        }
        catch (Sony.NP.NpToolkitException)
        {
            // This means that one or more of the user has an error code associated with them. This might mean they are not signed in or are not signed up to an online account.
        }

        try
        {
            for (int i = 0; i < users.LocalUsersIds.Length; i++)
            {
                if (users.LocalUsersIds[i].UserId.Id == localUserId)
                {
                    if (users.LocalUsersIds[i].SceErrorCode == 0)
                    {
                        return true;
                    }

                }
            }
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }

        return false;
    }

    public void GetLocalProfiles()
    {
        Sony.NP.UserProfiles.LocalUsers users = new Sony.NP.UserProfiles.LocalUsers();

        try
        {
            OnScreenLog.Add("Get Local Profiles");

            Sony.NP.UserProfiles.GetLocalUsers(users);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            if (e.SceErrorCode != (int)Sony.NP.Core.ReturnCodes.SUCCESS)
            {
                OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
                return;
            }
        }

        // Even if an exception has occured check the results for each user. 
        for (int i = 0; i < users.LocalUsersIds.Length; i++)
        {
            if (users.LocalUsersIds[i].UserId.Id != Sony.NP.Core.UserServiceUserId.UserIdInvalid)
            {
                if (users.LocalUsersIds[i].SceErrorCode == (int)Sony.NP.Core.ReturnCodes.SUCCESS)
                {
                    OnScreenLog.Add("User id = " + users.LocalUsersIds[i].UserId.ToString() + " : Account id = " + users.LocalUsersIds[i].AccountId.ToString());
                }
                else
                {
                    string output = Sony.NP.Core.ConvertSceErrorToString(users.LocalUsersIds[i].SceErrorCode);

                    // Some error code has been returned for the user. This may just mean they are not singed in or have not signed up for an online account.
                    OnScreenLog.Add("User id = " + users.LocalUsersIds[i].UserId.ToString() + " : Account id = " + users.LocalUsersIds[i].AccountId.ToString() + " : Error = " + output);
                }
            }
        }

        OnScreenLog.AddNewLine();
    }

    public void RequestNpProfiles()
    {
        // Get the local profile account id's first. Same as  GetLocalProfiles() above.
        Sony.NP.UserProfiles.LocalUsers users = new Sony.NP.UserProfiles.LocalUsers();
        try
        {
            Sony.NP.UserProfiles.GetLocalUsers(users);
        }
        catch (Sony.NP.NpToolkitException)
        {
            // This means that one or more of the user has an error code associated with them. This might mean they are not signed in or are not signed up to an online account.
        }

        try
        {
            // Use the account ids to fill in the request
            Sony.NP.UserProfiles.GetNpProfilesRquest request = new Sony.NP.UserProfiles.GetNpProfilesRquest();

            request.UserId = PS4Common.InitialUserId;

            List<Sony.NP.Core.NpAccountId> accountIds = new List<Sony.NP.Core.NpAccountId>();

            for (int i = 0; i < users.LocalUsersIds.Length; i++)
            {
                if (users.LocalUsersIds[i].UserId.Id != Sony.NP.Core.UserServiceUserId.UserIdInvalid &&
                    users.LocalUsersIds[i].AccountId.Id != 0)
                {
                    accountIds.Add(users.LocalUsersIds[i].AccountId.Id);
                }
            }

            request.AccountIds = accountIds.ToArray();

            Sony.NP.UserProfiles.NpProfilesResponse response = new Sony.NP.UserProfiles.NpProfilesResponse();

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.UserProfiles.GetNpProfiles(request, response);
            OnScreenLog.Add("GetNpProfiles Async : Request Id = " + requestId);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void RequestNpProfiles(List<Sony.NP.Core.NpAccountId> accounts, Action<Sony.NP.UserProfiles.NpProfilesResponse> callback)
    {
        try
        {
            // Use the account ids to fill in the request
            Sony.NP.UserProfiles.GetNpProfilesRquest request = new Sony.NP.UserProfiles.GetNpProfilesRquest();
            request.UserId = PS4Common.InitialUserId;
            request.AccountIds = accounts.ToArray();
            Sony.NP.UserProfiles.NpProfilesResponse response = new Sony.NP.UserProfiles.NpProfilesResponse();

            // Make the async call which will return the Request Id 
            _getProfilesRequest.Add(callback, response);
            int requestId = Sony.NP.UserProfiles.GetNpProfiles(request, response);
            OnScreenLog.Add("RequestNpProfiles Async : Request Id = " + requestId);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void RequestVerifiedAccountsForTitle()
    {
        try
        {
            // Use the account ids to fill in the request
            Sony.NP.UserProfiles.GetVerifiedAccountsForTitleRequest request = new Sony.NP.UserProfiles.GetVerifiedAccountsForTitleRequest();
            request.UserId = PS4Common.InitialUserId;

            Sony.NP.UserProfiles.NpProfilesResponse response = new Sony.NP.UserProfiles.NpProfilesResponse();

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.UserProfiles.GetVerifiedAccountsForTitle(request, response);
            OnScreenLog.Add("RequestVerifiedAccountsForTitle Async : Request Id = " + requestId);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    // Get a profile that is 'RelationTypes.me'
    private Sony.NP.Profiles.Profile FindProfileME()
    {
        if (currentKnownProfiles == null)
        {
            return null;
        }

        if (currentKnownProfiles.Profiles == null || currentKnownProfiles.Profiles.Length < 1)
        {
            return null; // Must have at least one user
        }

        for (int i = 0; i < currentKnownProfiles.Profiles.Length; i++)
        {
            if (currentKnownProfiles.Profiles[i].RelationType == Sony.NP.Profiles.Profile.RelationTypes.me)
            {
                return currentKnownProfiles.Profiles[i];
            }
        }

        return null;
    }

    // Find the first profile that isn't 'RelationTypes.me'
    private Sony.NP.Profiles.Profile FindFirstOtherProfile()
    {
        if (currentKnownProfiles == null)
        {
            return null;
        }

        if (currentKnownProfiles.Profiles == null || currentKnownProfiles.Profiles.Length < 2)
        {
            return null; // Must have at least two users
        }

        for (int i = 0; i < currentKnownProfiles.Profiles.Length; i++)
        {
            if (currentKnownProfiles.Profiles[i].RelationType != Sony.NP.Profiles.Profile.RelationTypes.me)
            {
                return currentKnownProfiles.Profiles[i];
            }
        }

        return null;
    }

    public void DisplayUserProfileDialog()
    {
        try
        {
            Sony.NP.Profiles.Profile meProfile = FindProfileME();

            if (meProfile == null) return;

            Sony.NP.UserProfiles.DisplayUserProfileDialogRequest request = new Sony.NP.UserProfiles.DisplayUserProfileDialogRequest();
            request.TargetAccountId = meProfile.OnlineUser.AccountId;
            request.UserId = PS4Common.InitialUserId;

            Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.UserProfiles.DisplayUserProfileDialog(request, response);
            OnScreenLog.Add("DisplayUserProfileDialog Async : Request Id = " + requestId);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void DisplayGriefReportingDialog()
    {
        try
        {
            Sony.NP.Profiles.Profile otherProfile = FindFirstOtherProfile();

            if (otherProfile == null) return;

            Sony.NP.UserProfiles.DisplayGriefReportingDialogRequest request = new Sony.NP.UserProfiles.DisplayGriefReportingDialogRequest();
            request.targetAccountId = otherProfile.OnlineUser.AccountId;

            // Specify the reason for the report. It is mandatory to specify at least one reason for the report.
            // Also the dialog will produce an error is the reported data doesn't exist. For example if 'reportAboutMe' is true
            // and the user has a blank 'About Me' string the dialog will error.
            request.reportOnlineId = true;             ///< true when the Online Id should be reported
            request.reportName = false;                 ///< true when the Name should be reported
            request.reportPicture = false;               ///< true when the Picture of the profile should be reported
            request.reportAboutMe = false;              ///< true when the About Me section of the profile should be reported

            request.UserId = PS4Common.InitialUserId;

            Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

            OnScreenLog.Add("User " + request.UserId + " is reporting on : " + otherProfile.OnlineUser.OnlineID.Name + " : " + otherProfile.OnlineUser.AccountId.ToString());

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.UserProfiles.DisplayGriefReportingDialog(request, response);
            OnScreenLog.Add("DisplayGriefReportingDialog Async : Request Id = " + requestId);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void OnAsyncEvent(Sony.NP.NpCallbackEvent callbackEvent)
    {
        if (callbackEvent.Service == Sony.NP.ServiceTypes.UserProfile)
        {
            switch (callbackEvent.ApiCalled)
            {
                case Sony.NP.FunctionTypes.UserProfileGetNpProfiles:
                    var cr = callbackEvent.Response as Sony.NP.UserProfiles.NpProfilesResponse;
                    currentKnownProfiles = callbackEvent.Response as Sony.NP.UserProfiles.NpProfilesResponse;
                    _getProfilesRequest.Complete(cr);
                    OutputNpProfiles(callbackEvent.Response as Sony.NP.UserProfiles.NpProfilesResponse);
                    break;
                case Sony.NP.FunctionTypes.UserProfileGetVerifiedAccountsForTitle:
                    OutputVerifiedAccountsForTitle(callbackEvent.Response as Sony.NP.UserProfiles.NpProfilesResponse);
                    break;
                default:
                    break;
            }
        }
    }

    private void OutputNpProfiles(Sony.NP.UserProfiles.NpProfilesResponse response)
    {
        if (response == null) return;

        OnScreenLog.Add("NpProfiles Response");

        if (response.Locked == false)
        {
            if (response.Profiles != null)
            {
                if (response.Profiles.Length > 0)
                {
                    for (int i = 0; i < response.Profiles.Length; i++)
                    {
                        Sony.NP.Profiles.Profile profile = response.Profiles[i];

                        if (profile != null)
                        {
                            string output = profile.ToString();
                            OnScreenLog.Add(output);

                            foreach (var language in profile.LanguagesUsed)
                            {
                                if (language.Code != null && language.Code.Length > 0)
                                {
                                    OnScreenLog.Add("Language Used : " + language.Code);
                                }
                            }
                        }
                    }
                }
                else
                {
                    OnScreenLog.Add("No Profiles returned");
                }
            }
        }
    }

    private void OutputVerifiedAccountsForTitle(Sony.NP.UserProfiles.NpProfilesResponse response)
    {
        if (response == null) return;

        OnScreenLog.Add("VerifiedAccountsForTitle Response");

        if (response.Locked == false)
        {
            if (response.Profiles != null)
            {
                if (response.Profiles.Length > 0)
                {
                    for (int i = 0; i < response.Profiles.Length; i++)
                    {
                        Sony.NP.Profiles.Profile profile = response.Profiles[i];

                        if (profile != null)
                        {
                            string output = profile.ToString();
                            OnScreenLog.Add(output);
                        }
                    }
                }
                else
                {
                    OnScreenLog.Add("No Profiles returned");
                }
            }
        }
    }
#endif
}