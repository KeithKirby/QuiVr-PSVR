using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolume : MonoBehaviour {

    public AudioType Type;
    AudioSource src;
    float startVol;
    float curTypeVolume;
    public bool VolumeWithScale;
    [AdvancedInspector.Inspect("ChangeWithScale")]
    public float FullScale;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        VolumeSettings.AddAudio(this);
        if (src != null)
        {
            startVol = src.volume;
            curTypeVolume = VolumeSettings.GetVolume(Type);
            src.volume = startVol * curTypeVolume;
        }
    }

    public void OnChangedVolume(float val, AudioType type)
    {
        if(src != null && type == Type)
        {
            src.volume = startVol * val;
        }
    }

    void Update()
    {
        if(VolumeWithScale)
        {
            src.volume = startVol * curTypeVolume * (transform.localScale.x/FullScale);
        }
    }

    void OnDestroy()
    {
        VolumeSettings.RemoveAudio(this);
    }

    public bool ChangeWithScale()
    {
        return VolumeWithScale;
    }
}

public enum AudioType
{
    Effects,
    Ambient,
    Player,
    Master,
    Music
}
