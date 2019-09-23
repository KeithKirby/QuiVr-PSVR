using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderOffsetCurve : MonoBehaviour {

    public string TextureOffset = "_MainTex";
    public AnimationCurve XCurve;
    public AnimationCurve YCurve;
    public float TimeMult = 1f;
    public bool OnStart;
    public bool Loop;

    Renderer r;

    bool playing;
    float t = 0;
    float dir = 1;

	void Start () {
        r = GetComponent<Renderer>();
        if (OnStart)
            Play();
	}

    public void Play()
    {
        playing = true;
        dir = 1;
    }

    public void Stop()
    {
        playing = false;
    }

    public void Reverse()
    {
        playing = true;
        t = 1;
        dir = -1;
    }
	
	void Update () {
		if(playing && r != null)
        {
            t += (Time.deltaTime/TimeMult) * dir;
            r.material.SetTextureOffset(TextureOffset, new Vector2(XCurve.Evaluate(t), YCurve.Evaluate(t)));
            if(Loop)
            {
                if (dir < 0 && t <= 0)
                    t = 1;
                else if (dir > 0 && t >= 1)
                    t = 0;
            }
        }
	}
}
