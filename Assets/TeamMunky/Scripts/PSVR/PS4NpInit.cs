#define USE_ASYNC_HANDLING
//#define ENABLE_TUS_LOGGING

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;
using UnityEngine.PS4;

namespace Sony.NP
{
    [RequireComponent(typeof(PS4Plus))]
    [RequireComponent(typeof(PS4Matchmaking))]
    [RequireComponent(typeof(PS4Scoreboard))]
    public class PS4NpInit : MonoBehaviour
    {
#if UNITY_PS4
        public static bool Initialised { get { return _initialised; } }

        static bool _initialised = false;     // Is the NP plugin initialised and ready for use.
        static bool _instanceCreated = false;

        PS4TrophyManager m_trophyManager;

        public SonyNpFriends Friends { get { return m_Friends; } }
        //public NpParentalInfo ParentalSettings { get { return m_parentalInfo; } }

        SonyNpNotifications m_Notifications;
        SonyNpFriends m_Friends;
        //SonyNpUserProfiles m_UserProfiles;
        PS4Profiles _profiles;
        SonyNpNetworkUtils m_NetworkUtils;
        SonyNpTrophies m_Trophies;
        SonyNpPresence m_Presence;

        PS4Matchmaking _matchmaking;
        PS4Plus _plus;
        PS4Scoreboard _scoreboard;

        SonyNpTss m_Tss;
        SonyNpTus m_Tus;
        SonyNpMessaging m_Messaging;
        SonyNpCommerce m_Commerce;
        SonyNpAuth m_Auth;
        SonyNpWordFilter m_WordFilter;
        //NpParentalInfo m_parentalInfo;

        static bool dialogOpened = false;

        public Material iconMaterial;
        static Sony.NP.Icon currentIcon = null;
        static bool updateIcon = false;

        // This is called from any thread
        public static void SetIconTexture(Sony.NP.Icon icon)
        {
            currentIcon = icon;
            updateIcon = true;
        }

        // This must be called from the main thread otherwise the Texture2D can't be created.
        public void UpdateIcon()
        {
            if (updateIcon == true)
            {
                updateIcon = false;

                if (currentIcon != null)
                {
                    // This will create the texture if it is not already cached in the currentIcon.           
                    UnityEngine.Texture2D iconTexture = new UnityEngine.Texture2D(currentIcon.Width, currentIcon.Height);

                    iconTexture.LoadImage(currentIcon.RawBytes);

                    iconMaterial.mainTexture = iconTexture;

                    OnScreenLog.Add("Updating icon material : W = " + iconTexture.width + " H = " + iconTexture.height);
                }
            }
        }
                
        void Start()
        {
            if (_instanceCreated)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                _instanceCreated = true;
                
                // Initialize the NP Toolkit.
                OnScreenLog.Add("Initializing NP");

#if UNITY_PS4 && !UNITY_EDITOR
            OnScreenLog.Add(System.String.Format("Initial UserId:0x{0:X}  Primary UserId:0x{1:X}", UnityEngine.PS4.Utility.initialUserId, UnityEngine.PS4.Utility.primaryUserId));
#endif

                m_Notifications = new SonyNpNotifications();
                m_Friends = new SonyNpFriends();
                //m_UserProfiles = new SonyNpUserProfiles();
                m_NetworkUtils = new SonyNpNetworkUtils();
                m_Trophies = new SonyNpTrophies();
                m_trophyManager = GetComponent<PS4TrophyManager>();
                //m_NpUtils = new SonyNpUtils();
                m_Presence = new SonyNpPresence();
                //m_Ranking = new SonyNpRanking();
                //m_Matching = new SonyNpMatching();                
                m_Tss = new SonyNpTss();
                m_Tus = new SonyNpTus();
                m_Messaging = new SonyNpMessaging();
                _matchmaking = GetComponent<PS4Matchmaking>();
                _plus = GetComponent<PS4Plus>();
                _scoreboard = GetComponent<PS4Scoreboard>();
                _profiles = GetComponent<PS4Profiles>();

                m_Commerce = new SonyNpCommerce();
                m_Auth = new SonyNpAuth();
                m_WordFilter = new SonyNpWordFilter();
                //m_parentalInfo = new NpParentalInfo();

                InitialiseNpToolkit();

//                NetworkManager.Init();
            }
        }

        public Sony.NP.InitResult initResult;

        static string[] _usCountries =
        {
            "us" // United States
        };

