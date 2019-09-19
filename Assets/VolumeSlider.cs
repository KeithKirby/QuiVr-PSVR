using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSlider : MonoBehaviour {

    public AudioType Type;

    public void SetValue(float f)
    {
        if (Type != AudioType.Master)
            VolumeSettings.ChangeVolume(Type, f);
        else
            AudioListener.volume = f;
    }
}
