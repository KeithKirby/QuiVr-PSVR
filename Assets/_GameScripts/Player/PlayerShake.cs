using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShake : MonoBehaviour {

    public static PlayerShake instance;
    float pow;
    float dur;
    public float ShakeSpeed = 3f;
    public int RandomModSpeed;
    public float MovementMult;
    public float RotationMult;
    public float DebugPow;
    public float DebugTime;
    bool useHaptics;

    void Awake()
    {
        instance = this;
    }

    [AdvancedInspector.Inspect]
    public void DebugShake()
    {
        Shake(DebugPow, DebugTime, false);
    }

	public static void Shake(float power, float duration, bool useHaptics)
    {
        if(instance != null)
        {
            instance.pow = power;
            instance.dur = duration;
            instance.mod = 0;
            instance.useHaptics = useHaptics;
        }
    }

    Vector3 mmnt;
    int mod = 0;
    void Update()
    {
        if (dur > 0)
        {
            dur -= Time.deltaTime;
            //transform.localPosition += Time.deltaTime*(SmoothRandom.GetVector3(pow * dur) - (Vector3.one*0.5f))*2f;
            float durMult = dur;
            if (dur > 1)
                durMult = 1;
            //Random Movement
            if (mod % RandomModSpeed == 0)
                mmnt = Random.insideUnitSphere;
            mod++;
            Vector3 move = mmnt * pow * durMult;
            transform.localPosition = Vector3.Lerp(transform.localPosition, move*MovementMult, Time.deltaTime * ShakeSpeed);
            //Random Rotation
            Quaternion rotate = Quaternion.Euler(move*RotationMult);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotate, Time.deltaTime * ShakeSpeed);

            //Haptics
            if(useHaptics && Time.timeScale == 1.0f) // Disable haptics if paused
            {
                if (BowAim.instance.stringActions != null)
                    BowAim.instance.stringActions.TriggerHapticPulse((ushort)(pow * durMult * 750));
                if (BowAim.instance.holdActions != null)
                    BowAim.instance.holdActions.TriggerHapticPulse((ushort)(pow * durMult * 750));
            }
        }
        else if(transform.localPosition != Vector3.zero || transform.localRotation != Quaternion.identity)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * ShakeSpeed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * ShakeSpeed);
        }
            
    }

    
}