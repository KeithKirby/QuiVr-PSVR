using UnityEngine;
using System.Collections;
using VRTK;

public class HandAnim : MonoBehaviour, IHandAnim {

    VRTK_ControllerEvents events;
    VRTK_InteractGrab grab;
    Animation anim;
    public bool empty;
    public bool hasArrow;
    public bool drawing;
    public float inMenu;

    Vector3 origPos;
    Quaternion origRot;

    public float GripValue;

    KnucklesHandPose pose;

    FingerPositions fpos;
    public bool dummy;

    bool nocked;
    float prevMenu = -1;

    // Use this for initialization
    void Awake () {
        events = GetComponentInParent<VRTK_ControllerEvents>();
        grab = GetComponentInParent<VRTK_InteractGrab>();
        fpos = GetComponent<FingerPositions>();
        anim = GetComponent<Animation>();
        if (anim == null)
            anim = GetComponentInChildren<Animation>();
        anim.Play("Grab");
        anim["Grab"].speed = 0;
        origPos = transform.localPosition;
        origRot = transform.localRotation;
        HandGrab(GripValue);

	}

    public void ResetPosition()
    {
        transform.localPosition = origPos;
        transform.localRotation = origRot;
    }

    // Update is called once per frame
    void Update()
    {
        if (dummy)
            return;
        if ((hasArrow || drawing) && !nocked)
        {
            anim["Nock"].speed = 0;
            anim["Nock"].time = anim["Nock"].length;
            anim.Play("Nock");
            nocked = true;
        }
        else if (!empty)
        {
            HandGrab(1);
        }
        else if (inMenu > 0 && anim.GetClip("Point") != null)
        {
            anim["Point"].speed = 0;
            anim["Point"].time = anim["Point"].length * inMenu;
            anim.Play("Point");
            nocked = false;
        }
        else if (events != null)
        {
            HandGrab(events.GetTriggerAxis());
        }
        empty = false;
        hasArrow = false;
        if (grab != null && grab.GetGrabbedObject() == null)
            empty = true;
        else
        {
            if (grab != null && grab.GetGrabbedObject().GetComponent<ArrowNotch>() != null)
            {
                hasArrow = true;
            }
        }
    }

    float prevGrab = -1;
    public void HandGrab(float val)
    {
        if (anim == null)
            return;
        if(prevGrab != val)
        {
            prevGrab = val;
            anim["Grab"].speed = 0;
            GripValue = val;
            anim["Grab"].time = (val * anim["Grab"].length) * 0.98f;
            anim.Play("Grab");
        }
        nocked = false;
    }
        
    public void SetFingerPose(KnucklesHandPose p)
    {
        pose = p;  
    }

    public void LerpFingerPose(KnucklesHandPose p, float delta)
    {
        if (pose != null)
        {
            pose.thumb_lift = Mathf.Lerp(pose.thumb_lift, p.thumb_lift, delta);
            pose.index_curl = Mathf.Lerp(pose.index_curl, p.index_curl, delta);
            pose.middle_curl = Mathf.Lerp(pose.middle_curl, p.middle_curl, delta);
            pose.ring_curl = Mathf.Lerp(pose.ring_curl, p.ring_curl, delta);
            pose.pinky_curl = Mathf.Lerp(pose.pinky_curl, p.pinky_curl, delta);
            pose.thumbPos = Vector2.Lerp(pose.thumbPos, p.thumbPos, delta);
            pose.squeeze = Mathf.Lerp(pose.squeeze, p.squeeze, delta);
        }
        else
            pose = p;
    }

    void LateUpdate()
    {
        if (fpos != null && pose != null && (KnucklesHandControl.UsingFingers || dummy))
        {
            fpos.UpdatePose(pose);
        }
    }
}
