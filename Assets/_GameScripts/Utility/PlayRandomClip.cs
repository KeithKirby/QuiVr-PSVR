using UnityEngine;
using System.Collections;

public class PlayRandomClip : MonoBehaviour {

    AudioSource src;
    public AudioClip[] Clips;

    public bool PlayOnStart = true;
    public AudioType ClipType = AudioType.Master;


    float startVol;
    float startPitch;
	void Awake()
    {
        src = GetComponent<AudioSource>();
        startVol = src.volume;
        startPitch = src.pitch;
    }

    void Start()
    {
        if(PlayOnStart)
        { Play(); } 
    }

    public AudioClip Play()
    {
        if (src != null && Clips.Length > 0)
        {
            AudioClip c = Clips[Random.Range(0, Clips.Length)];
            src.clip = c;
            src.pitch = startPitch;
            src.volume = startVol * VolumeSettings.GetVolume(ClipType);          
            src.Play();
            return c;
        }
        return null;
    }

    public void PlayWithPitch(float pitch, float volMult=1)
    {
        if (src != null && Clips.Length > 0)
        {
            src.clip = Clips[Random.Range(0, Clips.Length)];
            src.pitch = pitch;
            src.volume = startVol * VolumeSettings.GetVolume(ClipType) * volMult;
            src.Play();
        }
    }
}
