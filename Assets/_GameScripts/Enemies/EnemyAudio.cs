using UnityEngine;
using System.Collections;

public class EnemyAudio : MonoBehaviour {

    public AudioClip[] Hit;
    public AudioClip[] Death;
    public AudioClip[] Spawn;
    public AudioClip[] Attack;
    public AudioClip[] Footsteps;
    public AudioClip[] FootstepsHeavy;
    public AudioClip[] FootstepsLight;
    AudioSource mySource;

    const float maxDistance = 500;

    void Awake()
    {
        mySource = GetComponent<AudioSource>();
        if(Footsteps.Length > 0 && mySource == null)
        {
            mySource = gameObject.AddComponent<AudioSource>();
            mySource.spatialBlend = 1;
            //mySource.minDistance = 3f;
        }
    }

    public void PlayRandomClip(AudioClip[] clips, float thd)
    {
        if (clips.Length > 0)
        {
            int i = Random.Range(0, clips.Length);
            VRAudio.PlayClipAtPoint(clips[i], transform.position, 1* VolumeSettings.GetVolume(AudioType.Effects), Random.Range(0.8f, 1.2f), thd);
        }
    }

    public void PlayRandomClip(AudioClip[] clips, float thd, float minDist)
    {
        if (clips.Length > 0)
        {
            int i = Random.Range(0, clips.Length);
            VRAudio.PlayClipAtPoint(
                clips[i],
                transform.position,
                1* VolumeSettings.GetVolume(AudioType.Effects),
                Random.Range(0.8f, 1.2f),
                thd,
                minDist,
                maxDistance, 
                AudioRolloffMode.Linear
            );
        }
    }

    public void PlayRandomClip(AudioClip[] clips, float thd, float minDist, float pitch)
    {
        if (clips.Length > 0)
        {
            int i = Random.Range(0, clips.Length);
            VRAudio.PlayClipAtPoint(
                clips[i],
                transform.position, 
                1 * VolumeSettings.GetVolume(AudioType.Effects),
                pitch, 
                thd,
                minDist,
                maxDistance, 
                AudioRolloffMode.Linear
            );
        }
    }

    public void PlayDeath()
    {
        PlayRandomClip(Death, 1f, 1);
    }

    public void PlayHit()
    {
        PlayRandomClip(Hit, 1f);
    }

    public void PlaySpawn()
    {
        PlayRandomClip(Spawn, .98f);
    }

    public void PlayAttack()
    {
        PlayRandomClip(Attack, .995f, 5f);
    }

    public void Footstep(int val=0)
    {
        AudioClip[] fs = Footsteps;
        if (val == -1)
            fs = FootstepsLight;
        else if (val == 1)
            fs = FootstepsHeavy;
        if(fs.Length > 0 && mySource != null)
        {
            mySource.clip = fs[Random.Range(0, fs.Length)];
            mySource.pitch = Random.Range(0.8f, 1.2f);
            mySource.volume = VolumeSettings.GetVolume(AudioType.Effects);
            mySource.Play();
        }
    }

    public void ToggleMovementSound(bool on, float speedMult=1)
    {
        if(mySource != null)
        {
            if(on && !mySource.isPlaying)
            {
                mySource.volume = 0.7f;
            }
            else if(!on && mySource.isPlaying)
            {
                mySource.volume = 0;
            }           
        }
    }
}
