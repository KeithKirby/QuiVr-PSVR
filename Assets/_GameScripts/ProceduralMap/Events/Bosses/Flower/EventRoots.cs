using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRoots : MonoBehaviour {

    [HideInInspector]
    public Teleporter target;
    [HideInInspector]
    public bool active;
    public bool rootsUp;

    RFX4_AudioCurves[] audiocurves;
    RFX4_ShaderFloatCurve[] floats;
    public AudioSource[] Loops;
    public ParticleSystem SpawnParticles;
    public ParticleSystem GetReady;

    void Awake()
    {
        audiocurves = GetComponentsInChildren<RFX4_AudioCurves>();
        floats = GetComponentsInChildren<RFX4_ShaderFloatCurve>();
    }

	public void Activate(Teleporter tp, float delay=0f)
    {
        if(!active)
        {
            StopAllCoroutines();
            active = true;
            rootsUp = false;
            target = tp;
            transform.position = tp.Positions[0].pos.position;
            GetReady.Play();
            StartCoroutine("SpawnRoots", delay);
        }
    }

    void Update()
    {
        if (active && rootsUp && target != null && TelePlayer.instance.currentNode == target && !PlayerLife.dead() )
            PlayerLife.Kill();
    }

    IEnumerator SpawnRoots(float delay)
    {
        float t = 0;
        while(t < delay)
        {
            t += Time.deltaTime;
            yield return true;
            if(target != null && target == TelePlayer.instance.currentNode)
                PlayerShake.Shake(0.5f, 0.5f, false);
        }
        GetReady.Stop();
        SpawnParticles.Play();
        SpawnParticles.GetComponent<AudioSource>().Play();
        foreach (var v in audiocurves) { v.Play(); }
        foreach (var v in floats) { v.Play(); }
        Invoke("Deactivate", 10f);
        yield return new WaitForSeconds(1.5f);
        rootsUp = true;
    }

    public void Deactivate()
    {
        target = null;
        rootsUp = false;
        StopAllCoroutines();
        GetReady.Stop();
        foreach (var v in audiocurves) { v.Reverse(); }
        foreach (var v in floats) { v.Reverse(); }
        Invoke("InactiveState", 3f);
    }

    void InactiveState()
    {
        active = false;
        rootsUp = false;
    }

}
