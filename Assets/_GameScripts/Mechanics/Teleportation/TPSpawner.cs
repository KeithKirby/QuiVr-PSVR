using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TPSpawner : Effect {

    public GameObject FizzleEffect;
    public Transform TPPos;
    public Transform[] PlayerPoints;

    public AudioClip[] DespawnClips;
    public float DespawnClipDelay;

    public int TPID = -1;

    IEnumerator Start()
    {
        yield return true;
        if(TeleporterManager.instance == null)
        {
            if (FizzleEffect != null)
            {
                GameObject o = Instantiate(FizzleEffect, transform.position, Quaternion.identity);
                o.SetActive(true);
            }    
            Destroy(gameObject);
        }
        else if(!dummy)
        {
            //Create teleporter
            List<Vector3> pts = new List<Vector3>();
            List<Quaternion> rts = new List<Quaternion>();
            foreach(var v in PlayerPoints)
            {
                pts.Add(v.position);
                rts.Add(v.rotation);
            }
            TPID = TeleporterManager.instance.CreateTP(TPPos.position, false, pts.ToArray(), rts.ToArray(), true);
        }
        float dur = baseEffect.StaticValue;
        if (baseEffect.randomType == RandomType.Duration)
        {
            dur = baseEffect.VariableValue + val;
        }
        dur = Mathf.Clamp(dur, 2, 100);
        Invoke("DestroyClip", dur - DespawnClipDelay);
        Invoke("DoDestroy", dur);
    }

    void DestroyClip()
    {
        if(DespawnClips.Length > 0)
        {
            VRAudio.PlayClipAtPoint(DespawnClips[Random.Range(0, DespawnClips.Length)], transform.position, 1f* VolumeSettings.GetVolume(AudioType.Effects), 1, 1, 1.5f, 8);
        }
    }

    void DoDestroy()
    {
        if (FizzleEffect != null)
        {
            GameObject o = Instantiate(FizzleEffect, transform.position, Quaternion.identity);
            o.SetActive(true);
        }
        if (TeleporterManager.instance != null && !dummy)
            TeleporterManager.instance.DestroyTP(TPID);
        Destroy(gameObject);
    }
}