        static string[] _euCountries =
        {
            "at", // Austria
            "be", // Belgium
            "bg", // Bulgaria
            "hr", // Croatia
            "cy", // Republic of Cyprus
            "cz", // Czech Republic
            "dk", // Denmark
            "fi", // Finland
            "fr", // France
            "de", // Germany
            "gr", // Greece
            "hu", // Hungary
            "ie", // Ireland
            "it", // Italy
            "lu", // Luxembourg
            "mt", // Malta
            "nl", // Netherlands
            "pl", // Poland
            "pt", // Portugal
            "ro", // Romania
            "sk", // Slovakia
            "si", // Slovenia
            "es", // Spain
            "se", // Sweden
            "gb" // UK
            // MISSING CODES
            // Estonia
            // Latvia
            // Lithuania
        };

        Sony.NP.AgeRestriction[] GetAgeRestrictions()
        {
            Sony.NP.AgeRestriction[] ageRestrictions = new Sony.NP.AgeRestriction[_usCountries.Length + _euCountries.Length];
            int pos = 0;
            foreach(var country in _usCountries)
                ageRestrictions[pos++] = new Sony.NP.AgeRestriction(10, new Sony.NP.Core.CountryCode(country)); // Age restriction is 10 in the US
            foreach (var country in _euCountries)
                ageRestrictions[pos++] = new Sony.NP.AgeRestriction(7, new Sony.NP.Core.CountryCode(country)); // Age restriction is 7 in EU
            return ageRestrictions;
        }

        void InitialiseNpToolkit()
        {
            Sony.NP.Main.OnAsyncEvent += Main_OnAsyncEvent;

            Sony.NP.InitToolkit init = new Sony.NP.InitToolkit();
                        
            init.contentRestrictions.DefaultAgeRestriction = 10; // Has to be the highest restriction
            init.contentRestrictions.AgeRestrictions = GetAgeRestrictions();

            // Only do this if age restriction isn't required for the product. See documentation for details.
            // init.contentRestrictions.ApplyContentRestriction = false;

            //init.threadSettings.affinity = Sony.NP.Affinity.AllCores; // Sony.NP.Affinity.Core2 | Sony.NP.Affinity.Core4;
            init.threadSettings.affinity = Sony.NP.Affinity.Core2 | Sony.NP.Affinity.Core4;

            // Mempools
            init.memoryPools.JsonPoolSize = 6 * 1024 * 1024;
            init.memoryPools.SslPoolSize *= 4;

            init.memoryPools.MatchingSslPoolSize *= 4;
            init.memoryPools.MatchingPoolSize *= 4;

            init.SetPushNotificationsFlags(Sony.NP.PushNotificationsFlags.NewGameDataMessage | Sony.NP.PushNotificationsFlags.NewInGameMessage |
                                            Sony.NP.PushNotificationsFlags.NewInvitation | Sony.NP.PushNotificationsFlags.UpdateBlockedUsersList |
                                            Sony.NP.PushNotificationsFlags.UpdateFriendPresence | Sony.NP.PushNotificationsFlags.UpdateFriendsList);

            try
            {
                initResult = Sony.NP.Main.Initialize(init);
                if (initResult.Initialized == true)
                {
                    OnScreenLog.Add("NpToolkit Initialized ");
                    OnScreenLog.Add("Plugin SDK Version : " + initResult.SceSDKVersion.ToString());
                    OnScreenLog.Add("Plugin DLL Version : " + initResult.DllVersion.ToString());
                    _initialised = true;
                    PS4Common.InitialUserId = UnityEngine.PS4.Utility.initialUserId;
                    for (int i = 0; i < 4; ++i)
                    {
                        var user = PS4Input.GetUsersDetails(i);
                        if (user.userId == PS4Common.InitialUserId)
                        {
                            PS4Common.InitialUserDetals = user;
                        }
                    }
                    //GetParentalControlInfo();
                }
                else
                {
                    OnScreenLog.Add("NpToolkit not initialized ");
                }
            }
            catch (Sony.NP.NpToolkitException e)
            {
                OnScreenLog.AddError("Exception During Initialization : " + e.ExtendedMessage);
            }
#if UNITY_EDITOR
            catch (DllNotFoundException e)
            {
                OnScreenLog.AddError("Missing DLL Expection : " + e.Message);
                OnScreenLog.AddError("The sample APP will not run in the editor.");
            }
#endif

            OnScreenLog.AddNewLine();

            GamePad[] gamePads = GetComponents<GamePad>();

            User.Initialise(gamePads);

            LogStartUp();
        }

