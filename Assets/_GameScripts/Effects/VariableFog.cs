using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableFog : MonoBehaviour {

    public Camera myCamera;
    public Color fogColor;
    public AnimationCurve CameraViewDist;
    public AnimationCurve FogDistanceMax;
    public AnimationCurve FogDistanceMin;

    [Range(0,1)]
    public float SetVal;

    public bool Mobile;

    static List<VariableFog> AllFogs;

    public void Awake()
    {
        if (AllFogs == null)
            AllFogs = new List<VariableFog>();
        AllFogs.Add(this);
        myCamera = GetComponent<Camera>();
    }

    [AdvancedInspector.Inspect]
    void Apply()
    {
        if(myCamera == null)
            myCamera = GetComponent<Camera>();
        ChangeSettings(SetVal);
    }

    void Start()
    {
        if(Mobile)
        {
            ChangeSettings(0);
            return;
        }
        if (Settings.HasKey("ViewDist"))
            ChangeSettings(Settings.GetFloat("ViewDist"));
        else
            ChangeSettings(0.75f);
    }


    public void ChangeSettings(float val)
    {
        RenderSettings.fogEndDistance = FogDistanceMax.Evaluate(val);
        foreach(var v in AllFogs)
        {
            v.ChangeSettingsAll(val);
        }
    }

    public void ChangeSettingsAll(float val)
    {
        if (myCamera == null)
            myCamera = GetComponent<Camera>();
        if (myCamera != null)
            myCamera.farClipPlane = CameraViewDist.Evaluate(val) + 10;
    }

    void OnDestroy()
    {
        if(AllFogs != null)
            AllFogs.Remove(this);
    }
}
