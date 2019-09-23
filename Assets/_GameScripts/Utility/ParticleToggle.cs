using UnityEngine;
using System.Collections;

public class ParticleToggle : MonoBehaviour {

    public ParticleSystem ps;

    public void TurnOn()
    {
        var em = ps.emission;
        em.enabled = true;
    }

    public void TurnOff()
    {
        var em = ps.emission;
        em.enabled = false;
    }
}
