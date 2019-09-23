using UnityEngine;
using System.Collections;

public class BowHandle : MonoBehaviour {
    public Transform arrowNockingPoint;
    public BowAim aim;
    public Transform nockSide;

    public SphereCollider HardCol;
    public CapsuleCollider NormalCol;

    public static BowHandle instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetExpertTrigger(Settings.GetBool("expertNock", false));
    }

    public void SetupNockAudio()
    {
        AudioSource src = GetComponent<AudioSource>();
        BowAudio baud = GetComponent<BowAudio>();
        if(src != null && baud != null)
        {
            AudioSource click = arrowNockingPoint.gameObject.AddComponent<AudioSource>();
            click.clip = src.clip;
            click.volume = src.volume;
            click.pitch = src.pitch;
            click.playOnAwake = src.playOnAwake;
            click.loop = src.loop;
            click.minDistance = src.minDistance;
            click.maxDistance = src.maxDistance;
            click.spatialize = true;
            click.spatialBlend = 1;
            baud.src = arrowNockingPoint.GetComponent<AudioSource>();
        }
    }

    public void SetExpertTrigger(bool value)
    {
        HardCol.enabled = value;
        NormalCol.enabled = !value;
    }
}