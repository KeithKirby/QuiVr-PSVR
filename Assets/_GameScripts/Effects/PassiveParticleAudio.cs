using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveParticleAudio : MonoBehaviour {

    AudioSource src;
    public ParticleSystem psyst;
    [HideInInspector]
    public float t = 0;

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(psyst != null)
        {
            if(psyst.isPlaying)
                t += Time.deltaTime;
            else
                t -= Time.deltaTime;
            if (t > 1) t = 1;
            else if (t < 0) t = 0;
            src.volume = t;
        }
    }
}
