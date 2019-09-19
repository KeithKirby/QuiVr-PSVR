using System;
using System.Collections;
using Sony.NP;
using UnityEngine;

// Sets up Photon for PS4 use, when ready this will unblock PS4Photon from starting

public class PhotonPs4Auth : MonoBehaviour
{
    // the Client ID of your "App Server/Website". Set in Inspector.
    [Tooltip("Client ID of your \"App Server/Website\". See DevNet.")]
    public string ClientId;

    [Tooltip("If the internal logging is turned on.")]
    public bool InternalLogging = true;

    Action<bool> _authComplete;

    static public bool Ready = false;
    static public PhotonPs4Auth Inst;

    // the scope of authorization. usually "psn:s2s".
    string AuthScope = "psn:s2s";

    // The AuthCode, which is used for Custom Auth.
    protected internal string authCode = "";

    // The service environment
    // TODO: Read this at runtime from the AuthCode ticket you get.
    private Auth.IssuerIdType issuerId = Auth.IssuerIdType.Invalid;

    Sony.NP.Auth.AuthCodeResponse _pendingResponse = null;

    public void Awake()
    {
        if (null == Inst)
        {
            Sony.NP.Main.OnAsyncEvent += OnAsyncEvent;
            Inst = this;
        }
    }

    // disable the component, if you don't want to run this on start.
    public IEnumerator Start()
    {
        while (!PS4NpInit.Initialised)
            yield return new WaitForEndOfFrame();
        SetupPs4AuthAndConnectUsingSettings();
    }

    public void Update()
    {
        // important: Don't forget to Update Sony.NP.Main or you won't get events/callbacks
#if !UNITY_EDITOR
        Main.Update();
#endif

        if(null!=_pendingResponse)
        {
            var pr = _pendingResponse;
            _pendingResponse = null;
            DoAuthComplete(pr);            
        }
    }

    public void SetupPs4AuthAndConnectUsingSettings()
    {
        Debug.LogFormat("SetupPs4AuthAndConnectUsingSettings GetUserId: {0}", PS4Common.InitialUserId);
        RequestAuthCode(null);
    }

    public void RequestAuthCode(Action<bool> authComplete)
    {
#if UNITY_EDITOR
#else
        if (ClientId.Length > 0)
        {
            try
            {
                Sony.NP.Auth.GetAuthCodeRequest request = new Sony.NP.Auth.GetAuthCodeRequest();

                // test values from SDK nptoolkit sample ... replace with your own project values
                Sony.NP.Auth.NpClientId clientId = new Sony.NP.Auth.NpClientId();
                clientId.Id = ClientId;

                request.ClientId = clientId;
                request.Scope = "psn:s2s";

                Sony.NP.Auth.AuthCodeResponse response = new Sony.NP.Auth.AuthCodeResponse();

                // the actual call to get an AuthCode
                int requestId = Sony.NP.Auth.GetAuthCode(request, response);
                Debug.Log("Get Auth Code Async : Request Id = " + requestId);
                _authComplete = authComplete;
            }
            catch (Sony.NP.NpToolkitException e)
            {
                Debug.LogError("Exception : " + e.ExtendedMessage);
                authComplete(false);
            }
        }
        else
        {
            Debug.Log("Photon: ClientId not set");
            authComplete(false);
        }
#endif
    }

    /// <summary>
    /// Requires this.onlineId, this.authCode and this.issuerId to be set correctly.
    /// </summary>
    public void SetupPs4AuthForPun()
    {
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.PlayStation;   // NOTE: this is new. if your CustomAuthenticationType lacks this, add it with a value of 4
        PhotonNetwork.AuthValues.UserId = PS4Common.InitialUserId.ToString();

        string env = "dev"; // defaults to dev
        if (issuerId == Auth.IssuerIdType.Live)
            env = "prod";
        if (issuerId == Auth.IssuerIdType.Certification)
            env = "qa";
        if (issuerId == Auth.IssuerIdType.Development)
            env = "dev";

        PhotonNetwork.AuthValues.AddAuthParameter("token", this.authCode);
        PhotonNetwork.AuthValues.AddAuthParameter("env", env);
        PhotonNetwork.AuthValues.AddAuthParameter("userName", PhotonNetwork.AuthValues.UserId);

        Debug.Log("SetupPs4AuthForPun()  AuthCode: " + authCode.Substring(0, 6) + " env: " + env + " issuerId: " + issuerId + " onlineID: " + PS4Common.InitialUserId + " PhotonNetwork.AuthValues.UserId: " + PhotonNetwork.AuthValues.UserId);
    }
    
    // callbacks from npToolkit2:

    public void OnAsyncEvent(Sony.NP.NpCallbackEvent callbackEvent)
    {
        if (callbackEvent.Service == Sony.NP.ServiceTypes.Auth)
        {
            switch (callbackEvent.ApiCalled)
            {
                case Sony.NP.FunctionTypes.AuthGetAuthCode:
                    _pendingResponse = callbackEvent.Response as Sony.NP.Auth.AuthCodeResponse;
                    break;
                default:
                    break;
            }
        }
    }

    void DoAuthComplete(Sony.NP.Auth.AuthCodeResponse response)
    {
        bool ok = response.AuthCode.Length > 0;
        Debug.LogFormat("DoAuthComplete returnCode({0}) Authcode({1})", response.ReturnCode, response.AuthCode);
        if (ok)
        {
            Debug.Log("DoAuthComplete success");
            authCode = response.AuthCode;
            issuerId = response.IssuerId;
            SetupPs4AuthForPun();
        }
        else
        {
            Debug.Log("DoAuthComplete failed");
        }
        Ready = true;        
        if (null != _authComplete)
        {
            var cb = _authComplete;
            _authComplete = null;
            cb(ok);
        }
    }
}