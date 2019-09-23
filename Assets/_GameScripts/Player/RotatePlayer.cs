using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.Events;

public class RotatePlayer : MonoBehaviour {
    
    bool Pressed;

    float _rotation = 0;
    float _rotationDirection = 1;
    float _easeTimer = 0;    
    float _rotationIncrement = 45.0f;

    public float EaseDuration = 0.1f;
    public float BlurMax = 0.03f;

    public Transform PlayerCam;

    public VRDirectionalBlur BlurEffect;

    AudioSource _source;

    void Start()
    {
        _rotation = transform.localEulerAngles.y;
        _source = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        PSVRManager.Recenter += PSVRManager_Recenter;
    }

    void OnDisable()
    {
        PSVRManager.Recenter -= PSVRManager_Recenter;
    }

    private void PSVRManager_Recenter()
    {
        transform.localPosition = Vector3.zero;
        Debug.Log("RotatePlayer:Recenter");
    }

    void Update() 
    {  
        if(BowAim.instance != null)
        {
            if (AppBase.v.controls == ControllerType.OculusTouch)
            {
                var stringActions = BowAim.instance.stringActions;
                if (stringActions != null)
                {
                    VRTK_ControllerEvents ev = stringActions.GetComponent<VRTK_ControllerEvents>();
                    if (ev != null)
                    {
                        Vector2 stickAxis = ev.GetTouchpadAxis();
                        if (Mathf.Abs(stickAxis.x) > 0.8f && !Pressed)
                        {
                            Pressed = true;
                            DoRotate((int)Mathf.Sign(stickAxis.x));
                        }
                        else if (Mathf.Abs(stickAxis.x) < 0.2f)
                            Pressed = false;
                    }
                }
            }
        }
#if UNITY_PS4
        if (PS4InputEx.MenuCDown(1))
            DoRotate(1);
        if (PS4InputEx.MenuDDown(1))
            DoRotate(-1);
#endif

        if (_easeTimer != EaseDuration)
        {
            _easeTimer += Time.unscaledDeltaTime;         
            if (_easeTimer < EaseDuration)
            {
                float t = _easeTimer / EaseDuration;
                t = Mathf.Sqrt(t);
                if (null != BlurEffect)
                {
                    BlurEffect.enabled = true;
                    BlurEffect.BlurAmount = t * Mathf.LerpAngle(BlurMax, 0, t) * _rotationDirection;
                }
            }
            else
            {
                _easeTimer = EaseDuration;
                if (null != BlurEffect)
                {
                    BlurEffect.enabled = false;
                    BlurEffect.BlurAmount = 0;
                }
            }   
        }
    }

    void DoRotate(int dir)
    {
        _rotationDirection = -dir;
        if (dir < 0)
        {
            _rotation += _rotationIncrement;
            while(_rotation > 360)
                _rotation -= 360;
        }
        else
        {
            _rotation -= _rotationIncrement;
            while (_rotation < 0)
                _rotation += 360;
        }
        if(null!=_source)
            _source.Play();
        _easeTimer = 0;
        Vector3 angle = transform.eulerAngles;
        angle.y = _rotation;

        if (null != PlayerCam)
        {
            var beforePos = PlayerCam.transform.position;
            transform.eulerAngles = angle;
            var afterPos = PlayerCam.transform.position;
            var correction = beforePos - afterPos;
            transform.position = transform.position + correction;
        }
        //Debug.LogFormat("BeforePos {0} AfterPos {1}", beforePos, afterPos);
    }
}