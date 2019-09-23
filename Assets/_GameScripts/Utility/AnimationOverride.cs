using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverride : MonoBehaviour {

    public Animation Anim;
    public BoneSection[] BoneSections;
    public Transform DefaultIgnore;
    public Transform[] AdditionalIgnores;
    public OverridePosition[] Positions;

    public bool IgnoreStart;

    void Awake()
    {
        if (Anim == null)
            Anim = GetComponent<Animation>();
    }

    void Start()
    {
        foreach(AnimationState a in Anim)
        {
            foreach(BoneSection t in BoneSections)
            {
                a.AddMixingTransform(t.Bone, t.shouldRecurse);
            }
        }
        if(IgnoreStart)
            StartIgnore();
    }

    public void StartIgnore()
    {
        foreach (AnimationState a in Anim)
        {
            a.RemoveMixingTransform(DefaultIgnore);
            if(AdditionalIgnores.Length > 0)
            {
                foreach(var i in AdditionalIgnores)
                {
                    a.RemoveMixingTransform(i);
                }
            }
        }
        foreach(var v in Positions)
        {
            v.SetRotation();
        }
    }

    public void StopIgnore()
    {
        foreach (AnimationState a in Anim)
        {
            a.AddMixingTransform(DefaultIgnore);
            if (AdditionalIgnores.Length > 0)
            {
                foreach (var i in AdditionalIgnores)
                {
                    a.AddMixingTransform(i);
                }
            }
        }
    }

    [BitStrap.Button]
    public void SaveRotations()
    {
        for(int i=0; i<Positions.Length; i++)
        {
            Positions[i].SaveRotation();
        }
    }

    [System.Serializable]
    public class BoneSection
    {
        public Transform Bone;
        public bool shouldRecurse;

        public override string ToString()
        {
            if (Bone != null)
                return Bone.name;
            return base.ToString();
        }
    }

    [System.Serializable]
    public class OverridePosition
    {
        public Transform Bone;
        public Vector3 LockRotation;

        public void SaveRotation()
        {
            LockRotation = Bone.localEulerAngles;
        }

        public void SetRotation()
        {
            Bone.localEulerAngles = LockRotation;
        }
    }
}
