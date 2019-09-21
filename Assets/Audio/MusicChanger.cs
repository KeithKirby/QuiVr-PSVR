using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : MonoBehaviour {

    public static MusicChanger instance;
    public AudioSource SourceA;
    public AudioSource SourceB;
    public float FadeTime;
    public float CrossThreshold;
    [HideInInspector]
    public string CurClip;

    [Header("Debug Info")]
    public AudioSource CurSrc;
    public AudioSource ChangeSrc;
    public bool paused;
    public float WantVol;
    public float setting = 0.8f;
    public float volMult;

    void Awake()
    {
        instance = this;
        InvokeRepeating("CheckVolume", 1f, 1f);
    }

    void CheckVolume()
    {
        if (Settings.HasKey("MusicVolume"))
            setting = Settings.GetFloat("MusicVolume");
        if (SourceA.volume > setting)
            SourceA.volume = setting;
        if (SourceB.volume > setting)
            SourceB.volume = setting;
    }

    public static void PlayClip(string clip, float MaxVol = 1)
    {
        if(instance !=  null)
        {
            instance.LoadAndPlay(clip);
            instance.WantVol = MaxVol;
        }
    }

    public void SetDynamicMusic(bool on)
    {
        MusicScheduler2.instance.volume = on ? 1 : 0;
        volMult = on ? 0 : 1;
    }

    public void FadeOutAudio(float dur)
    {
        if(CurSrc != null)
        {
            paused = true;
            StopAllCoroutines();
            StartCoroutine("FadeOut", dur);
        }
    }

    public void FadeInAudio(float dur)
    {
        if(CurSrc != null)
        {
            paused = false;
            StopAllCoroutines();
            StartCoroutine("FadeIn", dur);
        }
    }

    IEnumerator FadeOut(float dur)
    {    
        while (CurSrc.volume > 0 || ChangeSrc.volume > 0)
        {
            CurSrc.volume -= Time.deltaTime / dur;
            ChangeSrc.volume -= Time.deltaTime / dur;
            yield return true;
        }
    }

    IEnumerator FadeIn(float dur)
    {
        while (ChangeSrc.volume < setting * WantVol * volMult)
        {
            ChangeSrc.volume += Time.deltaTime / dur;
            yield return true;
        }
    }

    public void LoadAndPlay(string clip)
    {
        if(clip != CurClip && !paused)
        {
            CurClip = clip;
            StopAllCoroutines();
            StartCoroutine("LoadAndFade", clip);
        }
    }

    IEnumerator LoadAndFade(string clip)
    {
        ResourceRequest r = Resources.LoadAsync(clip, typeof(AudioClip));
        while(!r.isDone)
            yield return true;
        AudioClip newClip = (AudioClip)r.asset;
        if(CurSrc == SourceB)
        {
            CurSrc = SourceA;
            ChangeSrc = SourceB;
        }
        else
        {
            CurSrc = SourceB;
            ChangeSrc = SourceA;
        }
        ChangeSrc.clip = newClip;
        ChangeSrc.volume = 0;
        ChangeSrc.Play();
        while(CurSrc.volume > 0 && !paused)
        {
            CurSrc.volume -= Time.deltaTime / FadeTime;
            if (CurSrc.volume <= CrossThreshold && ChangeSrc.volume < WantVol*setting)
                ChangeSrc.volume += Time.deltaTime / FadeTime;
            yield return true;
        }
        CurSrc.Stop();
        while ( ChangeSrc.volume < WantVol*setting && !paused)
        {
            ChangeSrc.volume += Time.deltaTime / FadeTime;
            yield return true;
        }
    }

}
