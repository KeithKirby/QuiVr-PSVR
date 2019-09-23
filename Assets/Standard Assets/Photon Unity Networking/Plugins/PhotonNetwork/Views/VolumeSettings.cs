using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSettings : MonoBehaviour {

    static List<AudioVolume> vols;
    static Dictionary<string, float> volumeVals;
    static string[] types =
    {
        "Effects",
        "Ambient",
        "Player",
        "Master",
        "Music"
    };

    public static void AddAudio(AudioVolume vol)
    {
        if (vols == null)
            vols = new List<AudioVolume>();
        vols.Add(vol);
    }

    public static void RemoveAudio(AudioVolume vol)
    {
        if (vols != null)
            vols.Remove(vol);
    }

    public static void ChangeVolume(AudioType type, float vol)
    {
        if (volumeVals == null)
            volumeVals = new Dictionary<string, float>();
        volumeVals[types[(int)type]] = vol;
        if (vols == null)
            vols = new List<AudioVolume>();
        foreach (var v in vols)
        {
            v.OnChangedVolume(vol, type);
        }
    }

    public static float GetVolume(AudioType type)
    {
        if (volumeVals != null && volumeVals.ContainsKey(types[(int)type]))
            return volumeVals[types[(int)type]];
        return 1;
    }
}
