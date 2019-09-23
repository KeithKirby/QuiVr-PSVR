using UnityEngine;
using System.Collections;

public class HandAnimTest : MonoBehaviour {

    public string ClipName;
    [Range(0,1)]
    public float perc;
    public bool continuous;
    Animation anim;

    void Awake()
    {
        anim = GetComponent<Animation>();
    }

    void Update()
    {
        if(continuous)
        {
            ChangeFrame();
        }
    }

    [BitStrap.Button]
    public void ChangeFrame()
    {
        anim[ClipName].speed = 0;
        anim[ClipName].time = (anim[ClipName].length * perc)*0.95f;
        anim.Play(ClipName);
    }

    [BitStrap.Button]
    public void PlayAnim()
    {
        anim[ClipName].speed = 1;
        anim[ClipName].time = 0;
        anim.Play(ClipName);
    }
}
