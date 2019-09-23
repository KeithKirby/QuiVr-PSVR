using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormAnimations : MonoBehaviour {

    Animation anim;
    public Anim[] animations;

    public string startAnim;

    public string debugAnim;
    Anim currentAnim;

    void Awake()
    {
        anim = GetComponent<Animation>();
    }

    void Start()
    {
        PlayAnim(startAnim);
    }

    [AdvancedInspector.Inspect]
    public void PlayDebug()
    {
        PlayAnim(debugAnim);
    }

    public bool isPlaying()
    {
        return anim.isPlaying;
    }

    public float GetAnimLength(string s)
    {
        Anim a = GetAnim(s);
        if(a != null)
        {
            anim[a.animName].speed = a.speed;
            return anim[a.animName].length;
        }
        return 0.5f;
    }

    public float GetAnimSpeed(string s)
    {
        Anim a = GetAnim(s);
        if (a != null)
        {
            return anim[a.animName].speed;
        }
        return 0.5f;
    }

    public void PlayAnim(string name)
    {
        Anim a = GetAnim(name);
        if (a != null && anim[a.animName] != null)
        {
            currentAnim = a;
            anim[a.animName].speed = a.speed;
            if (a.crossfadeSpeed > 0)
                anim.CrossFade(a.animName, a.crossfadeSpeed);
            else
                anim.Play(a.animName);
        }
        else
            Debug.Log("Anim: " + name + " does not exist");
    }

    Anim GetAnim(string name)
    {
        for(int i=0; i<animations.Length;i++)
        {
            if (animations[i].name == name)
                return animations[i];
        }
        return null;
    }

    [System.Serializable]
    public class Anim
    {
        public string name;
        public string animName;
        public float speed = 0.5f;
        public float crossfadeSpeed = 3f;

        public override string ToString()
        {
            if (name != null)
                return name;
            return base.ToString();
        }
    }
}
