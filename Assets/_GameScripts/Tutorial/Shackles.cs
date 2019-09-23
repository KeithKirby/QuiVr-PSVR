using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class Shackles : MonoBehaviour {

    public AudioClip[] StruggleClips;
    float t;
    float nextCD;
    AudioSource src;
    Health h;
    public int ropes;
    public int broken;
    bool CreatureReleased;

    public UnityEvent OnRelease;
    public GameObject EnemyTParticles;
    public Transform CreatureHips;
    public AudioClip Mutter;

    public void BreakRope()
    {
        Debug.Log("Rope Broken");
        broken = broken + 1;
    }

    void Awake()
    {
        src = GetComponent<AudioSource>();
        h = GetComponentInChildren<Health>();
    }

    void Update()
    {
        t += Time.deltaTime;
        if(t >= nextCD && !CreatureReleased)
        {
            t = 0;
            nextCD = Random.Range(0.8f, 2.3f);
            PlayClip();
        }
        if(broken >= ropes && !CreatureReleased)
        {
            CreatureReleased = true;
            OnRelease.Invoke();
            src.clip = Mutter;
            src.Play();
            Invoke("TPCreature", 0.5f);
        }
    }

    void TPCreature()
    {
        GameObject g = (GameObject)Instantiate(EnemyTParticles, CreatureHips.position, Quaternion.identity);
        h.gameObject.SetActive(false);
    }

    void PlayClip()
    {
        if(!h.isDead())
        {
            src.clip = StruggleClips[Random.Range(0, StruggleClips.Length)];
            src.Play();
            foreach(var rb in h.GetComponentsInChildren<Rigidbody>())
            {
                if(!rb.isKinematic)
                {
                    Vector3 v = new Vector3(Random.Range(-0.5f, .5f), Random.Range(-0.5f, .5f), Random.Range(-0.5f, .5f));
                    rb.AddExplosionForce(Random.Range(100, 1000), rb.transform.position + v, 3f);
                }
            }
        }
    }
}
