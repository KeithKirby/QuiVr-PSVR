using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOrb : MonoBehaviour {

    public GameObject EnergyPrefab;
    public GameObject DisplayHolder;
    public static GameOrb instance;
    public AudioClip[] GainResourceClips;
    public AudioSource ReadyAudio;
    public AudioClip UseClip;
    public GameObject ReadyMarker;
    public LineRenderer GateConnection;
    public LineRenderer GateConnection2;
    public static Vector3 Target;
    public Transform TargetGate;
    Vector3 StartPos;
    public Image ProgressBar;
    public static int CurrentRevive;

    public ParticleSystem PercDisplay;
    ParticleSystem.EmissionModule perc;

    void Awake()
    {
        CurrentRevive = 0;
        instance = this;
        perc = PercDisplay.emission;
        StartPos = transform.position;
        Target = StartPos;
        ReadyAudio = GetComponent<AudioSource>();
    }

    public static void Reset()
    {
        if(instance != null)
        {
            instance.ClearProgress();
            Target = instance.StartPos;
            instance.transform.position = instance.StartPos;
        }
    }

    void Start()
    {
        InvokeRepeating("CheckDisplay", 0.5f, 0.731f);
    }

    void Update()
    {
        if (CurrentRevive == 0 || req == -1)
            ready = false;
        transform.position = Vector3.MoveTowards(transform.position, Target, Vector3.Distance(transform.position, Target) * Time.deltaTime * 0.25f);
        if(GameBase.instance != null && GameBase.instance.CurrentStream != null && GameBase.instance.CurrentStream.RestoreGate != null)
        {
            Gate g = GameBase.instance.CurrentStream.RestoreGate;
            TargetGate = g.transform;
            GateConnection.SetPosition(0, transform.position);
            GateConnection.SetPosition(1, g.Halves[0].transform.position + (Vector3.up*12));
            GateConnection2.SetPosition(0, transform.position);
            GateConnection2.SetPosition(1, g.Halves[1].transform.position + (Vector3.up * 12));
        }
        if (ready && GameBase.instance.Difficulty > 0)
        {
            ReadyAudio.volume = Mathf.Lerp(ReadyAudio.volume, 1, Time.deltaTime);
            forceNext += Time.unscaledDeltaTime;
            if (forceNext > 3)
            {
                forceNext = 0f;
                TryRevive();
            }
        }
        else
        {
            ReadyAudio.volume = Mathf.Lerp(ReadyAudio.volume, 0, Time.deltaTime);
            forceNext = 0;
        }
        ProgressBar.fillAmount = Mathf.Lerp(ProgressBar.fillAmount, PercComplete, Time.unscaledDeltaTime);
    }

    public bool ready;
    float forceNext;
    public int req;
    float PercComplete;
    void CheckDisplay()
    {
        //Host setup values
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            req = GameBase.instance.GetReviveReq();
            PercComplete = 1;
            if (req > 0)
                PercComplete = (float)CurrentRevive / (float)req;
            if (CurrentRevive < req)
                ready = false;
            else if (PercComplete >= 1 && !ready && (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient))
                ready = true;
        }

        if (req <= 0 && GameBase.instance.Difficulty > 0)
        {
            if (DisplayHolder.activeSelf)
                DisplayHolder.SetActive(false);
        }
        else
        {
            GateConnection.gameObject.SetActive(GameBase.instance.Difficulty > 0);
            GateConnection2.gameObject.SetActive(GameBase.instance.Difficulty > 0);
            if (!DisplayHolder.activeSelf)
                DisplayHolder.SetActive(true);                
            //Toggles        
            ReadyMarker.SetActive(ready);
            GateConnection.material.SetFloat("_TintStrength", ready?3.5f:0.25f);
            GateConnection2.material.SetFloat("_TintStrength", ready ? 3.5f : 0.25f);

            //Variable
            perc.rateOverTime = (int)(PercComplete*30f);
        }
    }

    public static bool GiveEnergy(Vector3 pos, int num=1)
    {
        if(instance != null && instance.req > 0)
        {
            instance.GiveEnergy(pos, num, true);
            return true;
        }
        return false;
    }

    void GiveEnergy(Vector3 pos, int num, bool sync)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject ne = Instantiate(instance.EnergyPrefab, pos, Quaternion.identity);
            ne.SetActive(true);
            ne.GetComponentInChildren<ProjectileCollisionBehaviour>().OnCollide = new UnityEngine.Events.UnityEvent();
            ne.GetComponentInChildren<ProjectileCollisionBehaviour>().OnCollide.AddListener(delegate {GiveResource(1); });
        }
        if(sync && PhotonNetwork.inRoom)
        {
            GetComponent<PhotonView>().RPC("GiveEnergyNetwork", PhotonTargets.Others, pos + (Vector3.up * 2.2f), num);
        }
    }

    [PunRPC]
    void GiveEnergyNetwork(Vector3 pos, int num)
    {
        GiveEnergy(pos, num, false);
    }

    public static void GiveResource(int val=1)
    {
        CurrentRevive += val;
        if(instance != null)
        {
            AudioClip[] clips = instance.GainResourceClips;
            if (clips.Length > 1)
            {
                VRAudio.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], instance.transform.position, .8f*VolumeSettings.GetVolume(AudioType.Effects), Random.Range(0.85f, 1.1f), 1, 10);
            }
        }
    }

    [AdvancedInspector.Inspect]
    [PunRPC]
    public void TryRevive()
    {
        Gate g = TargetGate.GetComponent<Gate>();
        if (GameBase.instance.Difficulty <= 0 && g != null && !g.isClosed() && !g.isDestroyed())
        {
            if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            {
                g.ForceClose();
            }
            return;
        }
        CheckDisplay();
        int x = req;
        if (CurrentRevive >= x && x > 0 && GameBase.instance.inGame && !CreatureManager.bossOut)
        {
            if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            {
                GetComponent<PhotonView>().RPC("TryRevive", PhotonNetwork.masterClient);
                return;
            }
            ClearProgress();
            GameBase.instance.CurrentStream.RestoreGate.Restore();
        }
    }

    public void ClearProgress()
    {
        CurrentRevive = 0;
        ready = false;
        forceNext = 0;
    }

    public void HalfProgress()
    {
        float cr = CurrentRevive;
        cr /= 2;
        CurrentRevive = (int)cr;
        ready = false;
        forceNext = 0;
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(req);
            stream.SendNext(CurrentRevive);
            stream.SendNext(PercComplete);
            stream.SendNext(ready);
        }
        else if (stream.isReading)
        {
            req = (int)stream.ReceiveNext();
            CurrentRevive = (int)stream.ReceiveNext();
            PercComplete = (float)stream.ReceiveNext();
            ready = (bool)stream.ReceiveNext();
        }
   }

}
