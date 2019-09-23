using UnityEngine;
using System.Collections;

public class OwlSounds : MonoBehaviour {

    public AudioSource src;

    public AudioClip[] Screeches;
    public AudioClip[] RespawnsRegular;
    public AudioClip[] RespawnsAnnoyed;
    public AudioClip[] RespawnsAngry;

    public OwlHead head;

    void Update()
    {
        if(src != null && src.isPlaying)
        {
            float[] samples = new float[64];
            src.GetOutputData(samples, 0);
            float average = 0;
            foreach (var v in samples)
                average += Mathf.Abs(v);
            average /= samples.Length;
            if (average > 0.0002f)
                head.isTalking = true;
            else
                head.isTalking = false;
        }
    }

    public bool isPlaying()
    {
        return src.isPlaying;
    }

	public void PlayScreech()
    {
        src.volume = 0.9f;
        src.clip = Screeches[Random.Range(0, Screeches.Length)];
        src.time = 0;
        src.Play();
    }

    public void PlayRespawn(int anger)
    {
        int i = Random.Range(0, RespawnsRegular.Length);
        AudioClip c = RespawnsRegular[i];
        if (anger == 1)
        {
            i = Random.Range(0, RespawnsAnnoyed.Length);
            c = RespawnsAnnoyed[i];
        }
        else if(anger == 2)
        {
            i = Random.Range(0, RespawnsAngry.Length);
            c = RespawnsAngry[i];
        }
        src.volume = 0.7f;
        src.clip = c;
        src.time = 0;
        src.Play();
    }

    public void StopAudio()
    {
        src.Stop();
    }

    public void PlayAudioclip(AudioClip clip, float volume=1)
    {
        src.volume = 1;
        src.clip = clip;
        src.time = 0;
        src.Play();
    }
}
