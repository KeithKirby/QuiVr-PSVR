//=============================================================================
//
// Purpose: Interprets knuckles input and animates a hand model
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KnucklesHandPose
{
    public float middle_curl = 1;
    public float ring_curl = 1;
    public float pinky_curl = 1;
    public float index_curl = 1;
    public Vector2 thumbPos = Vector2.zero;
    public float thumb_lift = 0.0f;
    public float squeeze = 0.0f;

    public KnucklesHandPose(float m = 1, float r = 1, float p = 1, float i = 1, Vector2 tp = default(Vector2), float tl = 0, float s = 0)
    {
        middle_curl = m;
        ring_curl = r;
        pinky_curl = p;
        index_curl = i;
        thumbPos = tp;
        thumb_lift = tl;
        squeeze = s;
    }

    public KnucklesHandPose(float[] vals)
    {
        if(vals.Length > 0)
            thumb_lift = vals[0];
        if (vals.Length > 1)
            index_curl = vals[1];
        if (vals.Length > 2)
            middle_curl = vals[2];
        if (vals.Length > 3)
            ring_curl = vals[3];
        if (vals.Length > 4)
            pinky_curl = vals[4];
    }

    public bool isValid()
    {
        return middle_curl < 1 || ring_curl < 1 || pinky_curl < 1 || index_curl < 0.5f;
    }

    public float[] fingerVals()
    {
        List<float> vals = new List<float>();
        vals.Add(thumb_lift);
        vals.Add(index_curl);
        vals.Add(middle_curl);
        vals.Add(ring_curl);
        vals.Add(pinky_curl);
        return vals.ToArray();
    }

    public float AvgVal()
    {
        return (index_curl + middle_curl + ring_curl + pinky_curl) / 4f;
    }
}

public class KnucklesHandControl : MonoBehaviour
{

    public enum WhichHand
    {
        Left,
        Right
    }

    public static bool UsingFingers;

    public WhichHand whichHand;

    public SteamVR_TrackedObject controller;

    public KnucklesHandPose handPose;

    private float squeeze = 0.0f;

    private Vector2 trackpad_pos;

    private float trigger = 0.0f;
    private bool trigger_touched = false;

    IHandAnim anim;

    [HideInInspector]
    public SteamVR_Controller.Device vrcontroller;

    [Tooltip("use older vr controllers for testing. Trigger controls all fingers")]
    public bool emulate;

    float[] clamps = new float[6];

    void Start()
    {
        handPose = new KnucklesHandPose();
        if(SteamVR.enabled)
            vrcontroller = SteamVR_Controller.Input((int)controller.index);
    }

    public void SetClamp(int finger, float val = -1)
    {
        if (val == -1)
        {
            val = getposfromindex(finger);

            clamps[finger] = val;
        }
        else
        {
            clamps[finger] = val;
        }
    }

    public float getposfromindex(int i)
    {
        float val = 1;
        if (i == 0)
            val = handPose.thumb_lift;
        if (i == 1)
            val = handPose.index_curl;
        if (i == 2)
            val = handPose.middle_curl;
        if (i == 3)
            val = handPose.ring_curl;
        if (i == 4)
            val = handPose.pinky_curl;
        //if (i > 4 || i < 0)
        //	val = 1;

        return val;
    }

    public void TriggerHaptics(int length)
    {
        if (vrcontroller != null)
        {
            vrcontroller.TriggerHapticPulse((ushort)length);
        }
    }

    public bool touchingTrigger()
    {
        return trigger_touched;
    }

