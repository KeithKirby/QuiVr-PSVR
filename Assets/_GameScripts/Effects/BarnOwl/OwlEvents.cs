using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlEvents : MonoBehaviour {

    public ParticleSystem TPOut;
    public ParticleSystem TPIn;

    public GameObject Shield;

    public void Teleport(Vector3 pos, Quaternion rot)
    {
        if(TPOut != null)
        {
            TPOut.transform.position = transform.position;
            TPOut.Play();
            if (TPOut.GetComponent<AudioSource>() != null)
                TPOut.GetComponent<AudioSource>().Play();
        }
        transform.position = pos;
        transform.rotation = rot;
        if(TPIn != null)
        {
            TPIn.transform.position = transform.position;
            TPIn.Play();
            if (TPIn.GetComponent<AudioSource>() != null)
                TPIn.GetComponent<AudioSource>().Play();
        }
    }

    public void RespawnEffect()
    {
        TPIn.transform.position = transform.position;
        TPIn.Play();
        if (TPIn.GetComponent<AudioSource>() != null)
            TPIn.GetComponent<AudioSource>().Play();
    }

    public void ToggleInvincible(bool invincible)
    {
        GetComponent<ArrowImpact>().Inactive = invincible;
        if (Shield != null)
            Shield.SetActive(invincible);
    }
}
