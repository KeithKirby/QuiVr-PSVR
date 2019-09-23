using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderMode : MonoBehaviour
{
    public delegate void OnFadeRequestComplete();

    class FadeRequest
    {
        public string Name;
        public OnFadeRequestComplete Complete;
    }

    public float Alpha { get { return _alpha; } }

    float _fadeRate = 1.0f;
    float _alpha = 1;
    float _targetAlpha = 1;

    [AdvancedInspector.Inspect]
    public bool LevelStartFade
    {
        get { return _levelStartFade; }
        set
        {
            if (_levelStartFade != value)
            {
                //Debug.LogFormat("LevelStartFade = {0}", value);
                _levelStartFade = value;
                UpdateTargetAlpha();
            }
        }
    }
    bool _levelStartFade = false;

    [AdvancedInspector.Inspect]
    public bool LoadTransition
    {
        get { return _isLoading; }
        set
        {
            if (_isLoading != value)
            {
                //Debug.LogFormat("LoadTransition = {0}", value);
                _isLoading = value;
                UpdateTargetAlpha();
            }
        }
    }
    bool _isLoading = false;

    [AdvancedInspector.Inspect]
    public bool UIFade
    {
        get { return _isUIFade; }
        set
        {
            if (_isUIFade != value)
            {
                //Debug.LogFormat("UIFade = {0}", value);
                _isUIFade = value;
                UpdateTargetAlpha();
            }
        }
    }
    bool _isUIFade = false;

    [AdvancedInspector.Inspect]
    public bool StartupSequence
    {
        get { return _startupSequence; }
        set
        {
            if (_startupSequence != value)
            {
                //Debug.LogFormat("StartupSequence = {0}", value);
                _startupSequence = value;
                UpdateTargetAlpha();
            }
        }
    }
    bool _startupSequence = false;

    [AdvancedInspector.Inspect]
    public bool ReconnectBlackout
    {
        get { return _reconnectBlackout; }
        set
        {
            if (_reconnectBlackout != value)
            {
                //Debug.LogFormat("StartupSequence = {0}", value);
                _reconnectBlackout = value;
                UpdateTargetAlpha();
            }
        }
    }
    bool _reconnectBlackout = false;

    // Force to instantly blacked out, used to make debug look the same as a proper transition
    [AdvancedInspector.Inspect]
    public void ForceLoadTransition()
    {
        _isLoading = true;
        _alpha = _targetAlpha = 1;
    }

    [AdvancedInspector.Inspect]
    public bool IsTeleporting
    {
        get
        {
            return _isTeleporting;
        }
        set
        {
            if (_isTeleporting != value)
            {
                //Debug.LogFormat("IsTeleporting = {0}", value);
                _isTeleporting = value;
                UpdateTargetAlpha();
            }
        }
    }
    bool _isTeleporting = false;

    // Force to instantly blacked out, leave target alpha unchanged, will fade in if nothing is blocking
    [AdvancedInspector.Inspect]
    public void TeleportTransition(OnFadeRequestComplete fn)
    {
        //Debug.LogFormat("TeleportTransitionStart");
        _alpha = 1;
        IsTeleporting = true;
        WaitForFadeOut("Teleport",
            () =>
            {
                IsTeleporting = false;
                fn();
                //Debug.LogFormat("TeleportTransitionEnd");
            });
    }

    [AdvancedInspector.Group("CameraMask")]
    [AdvancedInspector.Inspect]
    public bool OptionsMenuActive
    {
        get
        {
            return _optionsMenuActive;
        }
        set
        {
            if (_optionsMenuActive != value)
            {
                _optionsMenuActive = value;
                UpdateCameraMask();
            }
        }
    }
    bool _optionsMenuActive = false;

    void UpdateTargetAlpha()
    {
        if (_isLoading || _isTeleporting || _isUIFade || _startupSequence || _levelStartFade || _reconnectBlackout)
            _targetAlpha = 1;
        else
            _targetAlpha = 0;
    }

    [AdvancedInspector.Inspect]
    public bool SystemEnvironmentActive
    {
        get
        {
            return _systemEnvironmentActive;
        }
        set
        {
            if (_systemEnvironmentActive != value)
            {
                _systemEnvironmentActive = value;
                UpdateCameraMask();
            }
        }
    }
    bool _systemEnvironmentActive = false;

    void UpdateCameraMask()
    {
        var mode = (_optionsMenuActive || _systemEnvironmentActive) ? CameraMaskMode.System : CameraMaskMode.Game;
        if(mode != MaskMode)
        {
            MaskMode = mode;
            if(null!=CameraMaskChanged)
                CameraMaskChanged(mode);
        }
    }

    public enum CameraMaskMode
    {
        Game,
        System
    }

    public delegate void OnCameraMaskChanged(CameraMaskMode mode);
    public event OnCameraMaskChanged CameraMaskChanged;

    public CameraMaskMode MaskMode = CameraMaskMode.Game;
    public LayerMask CurrentMask
    {
        get
        {
            return MaskMode == CameraMaskMode.Game ? GameCameraMask : SystemCameraMask;
        }
    }

    public LayerMask GameCameraMask;
    public LayerMask SystemCameraMask;

    void SetupPublisher_OptionsScreenActive(bool show)
    {
        OptionsMenuActive = show;
    }

    void SetupPublisher_SetupScreenActive(bool show)
    {
        SystemEnvironmentActive = show;
    }

    static RenderMode _inst;

    public static RenderMode GetInst() { return _inst; }

    private void Awake()
    {
        if (null == _inst)
        {
            _inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    void Start()
    {
        SetupPublisher.SetupScreenActive += SetupPublisher_SetupScreenActive;
        SetupPublisher.OptionsScreenActive += SetupPublisher_OptionsScreenActive;
        UpdateTargetAlpha();
    }

    List<FadeRequest> _fadeInRequests = new List<FadeRequest>();
    List<FadeRequest> _fadeOutRequests = new List<FadeRequest>();

    bool IsFadedOut
    {
        get
        {
            return _alpha == _targetAlpha && _targetAlpha == 1; // Already faded out
        }
    }

    public bool IsFadedIn
    {
        get
        {
            return _alpha == _targetAlpha && _targetAlpha == 0; // Already faded in
        }
    }

    public void WaitForFadeOut(string name, OnFadeRequestComplete fn)
    {
        _fadeOutRequests.Add(new FadeRequest { Name = name, Complete = fn });
        if (IsFadedOut) // Already done, fire on next frame
            StartCoroutine(FadeOutComplete());
            
    }

    public void WaitForFadeIn(string name, OnFadeRequestComplete fn)
    {
        _fadeInRequests.Add(new FadeRequest { Name = name, Complete = fn });
        if (IsFadedIn) // Already done, fire immediately
            StartCoroutine(FadeInComplete());
    }

    public void InstantTransition()
    {
        _alpha = _targetAlpha;
    }

    // Update is called once per frame
    void Update () {

        if (_alpha != _targetAlpha)
        {
            float dAlpha = _fadeRate * Time.fixedDeltaTime;
            if(_alpha<_targetAlpha)
            {
                _alpha += dAlpha;
                if (_alpha >= _targetAlpha)
                {
                    _alpha = _targetAlpha;
                    StartCoroutine(FadeOutComplete());
                }
            }
            else
            {
                _alpha -= dAlpha;
                if (_alpha <= _targetAlpha)
                {
                    _alpha = _targetAlpha;
                    StartCoroutine(FadeInComplete());
                }
            }
        }
	}

    IEnumerator FadeOutComplete()
    {
        for(int i=0;i<4;++i)
            yield return new WaitForEndOfFrame();
        foreach (var req in _fadeOutRequests)
            req.Complete();
        _fadeOutRequests.Clear();
    }

    IEnumerator FadeInComplete()
    {
        yield return new WaitForEndOfFrame();
        foreach (var req in _fadeInRequests)
            req.Complete();
        _fadeInRequests.Clear();
    }
}
