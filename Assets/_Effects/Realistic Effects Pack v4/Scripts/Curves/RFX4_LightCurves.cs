using UnityEngine;
using System.Collections;

public class RFX4_LightCurves : MonoBehaviour
{
    public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float GraphTimeMultiplier = 1, GraphIntensityMultiplier = 1;
    public bool IsLoop;

    [HideInInspector] public bool canUpdate;
    private float startTime;
    private Light lightSource;

    public bool OnStart= true;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
        lightSource.intensity = LightCurve.Evaluate(0);
    }

    private void OnEnable()
    {
        startTime = Time.time;
        if(OnStart)
            canUpdate = true;
    }

    private void Update()
    {
        var time = Time.time - startTime;
        if (canUpdate) {
            var eval = LightCurve.Evaluate(time / GraphTimeMultiplier) * GraphIntensityMultiplier;
            if(reverse)
                eval = LightCurve.Evaluate(1-(time / GraphTimeMultiplier)) * GraphIntensityMultiplier;
            lightSource.intensity = eval;
        }
        if (time >= GraphTimeMultiplier) {
            if (IsLoop) startTime = Time.time;
            else canUpdate = false;
        }
    }

    bool reverse;
    public void Reverse()
    {
        reverse = true;
        startTime = Time.time;
        canUpdate = true;
    }

    public void Play()
    {
        reverse = false;
        startTime = Time.time;
        canUpdate = true;
    }
}