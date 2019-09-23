using UnityEngine;
using System.Collections;

public class GameplayAudio : MonoBehaviour {

    public AudioClip[] HardWaves;
    public AudioClip[] LoseClips;
    public AudioClip[] WinClips;
    public AudioClip[] Horns;
    public AudioClip[] CBreakClips;

    public static GameplayAudio instance;

    void Awake()
    {
        instance = this;
    }

    public static void PlayWin()
    {
        if (instance != null && instance.WinClips.Length > 0)
        {
            Play2D(instance.WinClips[Random.Range(0, instance.WinClips.Length)], 0.8f);
        }
    }

    public static void PlayHardWave()
    {
        if(instance != null && instance.HardWaves.Length > 0)
        {
            Play2D(instance.HardWaves[Random.Range(0, instance.HardWaves.Length)]);
        }
    }

    public static void ComboBreak()
    {
        if(instance != null && instance.CBreakClips.Length > 0)
        {
            Play2D(instance.CBreakClips[Random.Range(0, instance.CBreakClips.Length)], 0.65f * VolumeSettings.GetVolume(AudioType.Effects));
        }
    }

    public static void PlayHorn()
    {
        if (instance != null && instance.Horns.Length > 0)
        {
            Play2D(instance.Horns[Random.Range(0, instance.Horns.Length)]);
        }
    }

    public static void PlayLose()
    {
        if (instance != null && instance.LoseClips.Length > 0)
        {
            Play2D(instance.LoseClips[Random.Range(0, instance.LoseClips.Length)]);
        }
    }

    static void Play2D(AudioClip clip, float volume=1f)
    {
        VRAudio.PlayClipAtPoint(clip, Vector3.zero, volume, 1, 0);
    }

    static void Play3D(AudioClip clip, Vector3 position, float volume = 1f)
    {
        VRAudio.PlayClipAtPoint(clip, position, volume, 1f, 0.99f, 15f);
    }
}
