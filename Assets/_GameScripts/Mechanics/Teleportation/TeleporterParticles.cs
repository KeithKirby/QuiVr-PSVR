using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterParticles : MonoBehaviour {

    Chestplate chest;
    MeshRenderer curChest;
    public ParticleOption[] Particles;
    public bool On;

	void Awake()
    {
        InvokeRepeating("CheckArmor", 0.5f, 1.25f);
    }

    void CheckArmor()
    {
        if(chest == null)
            chest = GetComponentInChildren<Chestplate>();
        if (chest != null && curChest == null)
        {
            curChest = chest.Mesh;
        }
    }

    public void ParticlesOn()
    {
        int i = Settings.GetInt("EffectQuality");
        if(curChest != null)
        {
            foreach(var v in Particles)
            {
                if(i >= v.QualitySetting)
                {
                    ParticleSystem.EmissionModule em = v.p.emission;
                    em.enabled = true;
                    On = true;
                }
            }
        }
    }

    public void ParticlesOff()
    {
        if(gameObject.activeInHierarchy)
            StartCoroutine("ShutOff");
        else
        {
            foreach (var v in Particles)
            {
                ParticleSystem.EmissionModule em = v.p.emission;
                em.enabled = false;
            }
        }
        On = false;
    }

    IEnumerator ShutOff()
    {
        yield return true;
        yield return true;
        foreach (var v in Particles)
        {
            ParticleSystem.EmissionModule em = v.p.emission;
            em.enabled = false;
        }
    }

    [System.Serializable]
    public class ParticleOption
    {
        public ParticleSystem p;
        public int QualitySetting;
    }
}
