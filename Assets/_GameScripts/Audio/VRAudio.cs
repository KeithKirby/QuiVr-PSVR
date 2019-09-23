using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRAudio : MonoBehaviour {

    public int NumSources = 10;
    AudioSource[] Pool;
    [HideInInspector]
    public static VRAudio instance;
    int cureIndex;
    
    void Awake()
    {
        instance = this;
        Pool = new AudioSource[NumSources];
        for(int i=0; i< NumSources; i++)
        {
            GameObject o = new GameObject("VRAudioSource_"+i);
            o.transform.SetParent(transform);
            Pool[i] = o.AddComponent<AudioSource>();
        }
    }

    public AudioSource GetNext(Vector3 location)
    {
        cureIndex++;
        if (cureIndex >= Pool.Length)
            cureIndex = 0;
        Pool[cureIndex].time = 0;
        Pool[cureIndex].transform.position = location;
        return Pool[cureIndex];
    }

	public static void PlayClipAtPoint(AudioClip clip, Vector3 pos, float vol)
    {
        PlayClipAtPoint(clip, pos, vol, Random.Range(0.9f, 1.1f), 1, 1);       
    }

    public static void PlayClipAtPoint(AudioClip clip, Vector3 pos, float vol, float pitch)
    {
        PlayClipAtPoint(clip, pos, vol, Mathf.Clamp(pitch, 0.01f, 3), 1, 1);
    }

    public static void PlayClipAtPoint(AudioClip clip, Vector3 pos, float vol, float pitch, float ThreeDPerc)
    {
        PlayClipAtPoint(clip, pos, vol, Mathf.Clamp(pitch, 0.01f, 3), ThreeDPerc, 1);
    }

    public static void PlayClipAtPoint(AudioClip clip, Vector3 pos, float vol, float pitch, float ThreeDPerc, float minRange, float maxRange=500, AudioRolloffMode mode = AudioRolloffMode.Logarithmic)
    {
        if (clip == null)
            return;
        AudioSource aSource;
        if (instance == null)
        {
            GameObject tempGO = new GameObject(clip.name);
            tempGO.transform.position = pos;
            aSource = tempGO.AddComponent<AudioSource>();
        }
        else
        {
            aSource = instance.GetNext(pos);
        }
        aSource.spatialize = CloseToHead(pos);
        aSource.minDistance = minRange;
        aSource.maxDistance = maxRange;
        aSource.spatialBlend = ThreeDPerc;
        aSource.rolloffMode = mode;
        aSource.pitch = Mathf.Clamp(pitch, 0.01f, 3);
        aSource.volume = vol;
        aSource.clip = clip;
        aSource.Play();
    }

    static bool CloseToHead(Vector3 pos)
    {
        if(PlayerHead.instance != null)
        {
            if (Vector3.Distance(pos, PlayerHead.instance.transform.position) < 15f)
                return true;
        }
        return false;
    }
}
