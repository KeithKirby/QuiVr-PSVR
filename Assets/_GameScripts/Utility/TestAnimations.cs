using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimations : MonoBehaviour {

    Animation anim;
    public string animToPlay;
    public float speed = 1f;

	void Awake()
    {
        anim = GetComponent<Animation>();
    }

    [AdvancedInspector.Inspect]
    public void PlayAnim()
    {
        if(anim != null && anim[animToPlay] != null)
        {
            anim[animToPlay].speed = speed;
            anim.CrossFade(animToPlay, 1);
        }
    }
}