        // NOTE : This is called on the "Sony NP" thread and is independent of the Behaviour update.
        // This thread is created by the SonyNP.dll when NpToolkit2 is initialised.
        private void Main_OnAsyncEvent(Sony.NP.NpCallbackEvent callbackEvent)
        {
            OnScreenLog.Add("Event: Service = (" + callbackEvent.Service + ") : API Called = (" + callbackEvent.ApiCalled + ") : Request Id = (" + callbackEvent.NpRequestId + ") : Calling User Id = (" + callbackEvent.UserId + ")");

            HandleAsynEvent(callbackEvent);
        }

        void Update()
        {
#if !UNITY_EDITOR
            Sony.PS4.Dialog.Main.Update();
            Sony.NP.Main.Update();
#endif
            UpdateIcon();
            //NetworkManager.Update();
        }

        void HandleNetStateChange(Sony.NP.NetworkUtils.NetStateChangeResponse r)
        {
            if (r == null)
                return;

            if (r.Locked == false)
            {
                if(NetworkUtils.NetworkEvent.networkDisconnected == r.NetEvent) // eg Network cable pulled
                {
                    // Enable to show disconnect message when cable is pulled
                    //Note n = new Note();
                    //n.title = "Disconnected";
                    //n.body = "Lost connection.";
                    //n.icon = AppBase.v.ErrorIcon;
                    //Notification.NotifyDelayed(n, 1.5f);
                }
            }
        }

        void HandleUserStateChange(Sony.NP.NpUtils.UserStateChangeResponse r)
        {
            Debug.Log("HandleUserStateChange");
            if (r == null)
            {
                Debug.Log("r is null");
                return;
            }

            if (r.Locked == false)
            {
                switch (r.CurrentLogInState)
                {
                case Sony.NP.NpUtils.LogInState.loggedIn:
                    PS4Plus.Inst.IsUserLoggedIn = true;
                    break;
                case Sony.NP.NpUtils.LogInState.loggedOut:
                    PS4Plus.Inst.IsUserLoggedIn = false;
                    break;
                case Sony.NP.NpUtils.LogInState.unknown:
                    Debug.Log("Unknown login state");
                    break;
                }
                switch (r.CurrentSignInState)
                {
                    case Sony.NP.NpUtils.SignInState.signedIn:
                        PS4Plus.Inst.IsUserSignedIn = true;
                        break;
                    case Sony.NP.NpUtils.SignInState.signedOut:
                        PS4Plus.Inst.IsUserSignedIn = false;
                        break;
                    case Sony.NP.NpUtils.SignInState.unknown:
                        Debug.Log("Unknown signin state");
                        break;
                }

            }
            else
            {
                Debug.Log("r is locked");
            }
        }

