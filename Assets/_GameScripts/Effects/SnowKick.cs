using UnityEngine;
using System.Collections;

public class SnowKick : MonoBehaviour {

    public ParticleSystem[] SnowEffect;
    public bool IgnoreRotation;

	public void Collision(ArrowCollision col)
    {
        if(Settings.GetInt("EffectQuality") >= 1  && SnowEffect.Length > 0)
        {
            foreach (var v in SnowEffect)
            {
                v.transform.position = col.impactPos;
                if(!IgnoreRotation)
                    v.transform.LookAt(col.impactPos + Vector3.Reflect(col.ImpactNormal, Vector3.down)*-1f);
                v.Play();
              
                //Corresponding Audio  
                PlayRandomClip prc = v.GetComponent<PlayRandomClip>();
                if(prc != null)
                    prc.Play();
                else
                {
                    AudioSource src = v.GetComponent<AudioSource>();
                    if (src != null)
                        src.Play();
                }
            }
        }
    }
}
