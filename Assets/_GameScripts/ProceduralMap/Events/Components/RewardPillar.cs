using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPillar : MonoBehaviour {

    public static RewardPillar instance;
    public Transform PillarHolder;
    public AnimationCurve HeightCurve;
    public float RaiseTime;
    public ParticleSystem ReadyDisplay;
    public ParticleSystem ReadyExplode;
    public ItemOrbDisplay Display;
    public RFX4_ShaderColorGradient OverFade;

    public ParticleSystem DuringRaiseEffect;
    public ParticleSystem OnRaiseEffect;
    public AudioClip RaiseClip;
    public AudioClip RaiseComplete;
    AudioSource src;
    void Awake()
    {
        instance = this;
        newItem = null;
        src = GetComponent<AudioSource>();
    }

    [AdvancedInspector.Inspect]
    void TestRaise()
    {
        Vector3 pos = transform.position;
        Reset();
        EnterSequence(pos, transform.rotation, false);
    }

    #region Statics

    public static void Reset()
    {
        if(instance != null)
        {
            instance.DoReset();
        }
    }

    public static void EnterSequence(Vector3 pos, Quaternion rotation, bool giveItem=true)
    {
        if(instance != null)
        {
            instance.startPos = pos;
            instance.transform.rotation = rotation;
            instance.EnterSeq(giveItem);
        }
    }

    #endregion

    void DoReset()
    {
        transform.position = Vector3.zero;
        Display.gameObject.SetActive(false);
        GaveReward = false;
        ReadyDisplay.Stop();
        ReadyDisplay.GetComponent<Collider>().enabled = false;
        ReadyDisplay.GetComponent<AudioSource>().Stop();
        newItem = null;
        moteReward = 0;
        noReward = false;
        DuringRaiseEffect.Stop();
    }

    Vector3 startPos;
    ArmorOption newItem;
    int moteReward = 0;
    bool noReward;
    void EnterSeq(bool giveItem=true)
    {
        transform.position = startPos;
        src.time = 0;
        src.clip = RaiseClip;
        src.loop = true;
        src.Play();
        StopAllCoroutines();
        StartCoroutine("Raise");
        OverFade.Play();
        DuringRaiseEffect.Play();
        //Item 
        if (giveItem && GameBase.CanGetReward())
        {
            if (ItemGenerator.ItemOnLoss(GameBase.instance.Difficulty))
            {
                int r = ItemGenerator.GetRatity(GameBase.instance.Difficulty);
                var itemGen = GameObject.FindObjectOfType<ItemGenerator>();
                itemGen.GetRandomItem(r, ItemGenerator.Encrypt("" + r), SetItem);
            }
            else
            {
                moteReward = ItemGenerator.GetResource(GameBase.instance.Difficulty);
                Armory.instance.GiveResource(moteReward, false);
            }
        } else if(!GameBase.CanGetReward())
        {
            noReward = true;
        }
    }

    bool duplicate;
    void SetItem(ArmorOption item)
    {
        newItem = item;
        duplicate = Armory.instance.HasDuplicate(item);
        Armory.instance.AddItem(item, true, false);
        Display.Setup(item);
    }

    IEnumerator Raise()
    {
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime / RaiseTime;
            PillarHolder.localPosition = Vector3.up * HeightCurve.Evaluate(t);
            yield return true;
        }
        OnRaiseEnd();
    }

    void OnRaiseEnd()
    {
        DuringRaiseEffect.Stop();
        ReadyDisplay.Play();
        ReadyDisplay.GetComponent<Collider>().enabled = true;
        ReadyDisplay.GetComponent<FadeAudio>().FadeIn();
        OverFade.Reverse();
        src.time = 0;
        src.clip = RaiseComplete;
        src.loop = false;
        src.Play();
        OnRaiseEffect.Play();
    }

    void DoExplode()
    {
        ReadyDisplay.Stop();
        ReadyDisplay.GetComponent<Collider>().enabled = false;
        ReadyDisplay.GetComponent<AudioSource>().Stop();

        ReadyExplode.Play();
        ReadyExplode.GetComponentInChildren<AudioSource>().Play();
    }

    bool GaveReward;
    [AdvancedInspector.Inspect]
    public void TryGiveReward()
    {
        if(!GaveReward)
        {
            GaveReward = true;
            DoExplode();
            if(noReward)
            {
                Debug.Log("No Reward Given: Cheats used");
            }
            else if(moteReward > 0)
            {
                Notification.Notify(Note.GainResource(moteReward));
                if (MoteParticles.instance != null)
                    MoteParticles.instance.SpawnMotes(ReadyDisplay.transform.position, moteReward);
            }
            else if(newItem != null)
            {
                if(duplicate)
                {
                    int diffAmt = 0;
                    if (GameBase.instance != null && GameBase.instance.Difficulty > 5)
                        diffAmt = ItemGenerator.GetResource(GameBase.instance.Difficulty);
                    int value = ItemDatabase.v.ResourceValues[newItem.rarity];
                    value += diffAmt;
                    Notification.Notify(Note.DuplicateResource(value, newItem));
                    if (MoteParticles.instance != null)
                        MoteParticles.instance.SpawnMotes(value);
                }
                else
                {
                    Notification.Notify(new Note(newItem));
                    Display.gameObject.SetActive(true);
                }
            }
        }
    }
}