        private void HandleAsynEvent(Sony.NP.NpCallbackEvent callbackEvent)
        {
            try
            {
                if (callbackEvent.Response != null)
                {
                    if (callbackEvent.Response.ReturnCodeValue < 0)
                    {
                        OnScreenLog.AddError("Response : " + callbackEvent.Response.ConvertReturnCodeToString(callbackEvent.ApiCalled));
                    }
                    else
                    {
                        OnScreenLog.Add("Response : " + callbackEvent.Response.ConvertReturnCodeToString(callbackEvent.ApiCalled));
                    }

                    if (callbackEvent.Response.HasServerError)
                    {
                        OutputSeverError(callbackEvent.Response);
                    }
                }

                switch (callbackEvent.Service)
                {
                    case Sony.NP.ServiceTypes.Notification:
                        {
                            switch (callbackEvent.ApiCalled)
                            {
                                case Sony.NP.FunctionTypes.NotificationDialogOpened:
                                    dialogOpened = true;
                                    GamePad.EnableInput(!dialogOpened);
                                    break;
                                case Sony.NP.FunctionTypes.NotificationDialogClosed:
                                    dialogOpened = false;
                                    GamePad.EnableInput(!dialogOpened);
                                    break;
                                case Sony.NP.FunctionTypes.NotificationNetStateChange:
                                    HandleNetStateChange(callbackEvent.Response as Sony.NP.NetworkUtils.NetStateChangeResponse);
                                    break;
                                case Sony.NP.FunctionTypes.NotificationUserStateChange:
                                    HandleUserStateChange(callbackEvent.Response as Sony.NP.NpUtils.UserStateChangeResponse);
                                    break;
                            }

                            m_Notifications.OnAsyncEvent(callbackEvent);

                            // Also call matching as this needs to process some notifications.
                            //m_Matching.OnAsyncEvent(callbackEvent);
                            _matchmaking.OnAsyncEvent(callbackEvent);
                        
                            if(callbackEvent.ApiCalled==Sony.NP.FunctionTypes.CommerceDisplayJoinPlusDialog)
                                _plus.OnJoinPlus(callbackEvent.Response as Sony.NP.Core.EmptyResponse);

                            // Also call messaging as this needs to process some notifications.
                            m_Messaging.OnAsyncEvent(callbackEvent);
                        }
                        break;
                    case Sony.NP.ServiceTypes.Friends:
                        m_Friends.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.UserProfile:
                        //m_UserProfiles.OnAsyncEvent(callbackEvent);
                        _profiles.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.NetworkUtils:
                        m_NetworkUtils.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.Trophy:
                        m_Trophies.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.NpUtils:
                        //m_NpUtils.OnAsyncEvent(callbackEvent);
                        switch (callbackEvent.ApiCalled)
                        {
                            case Sony.NP.FunctionTypes.NpUtilsSetTitleIdForDevelopment:
                                //OutputSetTitleIdForDevelopment(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                                break;
                            case Sony.NP.FunctionTypes.NpUtilsDisplaySigninDialog:
                                //OutputDisplaySigninDialog(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                                break;
                            case Sony.NP.FunctionTypes.NpUtilsCheckAvailability:
                                //OutputCheckAvailability(callbackEvent.Response as Sony.NP.Core.EmptyResponse);                                
                                _plus.OnCheckAvailability(callbackEvent.Response as Sony.NP.Core.EmptyResponse);                                
                                break;
                            case Sony.NP.FunctionTypes.NpUtilsCheckPlus:
                                //OutputCheckPlus(callbackEvent.Response as Sony.NP.NpUtils.CheckPlusResponse);
                                _plus.OnCheckPlus(callbackEvent.Response as Sony.NP.NpUtils.CheckPlusResponse);
                                break;
                            case Sony.NP.FunctionTypes.NpUtilsGetParentalControlInfo:
                                //OutputGetParentalControlInfo(callbackEvent.Response as Sony.NP.NpUtils.GetParentalControlInfoResponse);
                                _plus.OnGetParentalControlInfo(callbackEvent.Response as Sony.NP.NpUtils.GetParentalControlInfoResponse);
                                //OnGetParentalControlInfo(callbackEvent.Response as Sony.NP.NpUtils.GetParentalControlInfoResponse);
                                break;
                            default:
                                break;
                        }
                        break;
                    case Sony.NP.ServiceTypes.Presence:
                        m_Presence.OnAsyncEvent(callbackEvent);
                        break;                    
                    case Sony.NP.ServiceTypes.Ranking:
                        //m_Ranking.OnAsyncEvent(callbackEvent);
                        _scoreboard.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.Matching:
                        //m_Matching.OnAsyncEvent(callbackEvent);
                        _matchmaking.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.Tss:
                        m_Tss.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.Tus:
                        m_Tus.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.Messaging:
                        m_Messaging.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.Commerce:
                        m_Commerce.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.Auth:
                        m_Auth.OnAsyncEvent(callbackEvent);
                        break;
                    case Sony.NP.ServiceTypes.WordFilter:
                        m_WordFilter.OnAsyncEvent(callbackEvent);
                        break;
                    default:
                        break;
                }

                OnScreenLog.AddNewLine();

                //NetworkManager.OnAsyncEvent(callbackEvent);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                OnScreenLog.AddError("Main_OnAsyncEvent NpToolkit Exception = " + e.ExtendedMessage);
                Console.Error.WriteLine(e.ExtendedMessage); // Output to the PS4 Stderr TTY
                Console.Error.WriteLine(e.StackTrace); // Output to the PS4 Stderr TTY
            }
            catch (Exception e)
            {
                OnScreenLog.AddError("Main_OnAsyncEvent General Exception = " + e.Message);
                OnScreenLog.AddError(e.StackTrace);
                Console.Error.WriteLine(e.StackTrace); // Output to the PS4 Stderr TTY
            }
        }

        private void OutputSeverError(Sony.NP.ResponseBase response)
        {
            if (response == null) return;

            if (response.HasServerError)
            {
                string errorMsg = String.Format("Server Error : returnCode = (0x{0:X}) : webApiNextAvailableTime = ({1}) : httpStatusCode = ({2})", response.ReturnCode, response.ServerError.WebApiNextAvailableTime, response.ServerError.HttpStatusCode);
                OnScreenLog.AddError(errorMsg);

                OnScreenLog.AddError("Server Error : jsonData = " + response.ServerError.JsonData);
            }
        }

