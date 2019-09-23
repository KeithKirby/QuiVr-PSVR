using UnityEngine;
using System.Collections;

public class ArrowAudio : MonoBehaviour {

    public AudioClip[] FireClips;
    public PhysicMaterial[] mats;

    public AudioClip[] DeflectClips;

    public AudioClip[] Wood;
    public AudioClip[] Stone;
    public AudioClip[] Metal;
    public AudioClip[] Ground;
    public AudioClip[] Flesh;
    public AudioClip[] HardFlesh;
    public AudioClip[] Bone;

    AudioSource src;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        src = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
	}

    public void Init()
    {
        Start();
    }

    public int isHard(Collider col)
    {
        if (col == null)
            return 1;
        if (col.sharedMaterial == null)
            return 1;
        if (col.sharedMaterial == mats[1] || col.sharedMaterial == mats[2])
            return 2;
        else if (col.sharedMaterial == mats[4] || col.sharedMaterial == mats[3] || col.sharedMaterial == mats[5])
            return 0;
        return 1;
    }
	
	// Update is called once per frame
	void Update () {
        if (rb == null || rb.isKinematic || rb.velocity.magnitude < 10f)
            src.volume = 0;
        else
        {
            src.volume = (rb.velocity.magnitude-10f)/25f;
            src.pitch = Mathf.Clamp((rb.velocity.magnitude-10f) / 15f, 0f, 1f);
        }
	}

    public void Deflect(Collider col)
    {
        float vol = rb.velocity.magnitude / 15f;
        PlayRandom(DeflectClips, vol);
    }

    public void PlayImpact(Collider col)
    {
        if(col == null)
        {
            Debug.Log("Hit object had no collider?");
            PlayRandom(Wood, rb.velocity.magnitude / 20f);
            return;
        }
        float vol = rb.velocity.magnitude/20f;
        if (col.sharedMaterial == mats[0])
            PlayRandom(Wood, vol);
        else if (col.sharedMaterial == mats[1])
            PlayRandom(Stone, vol);
        else if (col.sharedMaterial == mats[2])
            PlayRandom(Metal, vol);
        else if (col.sharedMaterial == mats[3])
            PlayRandom(Ground, vol);
        else if (col.sharedMaterial == mats[4])
            PlayRandom(Flesh, vol);
        else if (col.sharedMaterial == mats[5])
            PlayRandom(HardFlesh, vol);
        else if (col.sharedMaterial == mats[6])
            PlayRandom(Bone, vol);
        else if (col.sharedMaterial == mats[7])
            return; //No Sound
        else
        {
            PlayRandom(Wood, rb.velocity.magnitude / 20f);
        }      
    }

    void PlayRandom(AudioClip[] clips, float volume)
    {
        VRAudio.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], transform.position, volume, Random.Range(0.8f, 1.2f), 0.98f);
    }

    public void Play()
    {
        src.clip = FireClips[Random.Range(0, FireClips.Length)];
        src.Play();
    }
}
