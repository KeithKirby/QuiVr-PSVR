using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModelsAnimations : MonoBehaviour {

    Animation _anim;

    int _clipsCount;
    int _curClip;

    void NextAnim()
    {
        int index = 0;
        string clipName = string.Empty;
        foreach(AnimationState clip in _anim)
        {
            if (index == _curClip)
                clipName = clip.name;

            index++;
        }

        _anim.Play(clipName, PlayMode.StopAll);

        _curClip++;
        if (_curClip > _clipsCount)
            _curClip = 0;
    }

	// Use this for initialization
	void Start () {
        _anim = GetComponent<Animation>();
        if (null == _anim)
            return;

        _clipsCount = _anim.GetClipCount();

        InvokeRepeating("NextAnim", 0, 4);
    }
	
}
