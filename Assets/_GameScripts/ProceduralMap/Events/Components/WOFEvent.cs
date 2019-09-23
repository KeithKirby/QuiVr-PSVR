using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WOFEvent : EventTile {

    [Header("Gate Damage")]
    public GameObject LaserPrefab;
    public GameObject LaserImpact;

    [Header("Wheel")]
    public AnimationCurve[] Curves;
    public Transform wheel;
    public AudioSource WheelClick;
    public PlayRandomClip SpinStart;
    public AudioClip[] RewardClips;
    public AudioSource RewardSound;
    
    bool requestEnded;
    bool requestSpin;
    int spinPlayerID; 

    public override void Start()
    {
        base.Start();
        DeactivateSkip();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void StartIntro()
    {
        if (!startedIntro)
        {
            StartCoroutine("Intro");
            //Setup Difficulty based on Game Difficulty
            if (TargetGate != null)
            {
                float gateHP = TargetGate.GetComponent<Health>().maxHP;
                int NumAllowed = 2;
                if (GameBase.instance.Difficulty > 1000)
                    NumAllowed = 3;
                else if (GameBase.instance.Difficulty >= 1500)
                    NumAllowed = 4;
                else if (GameBase.instance.Difficulty >= 2000)
                    NumAllowed = 5;
                else if (GameBase.instance.Difficulty >= 3000)
                    NumAllowed = 6;
                PlayerDeathDamage = (int)(gateHP / (float)NumAllowed)+1;
            }
            base.StartIntro();
        }
    }

    IEnumerator Intro()
    {
        yield return true;
        StartEvent();
    }

    public override void StartEvent()
    {
        Debug.Log("Starting WOF Event");
        StartCoroutine("EventLoop");
        base.StartEvent();
    }

    public override void EndEvent()
    {
        base.EndEvent();
    }

    IEnumerator EventLoop()
    {
        yield return EventLeadup();
        requestSpin = false;
        while (!requestEnded)
        {
            if(requestSpin)
            {
                DeactivateSkip();
                yield return DoSpin(GetRandomNext(0, 120));
                yield return new WaitForSeconds(0.5f);
                ActivateSkip();
                yield return new WaitForSeconds(0.5f);
                requestSpin = false;
            }
            yield return true;
        }
        yield return PlayRewardSound(4);
        yield return new WaitForSeconds(1f);
        if (!TargetGate.destroyed)
        {
            EndEvent();
        }
    }

    IEnumerator EventLeadup()
    {
        TryStartMusic();
        //Bring out wheel
        yield return true;
    }

    [AdvancedInspector.Inspect]
    public void TestSpin()
    {
        if(!spinning)
            StartCoroutine("DoSpin", Random.Range(0, 120));
    }

    float lastAngle;
    bool spinning;
    IEnumerator DoSpin(int valueID)
    {
        SpinStart.Play();
        spinning = true;
        int numItems = 120;
        float timeToSpin = 15f;
        float maxAngle = (360 * Random.Range(10, 15)) + (valueID * (360f/numItems));
        float timer = 0.0f;
        float startAngle = wheel.localEulerAngles.z;
        lastAngle = startAngle;
        maxAngle = maxAngle - startAngle;
        AnimationCurve c = Curves[Random.Range(0, Curves.Length)];
        while (timer < timeToSpin)
        {
            //to calculate rotation
            float angle = maxAngle * c.Evaluate(timer/timeToSpin);
            if(angle - lastAngle > 9f)
            {
                lastAngle = angle;
                if(!WheelClick.isPlaying)
                {
                    WheelClick.pitch = Random.Range(0.9f, 1.1f);
                    WheelClick.Play();
                }
            }
            wheel.localEulerAngles = new Vector3(0, 180, angle + startAngle);
            timer += Time.deltaTime;
            yield return true;
        }
        wheel.localEulerAngles = new Vector3(0, 180, maxAngle + startAngle);
        spinning = false;
        yield return new WaitForSeconds(0.5f);
        yield return WheelReward(valueID);
    }

    IEnumerator WheelReward(int id)
    {
        if (id == 119 || id == 117)
        {
            yield return PlayRewardSound(3);
            DamageGate(wheel.transform.position, 999);
        }
        else if(id == 118 && GameBase.CanGetReward())
        {
            Debug.Log("WOF Reward: Wings");
            if (GameBase.CanGetReward())
            {
                yield return PlayRewardSound(2);
                Notification.Notify("Sorry", "Final reward not set up");
            }
        }
        else if(id < 4)
        {
            Debug.Log("WOF Reward: Epic Item");
            if (GameBase.CanGetReward())
            {
                yield return PlayRewardSound(2);
                Notification.Notify("Sorry", "Epic Item reward not set up");
            }
        }
        else if(id < 19)
        {
            yield return PlayRewardSound(1);
            yield return TryKillPlayer();
            Debug.Log("WOF Reward: Player Death");
        }
        else if(id < 30)
        {
            Debug.Log("WOF Reward: Fragments");
            if (GameBase.CanGetReward())
            {
                yield return PlayRewardSound(0);
                yield return GrantResource();
            }
        }
        else if(id < 43)
        {
            //Kill Player
            Debug.Log("WOF Reward: Player Death");
            yield return PlayRewardSound(1);
            yield return TryKillPlayer();
        }
        else if(id < 55)
        {
            //Random Item (Not Legendary)
            Debug.Log("WOF Reward: Random Item");
            if (GameBase.CanGetReward())
            {
                yield return PlayRewardSound(0);
                Notification.Notify("Sorry", "Item reward not set up");
            }
        }
        else if (id < 67)
        {
            //Kill Player
            Debug.Log("WOF Reward: Player Death");
            yield return PlayRewardSound(1);
            yield return TryKillPlayer();
        }
        else if (id < 79)
        {
            //Fragments
            Debug.Log("WOF Reward: Fragments");
            if (GameBase.CanGetReward())
            {
                yield return PlayRewardSound(0);
                yield return GrantResource();
            }
        }
        else if (id < 92)
        {
            //Kill Player
            Debug.Log("WOF Reward: Player Death");
            yield return PlayRewardSound(1);
            yield return TryKillPlayer();
        }
        else if (id < 102)
        {
            Debug.Log("WOF Reward: Fragments");
            if (GameBase.CanGetReward())
            {
                yield return PlayRewardSound(0);
                yield return GrantResource();
            }
        }
        else if (id < 115)
        {
            Debug.Log("WOF Reward: Player Death");
            yield return PlayRewardSound(1);
            yield return TryKillPlayer();
        }
        else if(id < 117)
        {
            Debug.Log("WOF Reward: Legendary Item");
            if (GameBase.CanGetReward())
            {
                yield return PlayRewardSound(2);
                Notification.Notify("Sorry", "Legendary Item reward not set up");
            }
        }
    }

    IEnumerator TryKillPlayer()
    {
        Transform targHead = PlayerHead.instance.transform;
        if(PhotonNetwork.inRoom && PhotonNetwork.player.ID != spinPlayerID)
        {
            targHead = PlayerHead.GetHead(spinPlayerID);
        }
        yield return new WaitForSeconds(0.2f);
        GameObject newLaser = Instantiate(LaserPrefab, wheel.transform.position, Quaternion.identity);
        newLaser.transform.SetParent(transform);
        newLaser.transform.LookAt(targHead.position);
        newLaser.SetActive(true);
        AudioSource src = newLaser.GetComponent<AudioSource>();
        src.pitch = Random.Range(0.9f, 1.1f);
        Destroy(newLaser, 5f);
        if (LaserImpact != null)
        {
            GameObject NewImpact = Instantiate(LaserImpact, targHead.position, Quaternion.identity);
            NewImpact.SetActive(true);
            Destroy(NewImpact, 5f);
        }
        yield return new WaitForSeconds(0.25f);
        if (!PhotonNetwork.inRoom || PhotonNetwork.player.ID == spinPlayerID)
            PlayerLife.Kill();
    }

    IEnumerator GrantResource()
    {
        int resourceAmt = 100;
        Armory.instance.GiveResource(resourceAmt, false);
        if (MoteParticles.instance != null)
            MoteParticles.instance.SpawnMotes(wheel.transform.position, resourceAmt);
        yield return new WaitForSeconds(1.25f);
    }

    IEnumerator PlayRewardSound(int val)
    {
        RewardSound.clip = RewardClips[val];
        RewardSound.time = 0;
        RewardSound.Play();
        while (RewardSound.isPlaying)
            yield return true;
    }

    #region Event Actions

    public void DamageGate(Vector3 startPt, int damage)
    {
        if(TargetGate == null)
        {
            Debug.Log("Tried to damage gate but didn't exist");
            return;
        }
        if (LaserPrefab != null)
        {
            GameObject newLaser = Instantiate(LaserPrefab, startPt, Quaternion.identity);
            newLaser.transform.SetParent(transform);
            newLaser.transform.LookAt(TargetGate.transform.position);
            newLaser.SetActive(true);
            AudioSource src = newLaser.GetComponent<AudioSource>();
            src.pitch = Random.Range(0.9f, 1.1f);
            Destroy(newLaser, 5f);
            if (LaserImpact != null)
            {
                GameObject NewImpact = Instantiate(LaserImpact, TargetGate.transform.position, Quaternion.identity);
                NewImpact.SetActive(true);
                Destroy(NewImpact, 5f);
            }
        }
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            TargetGate.GetComponent<Health>().takeDamage(damage);
    }

    #endregion

    public override void OnActionTaken(string action)
    {
        if (action == "SPIN_REQUEST" && !requestSpin)
        {
            EventManager.instance.IntEvent1(this, 1);
            if(PhotonNetwork.inRoom)
                EventManager.instance.IntEvent2(this, PhotonNetwork.player.ID);
        }
        else if (action == "END_REQUEST")
            EventManager.instance.IntEvent1(this, 2);
        base.OnActionTaken(action);
    }

    public override void IntEvent1Response(int val)
    {
        if (val == 1)
            requestSpin = true;
        else if (val == 2)
            requestEnded = true;
    }

    public override void IntEvent2Response(int val)
    {
        spinPlayerID = val;
    }

    #region Utility 

    public void ActivateSkip()
    {
        //SkipSpinTarget.SetActive(true);
        //SkipSpinTarget.GetComponent<ArcheryTarget>().Reset();
    }

    public void DeactivateSkip()
    {
        //SkipSpinTarget.SetActive(false);
    }

    #endregion  
}
