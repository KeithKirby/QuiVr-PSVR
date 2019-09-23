using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Gate : MonoBehaviour {

    public GateHalf[] Halves;
    public int CloseArea;
    [HideInInspector]
    public bool closed;
    [HideInInspector]
    public bool disabled;
    public bool destroyed;
    public EnemyStream Streamer;
    public EnemyStream NextStreamer;
    public int ReviveRequirement;
    public int ReviveIncrease = 5;
    int curRevReq;
    public GameObject ReviveDisplay;
    public UnityEvent OnClose;
    public UnityEvent OnDeath;
    public UnityEvent OnReset;
    public UnityEvent OnRestore;

    public float AvgTakeoverTime;

    public int CloseDifficulty;

    public ParticleSystem ClosePulse;

    public AudioSource[] CloseSounds;
    public AudioSource[] BreakSounds;
    Health h;

    public int GateIndex;

    public ParticleSystem ReviveEffect;

    public float DamageOnHit;
    public GameObject LaserPrefab;
    public GameObject LaserImpact;

    public Health GetHP()
    {
        return h;
    }

    void Awake()
    {
        h = GetComponent<Health>();
        displays = ReviveDisplay.GetComponentsInChildren<MeshRenderer>();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        ResetReq();
        h.OnAttacker.AddListener(OnAttacked);
        GateManager.instance.AddGate(this);
    }

    public void DisableGate()
    {
        disabled = true;
    }

    public bool isDisabled()
    {
        return disabled;
    }

    MeshRenderer[] displays;
    void Update()
    {
        if(!closed && !disabled)
        {
            closed = (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient);
            foreach(var v in Halves)
            {
                if(!v.isClosed())
                {
                    closed = false;
                }
            }
            if(closed)
            {
                ActivateClose();
            }
        }
        if (h.isDead() && !destroyed)
            TryDestroyGate();
        if (destroyed)
        {
            if (!ReviveDisplay.activeSelf)
                ReviveDisplay.SetActive(true);
            foreach(var v in displays)
            {
                Color c = v.material.GetColor("_Color");
                c.a = (float)RevAcum / (float)ReviveRequirement;
                v.material.SetColor("_Color", c);
            }
        }
        else if (ReviveDisplay.activeSelf)
            ReviveDisplay.SetActive(false);
        if (h.isDead() && !destroyed && (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient))
            TryDestroyGate();
    }

    public bool isDestroyed()
    {
        return destroyed;
    }

    public bool isClosed()
    {
        return closed;
    }

    [AdvancedInspector.Inspect]
    public void ForceRestore()
    {
        if(h.isDead())
        {
            if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
                Restore();
            else if (PhotonNetwork.inRoom)
                GateManager.instance.ForceRestore(this, PhotonTargets.MasterClient);//.RPC("RestoreNetwork", PhotonTargets.Others);
        }
    }

    public void Restore()
    {
        if(h.isDead())
        {
            ActivateRestore();
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                GateManager.instance.Restore(this, PhotonTargets.Others);// GetComponent<PhotonView>().RPC("RestoreNetwork", PhotonTargets.Others);
        }  
    }

    public void ResetReq()
    {
        curRevReq = ReviveRequirement;
        FirstClose = false;
    }

    public int RevAcum;
    public void AddRevReq(int val)
    {
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            GateManager.instance.AddRevive(this, val, PhotonTargets.MasterClient);//GetComponent<PhotonView>().RPC("AddRevNetwork", PhotonTargets.MasterClient, val);
        else
            AddRevive(val);
    }

    public void AddRevive(int val)
    {
        RevAcum += val;
        int req = CalculateReviveReq();
        if (destroyed && RevAcum >= req)
            Restore();
    }

    public int CalculateReviveReq()
    {
        int req = curRevReq;
        if (PhotonNetwork.inRoom && PhotonNetwork.playerList.Length > 1)
        {
            int plrNum = PhotonNetwork.playerList.Length;
            float[] diffMults = { 1f, 1.6f, 2.5f, 3.2f };
            req = (int)(curRevReq * diffMults[plrNum - 1]);
        }
        return req;
    }

    public void ActivateRestore()
    {
        curRevReq += ReviveIncrease;
        closed = false;
        h.Revive(true);
        destroyed = false;
        RevAcum = 0;
        OnRestore.Invoke();
        RestoreDisplay();
        if (GateManager.instance != null)
            GateManager.instance.AddRestore(GateIndex);
        //ForceClose(); //Close on end
    }

    public void SetEmission(Color c)
    {
        foreach(var v in Halves)
        {
            if (v.GateDisplay != null)
                v.GateDisplay.material.SetColor("_EmissionColor", c * 2.5f);
        }
    }

    public void RestoreDisplay(bool immediate=false)
    {
        if(!immediate)
        {
            ReviveEffect.Play();
            if (ReviveEffect.GetComponent<AudioSource>() != null)
                ReviveEffect.GetComponent<AudioSource>().Play();
        }       
        foreach (var v in Halves)
        {
            v.gameObject.SetActive(true);
            v.Reset();
        }
        ForceClose();
    }

    [AdvancedInspector.Inspect]
    public void ForceClose()
    {
        if (closed)
            return;
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            if (EventManager.InEvent) // If in event, skip it
            {
                var et = GameObject.FindObjectOfType<EventTile>();
                et.SkipEvent();
            }
            
            foreach (var v in Halves)
            {
                v.CloseNow();
            }
        }
        else if (PhotonNetwork.inRoom)
            GateManager.instance.ForceClose(this, PhotonTargets.MasterClient);//GetComponent<PhotonView>().RPC("ForceClose", PhotonTargets.MasterClient);
    }

    public void TryCloseNetwork()
    {
        if (!closed)
        {
            closed = true;
            ActivateClose();
        }
        else
        {
            CloseEffects();
        }
    }

    public bool FirstClose;
    void ActivateClose()
    {
        EnvironmentType env = EnvironmentType.Snow;
        if (GetComponentInParent<MapTile>() != null)
            env = GetComponentInParent<MapTile>().Environment;
        DebugTimed.Log("Gate " + (CloseArea + 1) + " Closed - Environment: " + env);
        FPSText.LogAvgFPS();
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            PlatformSetup.instance.IncrementVersionValue("Gate" + (CloseArea + 1) + "Closed", 1);
            //GameBase.instance.IncreaseDifficulty(CloseDifficulty);
            if(!FirstClose)
            {
                FirstClose = true;
                GameBase.instance.IncreaseDifficulty(100);
            }
        }
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            GateManager.instance.TryClose(this, PhotonTargets.Others);//GetComponent<PhotonView>().RPC("TryCloseNetwork", PhotonTargets.Others);
        if (Streamer != null)
        {
            Streamer.EndStream();
            GameBase.instance.KillAll();
        }
        if (NextStreamer != null)
        {
            NextStreamer.BeginStream();
            NextStreamer.Invoke("SpawnClose", 2.5f);
        }
        h.SetInvincible(5);
        CloseEffects();
        TeleporterManager.instance.EnableArea(CloseArea + 1);
        TeleporterManager.instance.UpdateDeathTP(transform, Vector3.up*-6);
        Invoke("ActivateTeleporterChange", 1f);
        EndTakeover();
        /* CHANGE DIFFICULTY
        Health h = Streamer.TargetWall.GetComponent<Health>();
        if (h != null && Streamer.usingDifficulty)
        {
            GameBase.instance.Difficulty += 2f * (h.currentHP / h.maxHP);
            if (h.currentHP == h.maxHP)
                GameBase.instance.Difficulty += 1.5f;
        }
        */
        Statistics.SaveStatistics();
        OnClose.Invoke();
    }

    void CloseEffects()
    {
        if (ClosePulse != null)
            ClosePulse.Play();
        foreach (var v in CloseSounds)
        {
            v.Play();
        }
        PlayerShake.Shake(1.25f, 0.6f, true);
    }

    public void OnAttacked(GameObject enemy)
    {
        if(DamageOnHit > 0 && enemy != null)
        {
            Creature c = enemy.GetComponent<Creature>();
            if(c != null && c.type.status == CreatureStatus.Standard)
            {
                Vector3 targ = enemy.transform.position + Vector3.up;
                DoLaser(targ,targ);
                if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                    GateManager.instance.SendLaser(this, PhotonTargets.Others, targ);
                c.health.takeDamage(DamageOnHit, false);
            }
        }
    }

    public void DoLaser(Vector3 LookPos, Vector3 EnemyPos)
    {
        if (LaserPrefab != null)
        {
            GameObject newLaser = Instantiate(LaserPrefab, LaserPrefab.transform.position, Quaternion.identity);
            newLaser.transform.SetParent(transform);
            newLaser.transform.LookAt(LookPos);
            newLaser.SetActive(true);
            AudioSource src = newLaser.GetComponent<AudioSource>();
            src.volume = src.volume * VolumeSettings.GetVolume(AudioType.Effects);
            src.pitch = Random.Range(0.9f, 1.1f);
            Destroy(newLaser, 5f);
            if (LaserImpact != null)
            {
                GameObject NewImpact = Instantiate(LaserImpact, EnemyPos, Quaternion.identity);
                NewImpact.SetActive(true);
                Destroy(NewImpact, 5f);
            }
        }
    }

    public float CurHP()
    {
        return h.currentHP;
    }

    [AdvancedInspector.Inspect]
    void ForceDestroyGate()
    {
        if (!destroyed)
        {
            DestroyGate();
            if (PhotonNetwork.inRoom)
                GateManager.instance.DestroyGate(this, PhotonTargets.Others);//GetComponent<PhotonView>().RPC("DestroyGate", PhotonTargets.Others);
        }
    }

    public void TryDestroyGate()
    {
        if ((!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient) && !destroyed && !TestOptions.SInvulnerableGate)
        {
            if(NextStreamer != null && NextStreamer.RestoreGate != null)
            {
                if (!NextStreamer.RestoreGate.isDestroyed() && NextStreamer.RestoreGate.isClosed())
                {
                    h.currentHP = 1;
                    h.dead = false;
                    return;
                }

            }
            DestroyGate();
            if (PhotonNetwork.inRoom)
                GateManager.instance.DestroyGate(this, PhotonTargets.Others);//GetComponent<PhotonView>().RPC("DestroyGate", PhotonTargets.Others);
        }
    }

    public void DestroyGate()
    {
        DebugTimed.Log("Gate " + (CloseArea + 1) + " Destroyed");
        destroyed = true;
        h.currentHP = 0;
        if (NextStreamer != null)
        {
            NextStreamer.EndStream();
            NextStreamer.SwitchStream(Streamer);
        }
        KillNearby(35);

        if (ClosePulse != null)
            ClosePulse.Play();
        PlayerShake.Shake(1f, 0.6f, true);
        if (Streamer != null)
        {
            Streamer.BeginStream();
            TeleporterManager.instance.UpdateDeathTP(Streamer.TargetWall.transform, Vector3.up*-6f);
        }
        Invoke("DestroyTeleporterChange", 2f);
        TeleporterManager.instance.EnableArea(CloseArea);
        if (TeleporterManager.instance.Areas[CloseArea].BackTeleporter != null)
            TeleporterManager.instance.Areas[CloseArea].BackTeleporter.Invoke("ForceUse", 1.75f);
        DestroyDisplay();
        Statistics.SaveStatistics();
        OnDeath.Invoke();
    }

    void KillNearby(float distance)
    {
        if(GameBase.instance != null)
        {
            foreach(var v in CreatureManager.AllEnemies())
            {
                if(v != null && !v.isDead() && v.type.status == CreatureStatus.Standard)
                {
                    float dist = Vector3.Distance(transform.position, v.transform.position);
                    if (dist <= distance)
                        v.Kill();
                }
            }
        }
    }

    public void DestroyDisplay(bool immediate=false)
    {
        if (!Application.isPlaying)
            return;
        if(!immediate)
        {
            foreach (var v in BreakSounds)
            {
                v.Play();
            }
        }
        foreach (var v in Halves)
        {
            v.DoDestroy(immediate);
        }
    }

    void DestroyTeleporterChange()
    {
        TeleporterManager.instance.DisableArea(CloseArea+1, CloseArea);
    }

    void ActivateTeleporterChange()
    {
        if(!destroyed && TeleporterManager.instance.Areas.Length > CloseArea+1)
        {
            if (TeleporterManager.instance.Areas[CloseArea + 1].StartTeleporter != null && !TeleporterManager.instance.PlayerInArea(CloseArea + 1))
                TeleporterManager.instance.Areas[CloseArea + 1].StartTeleporter.ForceUse();
            TeleporterManager.instance.DisableArea(CloseArea, CloseArea + 1);
        }
    }

    /*
    void OnPhotonPlayerConnected(PhotonPlayer plr)
    {
        if (PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("SetRotationValue", plr, Halves[0].transform.rotation, Halves[1].transform.rotation, closed, destroyed);
        }
    }
   

    [PunRPC]
    void SetRotationValue(Quaternion r1, Quaternion r2, bool cl, bool dst)
    {
        Halves[0].transform.rotation = r1;
        Halves[1].transform.rotation = r2;
        closed = cl;
        destroyed = dst;
        if(dst)
        {
            Halves[0].DoDestroy();
            Halves[1].DoDestroy();
        }
    }
     */

    public void ResetGate(bool doTeleporters=true)
    {
        h.Revive(true);
        RevAcum = 0;
        inTakeover = false;
        foreach (var v in Halves)
        {
            v.gameObject.SetActive(true);
            v.Reset();
        }
        OnReset.Invoke();
        if(doTeleporters)
        {
            if (CloseArea == 0)
                TeleporterManager.instance.EnableArea(CloseArea);
            TeleporterManager.instance.DisableArea(CloseArea + 1);
        }
        closed = false;
        destroyed = false;
        disabled = false;
    }

    //TODO - Swap to Gate Manager
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(Halves[0].GateObject.localEulerAngles);
            stream.SendNext(Halves[1].GateObject.localEulerAngles);
            stream.SendNext(closed);
            stream.SendNext(destroyed);
            stream.SendNext(disabled);
            stream.SendNext(RevAcum);
        }
        else
        {
            Halves[0].GateObject.localEulerAngles = (Vector3)stream.ReceiveNext();
            Halves[1].GateObject.localEulerAngles = (Vector3)stream.ReceiveNext();
            closed = (bool)stream.ReceiveNext();
            destroyed = (bool)stream.ReceiveNext();
            disabled = (bool)stream.ReceiveNext();
            RevAcum = (int)stream.ReceiveNext();
        }
    }

    bool inTakeover;
    float curTime;
    public void BeginTakeover()
    {
        if(!inTakeover)
        {
            inTakeover = true;
            curTime = Time.time;
        }
    }

    public void EndTakeover()
    {
        if(inTakeover)
        {
            inTakeover = false;
            float timeTaken = Mathf.Abs(Time.time - curTime);
            int deaths = GameBase.instance.PlayerDeaths;
            float AvgedTime = (2*AvgTakeoverTime)-timeTaken;
            float d = Mathf.Clamp((AvgedTime/30f)/(1+(deaths*0.2f)), 0.25f, 5f);
            GameBase.instance.Difficulty += d;
        }
    }

    void OnDestroy()
    {
        GateManager.instance.RemoveGate(this);
    }
}
