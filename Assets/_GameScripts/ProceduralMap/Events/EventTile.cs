using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTile : MonoBehaviour {

    [Header("Event Base")]
    public Teleporter StartTP;
    public Teleporter KillTP;
    public Transform DeathLocation;
    public Transform RewardLocation;
    public bool GivesRewardOnEnd = true;
    public GameObject EnterPortal;
    public GameObject ExitPortal;
    public GameObject EnterSky;
    public UnityEvent OnStart;
    public UnityEvent OnEnd;
    public int PlayerDeathDamage;
    public bool HideEntryWall = false;
    public GameObject SkipTarget;

    public float MusicVolume = 0.5f;
    AudioSource EventMusic;

    [HideInInspector]
    public EventManager mgr;
    [HideInInspector]
    public Gate TargetGate;
    [HideInInspector]
    public Gate NextGate;

    //Private Variables
    MapTile tile;
    [HideInInspector]
    public bool startedIntro;

    #region Life Cycle

    public virtual void Awake()
    {
        mgr = EventManager.instance;
        tile = GetComponentInParent<MapTile>();
        EventMusic = GetComponent<AudioSource>();
        foreach (var tp in GetComponentsInChildren<Teleporter>())
        {
            tp.OnTeleport.AddListener(delegate
            {
                if(tile != null)
                {
                    EnvironmentManager.ChangeEnv(tile.Environment);
                    TileManager.instance.ShowVisible(tile);
                }
                StartIntro();
            });
        }
        EventManager.Tiles.Add(this);
        //EnterPortal.GetComponent<ArrowImpact>().OnHit.AddListener(delegate { EventManager.instance.EnterTile(this); });
        EnterPortal.GetComponentInChildren<PlayerTrigger>().OnPlayerEnter.AddListener(delegate { EventManager.instance.EnterTile(this); });
        EventSeed = Random.Range(0, int.MaxValue);
        Rand = new System.Random(EventSeed);
        if (SkipTarget != null)
            SkipTarget.SetActive(false);
    }

    public void AllEnter()
    {
        StartTP.ForceUse();
        EnterPortal.SetActive(false);
        if(HideEntryWall)
        {
            LavaEntrance l = transform.parent.GetComponentInChildren<LavaEntrance>();
            if (l != null)
                l.gameObject.SetActive(false);
        }
        if(EnterSky != null)
        {
            StartCoroutine("EventSkyStart");
        }
    }

    IEnumerator EventSkyStart()
    {
        yield return true;
        EnterSky.gameObject.SetActive(true);
        EnterSky.transform.position = PlayerHead.instance.transform.position;
        EnterSky.GetComponent<BeautifulDissolves.Dissolve>().TriggerDissolve();
    }

    public virtual void Start()
    {
        RewardLocation.gameObject.SetActive(false);
    }

    public void SetNextGate(Gate g)
    {
        NextGate = g;
        ExitPortal.GetComponent<ArrowImpact>().OnHit.AddListener(delegate {
            NextGate.ForceRestore();
            DisableInternalTPs();
            ExitPortal.SetActive(false);
            RewardLocation.gameObject.SetActive(false);
        });

        g.OnClose.AddListener(delegate
        {
            DisableInternalTPs();
            ExitPortal.SetActive(false);
        });
    }

    bool musicOn;
    public virtual void Update()
    {
        if (EventMusic != null && musicOn)
        {
            float vol = Settings.GetFloat("MusicVolume", 0.8f);
            EventMusic.volume = MusicVolume * vol;
        }           
        if (EventManager.InEvent && TargetGate != null &&  TargetGate.destroyed)
        {
            StopAllCoroutines();
            EventManager.SetInEvent(false);
            ArrowEffects.EffectsDisabled = false;
            TryEndMusic();
        }
    }

    public virtual void EventEndCleanup() { }

    public void EnableEntrance()
    {
        if (EnterPortal != null)
        {
            //EnterPortal.SetActive(true);
            //Invoke("EnableEnterCollider", 8f);
            Invoke("EnableEnterCollider", 3.5f);
        }

    }

    void EnableEnterCollider()
    {
        EnterPortal.SetActive(true);
        //EnterPortal.GetComponent<Collider>().enabled = true;
    }

    public void EnableExitPortal()
    {
        if (ExitPortal != null)
        {
            ExitPortal.GetComponent<Collider>().enabled = true;
        }
    }

    public virtual void StartIntro()
    {
        if(!startedIntro)
        {
            mgr.StartIntro(this);
            if (SkipTarget != null)
                SkipTarget.SetActive(true);
            if (SkipTarget != null)
                SkipTarget.GetComponent<ArrowImpact>().OnHit.AddListener(delegate { HitSkipTarget(); });
            EventManager.SetInEvent(true);
            startedIntro = true;
            if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
                mgr.SetEventSeed(this, Random.Range(0, int.MaxValue));
        }
    }

    public void OnIntroEnd()
    {
        StartEvent();
    }

    bool syncedRandom;
    public IEnumerator SyncRandom()
    {
        if(PhotonNetwork.inRoom)
        {
            syncedRandom = false;
            if (PhotonNetwork.isMasterClient)
                EventManager.instance.SyncEventSeed(this, EventSeed, randNum);
            float t = 0;
            while(!syncedRandom && t < 5f)
            {
                t += Time.deltaTime;
                yield return true;
            }
        }
    }

    public virtual void StartEvent()
    {
        OnStart.Invoke();
        if (SkipTarget != null)
            SkipTarget.SetActive(false);
        PlayerLife.myInstance.SetGateDamage(PlayerDeathDamage);
        if (DeathLocation != null)
            TeleporterManager.instance.UpdateDeathTP(DeathLocation, KillTP);
        if(NextGate != null)
        {
            MapTile mp = NextGate.GetComponentInParent<MapTile>();
            if (mp != null)
            {
                foreach(var v in mp.Inside)
                {
                    foreach (var t in v.Teleporters)
                    {
                        Teleporter tp = t.GetComponentInChildren<Teleporter>();
                        if (tp != null)
                        {
                            tp.ForceDisable(true);
                            tp.DisableTeleport();
                        }
                    }
                }
            }
        }
        if (TargetGate != null)
        {
            MapTile mp = TargetGate.GetComponentInParent<MapTile>();
            if (mp != null)
            {
                foreach (var v in mp.NextZone)
                {
                    foreach (var t in v.Teleporters)
                    {
                        Teleporter tp = t.GetComponentInChildren<Teleporter>();
                        if (tp != null)
                            tp.ForceDisable(true);
                    }
                }
            }
        }
    }

    public virtual void EndEvent()
    {
        OnEnd.Invoke();
        if (TargetGate != null)
            TargetGate.GetHP().invincible = true;
        EventManager.SetInEvent(false);
        RewardLocation.gameObject.SetActive(true);
        Invoke("RewardSequence", 5f);
        if(PlayerLife.myInstance != null)
            PlayerLife.myInstance.ResetGateDamage();
        TryEndMusic();
        if (NextGate != null)
        {
            MapTile mp = NextGate.GetComponentInParent<MapTile>();
            if (mp != null)
            {
                foreach (var v in mp.Inside)
                {
                    foreach (var t in v.Teleporters)
                    {
                        Teleporter tp = t.GetComponentInChildren<Teleporter>();
                        if (tp != null)
                            tp.ForceDisable(false);
                    }
                }
            }
        }
    }

    public void DisableInternalTPs()
    {
        foreach(var v in gameObject.GetComponentsInChildren<Teleporter>())
        {
            v.ForceDisable(true);
            v.Disabled = true;
        }
    }

    public void TryStartMusic()
    {
        if(EventMusic != null)
        {
            MusicChanger.instance.FadeOutAudio(6);
            MusicScheduler2.instance.volume = 0;
            StopCoroutine("fadeMusic");
            StartCoroutine("fadeMusic", true);
        }
    }

    public void TryEndMusic()
    {
        musicOn = false;
        if(EventMusic != null)
        {
            if(EnvironmentManager.instance.UseDynamicMusic)
            {
                MusicScheduler2.instance.volume = 1;
            }
            else
            {
                MusicChanger.instance.FadeInAudio(6);
            }
            StopCoroutine("fadeMusic");
            StartCoroutine("fadeMusic", false);
        }
    }

    IEnumerator fadeMusic(bool fadeIn)
    {
        if (fadeIn)
            EventMusic.Play();
        float t = EventMusic.volume;
        float musicVol = Settings.GetFloat("MusicVolume");
        while ((t < MusicVolume*musicVol && fadeIn) || (!fadeIn && t > 0))
        {
            t += (fadeIn ? 1f : -1f) * Time.deltaTime *0.1f;
            EventMusic.volume = t;
            yield return true;
        }
        if (!fadeIn)
        {
            musicOn = false;
            EventMusic.Stop();
        }
        else
        {
            musicOn = true;
        }

    }

    void RewardSequence()
    {
        Teleporter RewardTP = RewardLocation.GetComponentInChildren<Teleporter>();
        if(TelePlayer.instance.currentNode != RewardTP)
            RewardTP.ForceUse();
        if(GivesRewardOnEnd)
        {
            RewardPillar.Reset();
            RewardPillar.EnterSequence(RewardLocation.position + (Vector3.up*0.2f), RewardLocation.rotation, true);
        }
        Invoke("StartExitSequence", 8f);
    }

    void StartExitSequence()
    {
        ExitPortal.SetActive(true);
        Invoke("EnableExitPortal", 20f);
    }

    public virtual void OnDestroy()
    {
        EventManager.Tiles.Remove(this);
    }

    public virtual void OnActionTaken(string action) { }

    bool requestedSkip;
    public void HitSkipTarget()
    {
        if(!requestedSkip)
        {
            requestedSkip = true;
            mgr.SkipRequest(this);
        }

    }

    public virtual void SkipEvent()
    {
        if (PlayerLife.myInstance != null)
            PlayerLife.myInstance.ResetGateDamage();
        TryEndMusic();
        if (NextGate != null)
        {
            MapTile mp = NextGate.GetComponentInParent<MapTile>();
            if (mp != null)
            {
                foreach (var v in mp.Inside)
                {
                    foreach (var t in v.Teleporters)
                    {
                        Teleporter tp = t.GetComponentInChildren<Teleporter>();
                        if (tp != null)
                            tp.ForceDisable(false);
                    }
                }
            }
        }
        EventManager.SetInEvent(false);
        if (NextGate != null)
        {
            NextGate.ForceRestore();
            DisableInternalTPs();
        }
    }

    #endregion

    #region NetworkedReturns

    public virtual void IntEvent1Response(int val) {}
    public virtual void IntEvent2Response(int val) {}
    public virtual void IVVEventResponse(int ival, Vector3 v1, Vector3 v2) {}
    public virtual void Vec3Event1Response(Vector3 val) {}
    public virtual void Vec3Event2Response(Vector3 val) {}
    public virtual void Vec3Event3Response(Vector3 val) {}
    public virtual void QVEvent1Response(Vector3 val, Quaternion rot) { }

    int skipreqs;
    public void SkipRequest()
    {
        skipreqs++;
        if(!PhotonNetwork.inRoom || (PhotonNetwork.isMasterClient && skipreqs > (PhotonNetwork.playerList.Length/2f)))
        {
            mgr.SkipEvent(this);
        }
    }
    #endregion

    #region Utility

    [HideInInspector]
    public int EventSeed;
    [HideInInspector]
    public int randNum;
    [HideInInspector]
    public System.Random Rand;
    public void SetSeed(int seed)
    {
        Debug.Log("Event Seed Set: " + seed);
        EventSeed = seed;
        Rand = new System.Random(EventSeed);
        randNum = 0;
    }
    public void SyncSeed(int seed, int num)
    {
        if (EventSeed != seed)
            SetSeed(seed);
        if(num > randNum)
        {
            int diff = num - randNum;
            for(int i=0; i<diff; i++)
            {
                GetRandomNext(0, 5);
            }
            Debug.Log("Random was out of sync at seed - Fixed: " + num + " - " + randNum);
        }
        else if(num < randNum)
        {
            SetSeed(seed);
            if (num > 0) // CAREFUL - Recursion
                SyncSeed(seed, num);
        }
        syncedRandom = true;
    }


    public int GetRandomNext(int min, int max)
    {
        randNum++;
        return Rand.Next(min, max);
    }

    public float GetRandomNext(float min, float max)
    {
        randNum++;
        int Min = (int)(min*100000);
        int Max = (int)(max*100000);
        int val = Rand.Next(Min, Max);
        return (val/100000f);
    }

    public void KillPlayer(float dur, bool ignoreTP = false)
    {
        if (PlayerLife.myInstance != null)
            PlayerLife.myInstance.Die(dur, ignoreTP, KillTP);
    }

    #endregion

}
