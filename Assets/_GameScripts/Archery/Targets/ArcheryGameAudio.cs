using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class ArcheryGameAudio : MonoBehaviour {

    public static ArcheryGameAudio instance;

    public AudioClip NewGame;
    public AudioClip EndGame;
    public AudioClip NextWave;
    public AudioClip Tick;
    public AudioClip EndWave;

    AudioSource src;
    void Awake()
    {
        instance = this;
        src = GetComponent<AudioSource>();
    }

    public void NewGameSequence()
    {
        StopCoroutine("NGSeq");
        StartCoroutine("NGSeq");
    }

    IEnumerator NGSeq()
    {
        for(int i=0; i<4; i++)
        {
            src.clip = Tick;
            src.Play();
            yield return new WaitForSeconds(1);
        }
        StopCoroutine("NGSeq");
    }

    public void EndWaveAudio()
    {
        src.clip = EndWave;
        src.Play();
    }

    public void NewGameAudio()
    {
        src.clip = NewGame;
        src.Play();
    }

    public void EndGameAudio()
    {
        src.clip = EndGame;
        src.Play();
    }

    public void NextWaveAudio()
    {
        src.clip = NextWave;
        src.Play();
    }
}