        /*public void GetParentalControlInfo()
        {
            try
            {
                Sony.NP.NpUtils.GetParentalControlInfoRequest request = new Sony.NP.NpUtils.GetParentalControlInfoRequest();
                request.UserId = PS4Common.InitialUserId;
                request.Async = true;
                Sony.NP.NpUtils.GetParentalControlInfoResponse response = new Sony.NP.NpUtils.GetParentalControlInfoResponse();
                int requestId = Sony.NP.NpUtils.GetParentalControlInfo(request, response);
                OnScreenLog.Add("GetParentalControlInfo Async : Request Id = " + (UInt32)requestId);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
            }
        }*/

        void LogStartUp()
        {
#if ENABLE_TUS_LOGGING
        Thread thread = new Thread(new ThreadStart(ExecuteLogStartUp));
        thread.Start();
#endif
        }

        bool enableTUSLogging = true;
        /*
        void OnGetParentalControlInfo(Sony.NP.NpUtils.GetParentalControlInfoResponse response)
        {
            if(null==response)
            {
                Debug.LogError("Failed to get parental control info, null response");
                return;
            }
            if (response.Locked == false)
            {
                m_parentalInfo.Age = response.Age;
                m_parentalInfo.ChatRestriction = response.ChatRestriction;
                m_parentalInfo.UGCRestriction = response.UGCRestriction;
                OnScreenLog.Add("Age : " + response.Age);
                OnScreenLog.Add("ChatRestriction : " + response.ChatRestriction);
                OnScreenLog.Add("UGCRestriction : " + response.UGCRestriction);
            }
            else
            {
                Debug.LogError("Failed to get parental control info, locked");
            }
        }
        */

        class LoggingCounts
        {
            public int[] serviceUsage = new int[(int)Sony.NP.ServiceTypes.Size];
            public int totalCount = 0;

            public void Reset()
            {
                totalCount = 0;
                for (int i = 0; i < serviceUsage.Length; i++)
                {
                    serviceUsage[i] = 0;
                }
            }
        }

        LoggingCounts currentCounts = new LoggingCounts();
        LoggingCounts uploadCounts = new LoggingCounts();

        static int currentThreshold = 1;
        
        public void ExecuteLogService()
        {
            // Taken from "Using the Title User Storage" documentation
            // Reference Count and Voting
            // By preparing a TUS variable that others can write to, and using sceNpTusAddAndGetVariableA(), you can see how many times you have been referenced.
            // The TUS variable of a virtual user can be counted up using sceNpTusAddAndGetVariableA() to realize a vote-casting mechanism.
            // If you use this mechanism, however, adopt a scheme on the application side where each user's number of references or votes can be limited. 
            // For example, make a recording when the save data or title user storage is used to make a reference or to cast a vote, 
            // and control the count or frequency of this operation. A specification that lacks this control will lead to excessive loads on the server 
            // and/or result in an unrealistic score. Make sure the necessary precautions are taken to avoid such situations.
            try
            {
                for (int i = 1; i < uploadCounts.serviceUsage.Length; i++)
                {
                    if (uploadCounts.serviceUsage[i] > 0)
                    {
                        Sony.NP.Tus.AddToAndGetVariableRequest request = new Sony.NP.Tus.AddToAndGetVariableRequest();

                        Sony.NP.Tus.VirtualUserID id = new Sony.NP.Tus.VirtualUserID();
                        id.Name = "_ERGVirtualUser1";
                        request.TusUser = new Sony.NP.Tus.UserInput(id);

                        Sony.NP.Tus.VariableInput var = new Sony.NP.Tus.VariableInput();
                        var.Value = uploadCounts.serviceUsage[i];
                        var.SlotId = i + 6;

                        request.Var = var;

                        //Sony.NP.Tus.AtomicAddToAndGetVariableResponse response = new Sony.NP.Tus.AtomicAddToAndGetVariableResponse();
                        Sony.NP.Tus.VariablesResponse response = new Sony.NP.Tus.VariablesResponse();

                        request.UserId = User.GetActiveUserId;

                        request.Async = false;

                        Sony.NP.Tus.AddToAndGetVariable(request, response);
                    }
                }
            }
            catch (Sony.NP.NpToolkitException e)
            {
                OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
            }
        }
#endif
    }
}