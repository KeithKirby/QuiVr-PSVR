using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class BowAudio : MonoBehaviour {

    public AudioSource src;
    public AudioClip[] Twangs;
    public AudioClip[] Nocks;
    public BowHandle Handle;

	// Use this for initialization
	void Start () {
        if(src == null)
            src = GetComponent<AudioSource>();
	}

    public void Play(float vol, float pitch)
    {
        src.pitch = pitch;
        src.volume = vol;
        src.time = 0;
        src.Play();
    }

    public void PlayNock()
    {
        if(Nocks.Length > 0 && Handle != null)
            VRAudio.PlayClipAtPoint(Nocks[Random.Range(0, Nocks.Length)], Handle.arrowNockingPoint.position, 0.8f, 0.8f);
        else if (Twangs.Length > 0)
            VRAudio.PlayClipAtPoint(Nocks[Random.Range(0, Nocks.Length)], transform.position, 0.8f, 0.8f);
    }

    public void PlayTwang(float vol)
    {
        if(Twangs.Length > 0 && Handle != null)
            VRAudio.PlayClipAtPoint(Twangs[Random.Range(0, Twangs.Length)], Handle.arrowNockingPoint.position, vol);
        else if(Twangs.Length > 0)
            VRAudio.PlayClipAtPoint(Twangs[Random.Range(0, Twangs.Length)], transform.position, vol);
    }

    void Update()
    {
        //ModulatePullbackSound
        //src.pitch = Random.Range(0.85f, 1.1f);
    }

    public void SetVolume(float val)
    {
        src.volume = val;
    }
}
