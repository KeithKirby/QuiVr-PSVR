using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VoiceVolumeCheck : MonoBehaviour {

    public Image Slider;
    public PhotonVoiceRecorder Recorder;
    public bool Running;
    public float CurAvgAmp = 0;
    float lastAvgAmp = 0;

    void Update()
    {
        if(Running)
        {
            if (Recorder != null && Recorder.LevelMeter != null)
                CurAvgAmp = Recorder.LevelMeter.CurrentAvgAmp;
            if(CurAvgAmp != lastAvgAmp)
            {
                lastAvgAmp = CurAvgAmp;
                Slider.fillAmount = CurAvgAmp*75f;
            }
        }
    }

    public void ToggleRunning(bool val)
    {
        Running = val;
    }
}
