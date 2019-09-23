using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class TeleportAudio : MonoBehaviour {

    AudioSource src;
    public AudioClip[] clips;

    void Start()
    {
        src = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        src.clip = clips[Random.Range(0, clips.Length)];
        src.volume = VolumeSettings.GetVolume(AudioType.Effects);
        src.Play();
    }

}
