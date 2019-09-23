using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleAttractor : MonoBehaviour {

    public ParticleSystem pSys;
    public float attraction = 2.0f;
    public bool worldSpaceParticles = false;
    public bool DestroyOnReach;
    public AudioSource DestroyClip;

    private ParticleSystem.Particle[] m_Particles;

    void LateUpdate()
    {
        if (pSys == null)
            return;
        InitializeIfNeeded();
        if (m_Particles.Length < 1)
            return;
        int numParticlesAlive = pSys.GetParticles(m_Particles);
        Vector3 target = transform.position;
        if (!worldSpaceParticles)
            target = target - pSys.transform.position;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            float dist = Vector3.Distance(m_Particles[i].position, target);
            m_Particles[i].position = Vector3.MoveTowards(m_Particles[i].position, target, Time.deltaTime * attraction * Mathf.Clamp(dist, 1f, 100f));
            if (DestroyOnReach && dist < 0.05f)
            {
                m_Particles[i].remainingLifetime = 0;
                if(DestroyClip != null)
                {
                    DestroyClip.pitch = Random.Range(0.9f, 1.1f);
                    DestroyClip.Play();
                }
            }  
        }
        pSys.SetParticles(m_Particles, numParticlesAlive);
    }

    void InitializeIfNeeded()
    {
        if (m_Particles == null || m_Particles.Length < pSys.maxParticles)
            m_Particles = new ParticleSystem.Particle[pSys.maxParticles];
    }
}
