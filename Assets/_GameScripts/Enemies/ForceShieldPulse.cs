using UnityEngine;
using System.Collections;

public class ForceShieldPulse : MonoBehaviour {

    public GameObject PulsePrefab;
    public AudioClip[] PulseClips;

    public void Pulse(ArrowCollision e)
    {
        GameObject pref = (GameObject)Instantiate(PulsePrefab, transform);
        pref.transform.localPosition = Vector3.zero;
        pref.transform.localEulerAngles = Vector3.zero;
        pref.transform.localScale = Vector3.one;
        pref.transform.LookAt(e.impactPos);
        if(PulseClips.Length > 0)
            VRAudio.PlayClipAtPoint(PulseClips[Random.Range(0, PulseClips.Length)], e.impactPos, .75f, 1, 1, 5f);
    }
}
