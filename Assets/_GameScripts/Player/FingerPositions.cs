using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPositions : MonoBehaviour {

    public FingerInfo[] Fingers;

    void Awake()
    {
        foreach(var v in Fingers)
        {
            v.Setup();
        }
    }

	public void UpdatePose(KnucklesHandPose pose)
    {
        float[] fVals = pose.fingerVals();
        for(int i=0; i<Mathf.Min(Fingers.Length, fVals.Length); i++)
        {
            Fingers[i].Set(fVals[i]);
        }
    }

    [AdvancedInspector.Inspect]
    public void SetEnds()
    {
        for (int i = 0; i < Fingers.Length; i++)
        {
            Fingers[i].SetEnd();
        }
    }

    [AdvancedInspector.Inspect]
    public void SetFingers()
    {
        //Thumb
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains("Thumb_01"))
                Fingers[0].TopJoint = child;
            if (child.name.Contains("Thumb_02"))
                Fingers[0].MidJoint = child;
            if (child.name.Contains("Thumb_03"))
                Fingers[0].BottomJoint = child;

            if (child.name.Contains("Index_01"))
                Fingers[1].TopJoint = child;
            if (child.name.Contains("Index_02"))
                Fingers[1].MidJoint = child;
            if (child.name.Contains("Index_03"))
                Fingers[1].BottomJoint = child;

            if (child.name.Contains("Middle_01"))
                Fingers[2].TopJoint = child;
            if (child.name.Contains("Middle_02"))
                Fingers[2].MidJoint = child;
            if (child.name.Contains("Middle_03"))
                Fingers[2].BottomJoint = child;

            if (child.name.Contains("Ring_01"))
                Fingers[3].TopJoint = child;
            if (child.name.Contains("Ring_02"))
                Fingers[3].MidJoint = child;
            if (child.name.Contains("Ring_03"))
                Fingers[3].BottomJoint = child;

            if (child.name.Contains("Pinky_01"))
                Fingers[4].TopJoint = child;
            if (child.name.Contains("Pinky_02"))
                Fingers[4].MidJoint = child;
            if (child.name.Contains("Pinky_03"))
                Fingers[4].BottomJoint = child;
        }
    }

    [System.Serializable]
    public class FingerInfo
    {
        public string Name;
        public Transform TopJoint;
        public Transform MidJoint;
        public Transform BottomJoint;
        Quaternion StartTop;
        Quaternion StartMid;
        Quaternion StartBot;
        public Quaternion EndTop;
        public Quaternion EndMid;
        public Quaternion EndBot;

        public void Setup()
        {
            if (TopJoint != null)
                StartTop = TopJoint.localRotation;
            if (MidJoint != null)
                StartMid = MidJoint.localRotation;
            if (BottomJoint != null)
                StartBot = BottomJoint.localRotation;
        }

        public void Set(float val)
        {
            val = 1 - val;
            if (TopJoint != null)
                TopJoint.localRotation = Quaternion.Lerp(StartTop, EndTop, val);
            if (MidJoint != null)
                MidJoint.localRotation = Quaternion.Lerp(StartMid, EndMid, val);
            if (BottomJoint != null)
                BottomJoint.localRotation = Quaternion.Lerp(StartBot, EndBot, val);
        }

        public override string ToString()
        {
            if (Name != null)
                return Name;
            return "Finger";
        }

        public void SetEnd()
        {
            if (TopJoint != null)
                EndTop = TopJoint.localRotation;
            if (MidJoint != null)
                EndMid = MidJoint.localRotation;
            if (BottomJoint != null)
                EndBot = BottomJoint.localRotation;
        }
    }
}
