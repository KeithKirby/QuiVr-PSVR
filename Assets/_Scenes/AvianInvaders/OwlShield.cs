using UnityEngine;
using System.Collections;

public class OwlShield : MonoBehaviour {

    public AAManager mgr;
    bool startedFirst;

    bool playingClip;

    AudioSource src;

    public AudioClip[] hitClips;

    void Start()
    {
        src = GetComponent<AudioSource>();
    }

	public void ArrowImpact()
    {
        Debug.Log("Arrow Hit Shield");
        if(!mgr.isPlaying() && startedFirst)
        {
            mgr.NewGame();
        }
        else if(mgr.isPlaying())
        {
            if(!src.isPlaying && Random.Range(0, 100) < 15)
            {
                src.clip = hitClips[Random.Range(0, hitClips.Length)];
                src.time = 0;
                src.Play();
            }
        }
    }

    public void InitStart()
    {
        startedFirst = true;
    }
}