    // Update is called once per frame
    void Update()
    {
        if (!SteamVR.enabled || vrcontroller == null)
            return;
        //if (vrcontroller == null) {
        vrcontroller = SteamVR_Controller.Input((int)controller.index);
        //}

        /*
        if(anim == null)
        {
            anim = GetComponentInChildren<IHandAnim>();
            return;
        }
        */

        // Get finger curl axes and apply filtering
        if (!emulate)
        {
            handPose.index_curl = Mathf.Lerp(handPose.index_curl, 1f - vrcontroller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis3).x, 30.0f * Time.deltaTime);
            handPose.middle_curl = Mathf.Lerp(handPose.middle_curl, 1f - vrcontroller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis3).y, 30.0f * Time.deltaTime);
            handPose.ring_curl = Mathf.Lerp(handPose.ring_curl, 1f - vrcontroller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis4).x, 30.0f * Time.deltaTime);
            handPose.pinky_curl = Mathf.Lerp(handPose.pinky_curl, 1f - vrcontroller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis4).y, 15.0f * Time.deltaTime);
            if (handPose.middle_curl < 1 || handPose.ring_curl < 1 || handPose.pinky_curl < 1)
                UsingFingers = true;
            handPose.thumb_lift = Mathf.Lerp(handPose.thumb_lift, vrcontroller.GetTouch(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) ? 0 : 1, Time.deltaTime * 10);

            // Grab trigger position and adjust index finger
            trigger = vrcontroller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
            trigger_touched = handPose.index_curl < 0.1f;
            //trigger_touched = vrcontroller.GetTouch(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
            handPose.index_curl = 0.9f * handPose.index_curl + -0.1f * trigger;

        }
        else
        { //emulates finger tracking using old controller trigger axis.
            float triggerAmount = 1 - vrcontroller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
            handPose.index_curl = triggerAmount;
            handPose.middle_curl = triggerAmount;
            handPose.ring_curl = triggerAmount;
            handPose.pinky_curl = triggerAmount;
            handPose.thumb_lift = triggerAmount;
        }


        // Grab trackpad coords for showing thumb position
        // Note: X-axis is flipped on the left controller since we use the same animation
        bool xflip = whichHand == WhichHand.Left;
        trackpad_pos = new Vector2((xflip ? 1 : 0) + (xflip ? -1 : 1) * (vrcontroller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x + 1.0f) / 2.0f, (vrcontroller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y + 1.0f) / 2.0f);

        handPose.thumbPos = trackpad_pos;



        // Calculate squeeze heuristic
        float squeeze_new;
        //squeeze_new = Mathf.Max(0, -middle_curl*15f) + Mathf.Max(0, -ring_curl*15f) + Mathf.Max(0, -index_curl*15f);
        //squeeze_new = Mathf.Max(Mathf.Max(0, -middle_curl * 20f), Mathf.Max(0, -ring_curl * 30f), Mathf.Max(0, -pinky_curl * 30f));
        //squeeze_new = Mathf.Max(Mathf.Max(0, -middle_curl * 20f), Mathf.Max(0, -ring_curl * 25f), Mathf.Max(0, -pinky_curl * 25f));
        //squeeze_new = Mathf.Max(Mathf.Max(0, -middle_curl * 3.0f), Mathf.Max(0, -ring_curl * 3.0f), Mathf.Max(0, -pinky_curl * 2f));
        squeeze_new = Mathf.Max(Mathf.Max(0, -handPose.middle_curl * 20f), Mathf.Max(0, -handPose.ring_curl * 20f), Mathf.Max(0, -handPose.pinky_curl * 25f));
        handPose.squeeze = Mathf.Lerp(handPose.squeeze, squeeze_new, 30.0f * Time.deltaTime);

        clampPose();

        /* Set animation times
        anim.Play("Ungrasp_Middle", 1, handPose.middle_curl);
        anim.Play("Ungrasp_Ring", 2, handPose.ring_curl);
        anim.Play("Ungrasp_Pinky", 3, handPose.pinky_curl);
        anim.Play("Ungrasp_Index", 4, handPose.index_curl);
        //anim.Play("Ungrasp_Thumb", 5, sockserv.thumb_curl);
        anim.Play("Trackpad_X", 6, handPose.thumbPos.x);
        anim.Play("Trackpad_Y", 7, handPose.thumbPos.y);
        anim.Play("Squeeze", 5, handPose.squeeze);
        anim.Play("Thumb_Lift", 8, handPose.thumb_lift);
        anim.speed = 0;
        */
        //anim.SetFingerPose(handPose);
    }


    void clampPose()
    { // apply clamping for finger positions
        handPose.thumb_lift = Mathf.Clamp(handPose.thumb_lift, clamps[0], 1);
        handPose.index_curl = Mathf.Clamp(handPose.index_curl, clamps[1], 1);
        handPose.middle_curl = Mathf.Clamp(handPose.middle_curl, clamps[2], 1);
        handPose.ring_curl = Mathf.Clamp(handPose.ring_curl, clamps[3], 1);
        handPose.pinky_curl = Mathf.Clamp(handPose.pinky_curl, clamps[4], 1);
    }

}