using UnityEngine;
using UnityEngine.VR;
using UnityEngine.SceneManagement;
using System.Collections;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class PSVRSetupManager : MonoBehaviour
{
    public CalibrateHeightAndPositionPage CalibrationPage;
    public GameObject SetupUI;
    public GameObject SetupEnvironment;

    Animator stateMachine;
    public bool ShowInstructionsWhenLoadingDirect = false;

    public float EnvironmentHeightOffset = 1.6f;

    public VisibleCheck VisCheck;

    bool _inStartupSequence = true;
    bool _reconnect = false;
    bool _fakeHMDConnect = false; // Flag which is used to fake an hmd connect in editor or no vr mode

    IEnumerator Start()
    {
        Debug.Log("PSVRSetupManager Start");
        QualitySettings.vSyncCount = 1;
        if (null == CameraRigs.ActiveRig)
            yield return null;

        stateMachine = GetComponent<Animator>();
        CalibrationPage.Complete += CalibrationPage_Complete;

        var scene = SceneManager.GetActiveScene();
        // Loading direct into level, skip instructions
        if (false == ShowInstructionsWhenLoadingDirect && scene.name != "PSVRSetup")
        {
            QuickStart(); // Started level directly
        }
        else
        {
            EnableWorldRendering = false;
            SetupStart(); // Started in setup scene            
        }
    }

    void OnEnable()
    {
#if UNITY_PS4
        // Register the callback needed to detect resetting the HMD
        Utility.onSystemServiceEvent += OnSystemServiceEvent;
#endif
        PSVRManager.instance.HMDConnected += Instance_HMDConnected;
        PSVRManager.instance.HMDDisconnected += HmdDisconnected;
    }

    void OnDisable()
    {
#if UNITY_PS4
        // Register the callback needed to detect resetting the HMD
        Utility.onSystemServiceEvent -= OnSystemServiceEvent;
#endif
        PSVRManager.instance.HMDConnected -= Instance_HMDConnected;
        PSVRManager.instance.HMDDisconnected -= HmdDisconnected;
    }
    
    void QuickStart()
    {
        EndSetupSequence();
    }

    void SetupStart()
    {
        var fc = RenderMode.GetInst();
        fc.UIFade = true;
        fc.InstantTransition();
        fc.UIFade = false;

        // Setup hmd immediately
        PSVRManager.instance.SetupHmdDevice();
    }

    public bool EnableWorldRendering
    {
        set
        {
            SetupPublisher.IsSetupScreenActive = !value;

            if (_worldRenderingEnabled!=value)
            {
                _worldRenderingEnabled = value;
                if(_worldRenderingEnabled)
                {
                    SetupEnvironment.SetActive(false);
                }
                else
                {
                    SetupEnvironment.SetActive(true);
                }
            }
        }
    }
    bool _worldRenderingEnabled = true;

    void UpdateReconnect()
    {
        if (_reconnect)
        {
            AnimatorStateInfo stateInfo = stateMachine.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Reconnect"))
            {
                if (PS4InputEx.ZapDown(0) || PS4InputEx.ZapDown(1))
                {
#if UNITY_EDITOR
                    Instance_HMDConnected();
#else
                    PSVRManager.instance.SetupHmdDevice();
#endif
                }
            }
        }
    }

    void Update()
    {
        if (null == stateMachine) // Component still starting
            return;

        bool grab = false;

#if ENABLE_PAD_CONTROL
        var grabRight = PS4InputEx.GrabRight(); // DS4 controller
        grab = grabRight > 0;
#endif

        var move0 = PS4InputEx.GetMove(0);
        var move1 = PS4InputEx.GetMove(1);

        UpdateReconnect();

        if (_inStartupSequence)
        {
            AnimatorStateInfo stateInfo = stateMachine.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Warning"))
            {

#if UNITY_EDITOR
                bool allowSkipNoHMD = true; // Editor mode, no hmd, fake it
#else
                bool allowSkipNoHMD = grab; // Game mode, if pad was used, then fake it
#endif
                if (VisCheck.IsLooking && (move0.Zap.Up || move1.Zap.Up || grab))
                {
                    if(allowSkipNoHMD && !_fakeHMDConnect)
                    {
                        _fakeHMDConnect = true;
                        Instance_HMDConnected(); // Fake the hmd being connected
                    }
                    else
                    {
                        if (UnityEngine.XR.XRSettings.enabled == true || allowSkipNoHMD)
                        {
                            var fade = RenderMode.GetInst();
                            if (fade.IsFadedIn)
                            {
                                stateMachine.SetTrigger("Calibrate");
                            }
                            else
                            {
                                Debug.LogFormat("Clicked to start but still fading in");
                            }
                        }
                        else
                        {
                            PSVRManager.instance.SetupHmdDevice();
                        }
                    }                    
                }
            }
            else if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.StartGame"))
            {
                if (_inStartupSequence)
                {
                    StartCoroutine(FadeOutThenLoad());
                    _inStartupSequence = false;
                }
            }
        }
    }

    private void CalibrationPage_Complete()
    {
        stateMachine.SetTrigger("StartGame");
    }
    
    void EndSetupSequence()
    {
        Debug.Log("EndSetupSequence");
        var am = GameObject.FindObjectOfType<AppModel>();
        if(am!=null)
            am.CompletedSetup = true;

        _inStartupSequence = false;

        // Setup done, wait for level to start so we can show the world and hide the setup ui
        GameEventPublisher.LevelInitialised += GameEventPublisher_LevelInitialised;
        GameEventPublisher.PublishSetupComplete();

        SetupEnvironment.SetActive(false);
        EnableWorldRendering = true;
        stateMachine.SetTrigger("Hidden");
    }

    IEnumerator FadeOutThenLoad()
    {
        var fc = RenderMode.GetInst();
        fc.LoadTransition = true;
        while (fc.Alpha < 1)
            yield return new WaitForSecondsRealtime(0.1f);
        EndSetupSequence();
    }

    private void GameEventPublisher_LevelInitialised()
    {
        GameEventPublisher.LevelInitialised -= GameEventPublisher_LevelInitialised;
        Debug.Log("EnableWorldRendering(true) from PSVRSetupManager.GameEventPublisher_LevelInitialised");
        EnableWorldRendering = true;
    }

#if UNITY_PS4
    void OnSystemServiceEvent(Utility.sceSystemServiceEventType eventType)
    {
        if (stateMachine == null)
            return;

        AnimatorStateInfo stateInfo = stateMachine.GetCurrentAnimatorStateInfo(0);
        if (eventType == Utility.sceSystemServiceEventType.ResetVrPosition && stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Recentering"))
        {
            var fc = RenderMode.GetInst();
            fc.UIFade = false;
            stateMachine.SetTrigger("Safety");
        }
    }
#endif

    [AdvancedInspector.Inspect]
    public void HmdDisconnected()
    {
        if(_inStartupSequence) // HMD reconnected during startup
        {
            stateMachine.SetTrigger("RestartSetup");
            SetupStart();
        }
        else
        {
            if (!_reconnect)
            {
                var mainCam = Camera.main;
                if (null != mainCam)
                {
                    transform.position = mainCam.transform.position;
                    transform.rotation = mainCam.transform.rotation;
                }
                Debug.Log("EnableWorldRendering(false) from HmdDisconnected");
                EnableWorldRendering = false;
                _reconnect = true;
                stateMachine.SetTrigger("Reconnect");
            }
        }
    }

    static int s_blankCount = 0;

    IEnumerator CalibrationBlank()
    {
        int blankCount = s_blankCount;
        Debug.LogFormat("CalibrationBlank - Start {0}", blankCount);
        var fade = RenderMode.GetInst();
        fade.UIFade = true;
        fade.InstantTransition();
        yield return new WaitForSecondsRealtime(2);
        fade.UIFade = false;
        Debug.LogFormat("CalibrationBlank - End {0}", blankCount);
    }

    void Instance_HMDConnected()
    {
        if (_inStartupSequence)
        {
            if(null==stateMachine)
                stateMachine = GetComponent<Animator>();
            AnimatorStateInfo stateInfo = stateMachine.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Warning"))
            {
                StartCoroutine(CalibrationBlank());
            }
        }
        else if (_reconnect)
        {
            _reconnect = false;
            stateMachine.SetTrigger("Hidden");
            Debug.Log("EnableWorldRendering(true) from Instance_HMDConnected");
            EnableWorldRendering = true;
            StartCoroutine(PostReconnectFadeIn());
        }
    }

    // When reconnecting, blackout for 2 seconds
    IEnumerator PostReconnectFadeIn()
    {
        var fc = RenderMode.GetInst();
        fc.ReconnectBlackout = true;
        fc.InstantTransition();
        yield return new WaitForSecondsRealtime(2.0f);
        fc.ReconnectBlackout = false;
    }
}
