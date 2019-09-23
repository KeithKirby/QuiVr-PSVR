using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchOnAwake : MonoBehaviour {

    public Vector2 PitchRange;

    void Awake()
    {
        AudioSource src = GetComponent<AudioSource>();
        if(src != null)
        {
            src.pitch = Random.Range(PitchRange.x, PitchRange.y);
        }
    }
}
