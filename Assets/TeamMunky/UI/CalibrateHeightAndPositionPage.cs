using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class CalibrateHeightAndPositionPage : MonoBehaviour
{
    public delegate void OnComplete();
    public event OnComplete Complete;

    public Slider SliderRef;

    public float ConfirmDuration = 2;
    public float MinHeight = 0.8f;
    public float MaxHeight = 2.5f;
    float _confirmTimer = 0;
    bool _confirmed = false;

    float _heightSum = 0;
    int _heightMeasurements = 0;
    
	// Use this for initialization
	void Start () {
		
	}

    void OnEnable()
    {
        _confirmed = false;
    }

    static void DumpTransformHeirarchy(Transform go)
    {
        var objs = new List<string>();
        for(var g = go;g != null; g = g.parent)
        {
            var obj = g.gameObject;
            if(obj!=null)
            {
                objs.Add(string.Format("{0} {1} {2}\n", obj.name, g.localScale, g.lossyScale));
            }
            else
            {
                objs.Add(string.Format("noGameobject {1} {2}\n", obj.name, g.localScale, g.lossyScale));
            }
        }
        Debug.Log(string.Join(string.Empty, objs.ToArray()));
    }

    void TakeHeightReading()
    {
        // DaVinci method
        var trackedDevices = GameObject.FindObjectsOfType<TrackedPlayStationDevices>();
        foreach (var dev in trackedDevices)
        {
            if (dev.isActiveAndEnabled)
            {
                var moveA = dev.deviceMovePrimary.transform.position;
                var moveB = dev.deviceMoveSecondary.transform.position;
                float rawHeight = (moveA - moveB).magnitude;
                //rawHeight = rawHeight / CameraRigs.ActiveRig.CameraStartScale;
                _heightSum += rawHeight;
                ++_heightMeasurements;

            }
        }
    }

    void CalculateHeight()
    {
        var avgHeight = _heightMeasurements != 0 ? _heightSum / _heightMeasurements : 0;        
        PSVRManager.PlayerHeight = Mathf.Clamp(avgHeight, MinHeight, MaxHeight);
        //PSVRManager.PlayerScale = CameraRigs.ActiveRig.CameraStartScale;
        Debug.LogFormat("Measured height raw {0} clamped {1}", avgHeight, PSVRManager.PlayerHeight);
        if(null!=CameraRigs.ActiveRig)
            CameraRigs.ActiveRig.UpdateHeight();
        GameEventPublisher.PublishHeightChanged();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!_confirmed)
        {
            var grabRight = PS4InputEx.GrabRight();
            bool grab = grabRight != 0 && grabRight > -1;

            if (PS4InputEx.Zap(0) || PS4InputEx.Zap(1) || grab)
            {
                TakeHeightReading();

                _confirmTimer += Time.unscaledDeltaTime;
                if (_confirmTimer >= ConfirmDuration)
                {
                    _confirmed = true;
                    SliderRef.value = 1;
                    CalculateHeight();
                    UnityEngine.XR.InputTracking.Recenter();
                    if (null != Complete)
                        Complete();
                }
                else
                {
                    SliderRef.value = _confirmTimer / ConfirmDuration;
                }
            }
            else
            {
                if (_confirmTimer != 0)
                {
                    _confirmTimer = 0;
                    SliderRef.value = 0;
                    _heightSum = 0;
                    _heightMeasurements = 0;
                }
            }
        }
	}
}
