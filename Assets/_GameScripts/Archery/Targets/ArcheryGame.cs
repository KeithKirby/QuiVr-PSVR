using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class ArcheryGame : MonoBehaviour {

    public List<Transform> SpawnPoints;
    public GameObject TargetPrefab;
    List<ArcheryTarget> TargetsOut;

    public static ArcheryGame instance;
    ArcheryScore score;

    public Teleporter TP;

    public bool inGame;
    public int TargetsPerWave;
    public int WaveNum;
    int wave;

    public float GameTime = 120;
    float timer;

    public float WaveTime;

    public UnityEvent OnNewGame;
    public UnityEvent OnEndGame;
    public UnityEvent OnNextWave;
    public UnityEvent OnCompletedSet;

    public bool IsPlaying
    {
        set
        {
            if (_isPlaying != value)
            {
                _isPlaying = value;
                PS4Plus.Inst.IsArcheryGameActive = _isPlaying;
            }
        }
        get
        {
            return _isPlaying;
        }
    }
    bool _isPlaying = false;

    void Awake()
    {
        instance = this;
        TargetsOut = new List<ArcheryTarget>();
        ArrowEffects.EffectsDisabled = true;
        score = GetComponent<ArcheryScore>();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        Invoke("SpawnRandomTarget", 2f);
    }

    public int GetTeam()
    {
        if (TP != null)
        {
            int pid = TP.MyPosition();
            if (pid >= 0)
                return (pid % 2) + 1;
        }
        return -1;
    }

    public void Reset()
    {
        StopAllCoroutines();
        ClearTargs();
        TargetsOut = new List<ArcheryTarget>();
        inGame = false;
        IsPlaying = false;
        wave = 0;
        if (ArcheryScore.instance != null)
            ArcheryScore.instance.Reset();
        Invoke("SpawnRandomTarget", 2f);
    }

    public int GetTimer()
    {
        return (int)timer;
    }

    public float FindClosestTarget(Vector3 pos)
    {
        float x = float.MaxValue;
        foreach (var v in TargetsOut)
        {
            if (v != null)
            {
                float dist = Vector3.Distance(pos, v.transform.position);
                if (dist < x)
                    x = dist;
            }
        }
        return x;
    }

    [AdvancedInspector.Inspect]
    public void NewGame() // Player has grabbed the start orb
    {
        if (!PhotonNetwork.inRoom) // Single player
        {
            NewGame(false, PhotonNetwork.player.UserId);
        }
        else
        {
            //if(PhotonNetwork.isMasterClient)
            GetComponent<PhotonView>().RPC("NewGame", PhotonTargets.All, false, PhotonNetwork.player.UserId);
            //NewGame(false);
            //IsPlaying = true;
        }
    }

    [PunRPC]
    public void NewGame(bool forceNew, string invoker)
    {
        if (inGame && forceNew)
            EndGame();
        else if (inGame)
            return;

        wave = 0;
        timer = GameTime;
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            GetComponent<PhotonView>().RPC("ClearTargs", PhotonTargets.Others);
        ClearTargs();
        ArcheryScore.instance.Reset();
        inGame = true;
        if (ArcheryGameAudio.instance != null)
            ArcheryGameAudio.instance.NewGameSequence();
        if(invoker == PhotonNetwork.player.UserId)
        {
            IsPlaying = true;
        }
        Invoke("StartNew", 4f);
    }

    void StartNew()
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            StartCoroutine("Waves");
        if (ArcheryGameAudio.instance != null)
            ArcheryGameAudio.instance.NewGameAudio();
        OnNewGame.Invoke();
    }

    [PunRPC]
    public void EndGame()
    {
        IsPlaying = false;
        inGame = false;
        StopAllCoroutines();
        for (int i=0; i<TargetsOut.Count; i++)
        {
            if(TargetsOut[i] != null)
                TargetsOut[i].Break();
        }
        TargetsOut = new List<ArcheryTarget>();
        if (ArcheryGameAudio.instance != null)
            ArcheryGameAudio.instance.EndGameAudio();
        OnEndGame.Invoke();
        Invoke("SpawnRandomTarget", 2f);
    }

    public void SpawnTargetsSingle()
    {
        if(timeCD <= 0)
        {
            ClearTargs();
            SpawnTargets(0, null);
            timeCD = 1f;
        }
    }

    bool TargetsUp;
    public void SpawnTargets(int targetOverride=0, Transform[] SpawnOverride = null)
    {
        if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            if(TargetsOut.Count < Mathf.Max(TargetsPerWave*2, targetOverride))
            {
                SpawnPoints = Shuffle(SpawnPoints);
                Transform[] spts = SpawnPoints.ToArray();
                if (SpawnOverride != null)
                    spts = SpawnOverride;
                int x = TargetsPerWave;
                if (targetOverride > 0)
                    x = targetOverride;
                for (int i = 0; i < x; i++)
                {
                    Transform spawn = spts[i];
                    if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                        GetComponent<PhotonView>().RPC("SpawnTarget", PhotonTargets.Others, spawn.position, spawn.rotation);
                    SpawnTarget(spawn.position, spawn.rotation);
                }
            }
        }
    }

    public void SpawnRandomTarget()
    {
        if (inGame)
            return;
        score.Reset();
        SpawnPoints = Shuffle(SpawnPoints);
        Transform[] spts = SpawnPoints.ToArray();
        Transform spawn = spts[0];
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            GetComponent<PhotonView>().RPC("SpawnTarget", PhotonTargets.All, spawn.position, spawn.rotation);
        else if(!PhotonNetwork.inRoom)
            SpawnTarget(spawn.position, spawn.rotation);
    }

    [PunRPC]
    public void SpawnTarget(Vector3 pos, Quaternion rot)
    {
        if (ArcheryGameAudio.instance != null)
            ArcheryGameAudio.instance.NextWaveAudio();
        GameObject newTarg = (GameObject)Instantiate(TargetPrefab, pos, rot);
        TargetsOut.Add(newTarg.GetComponent<ArcheryTarget>());
        TargetsUp = true;
    }

    public List<Transform> Shuffle(List<Transform> list)
    {
        System.Random rnd = new System.Random();
        for (int t = 0; t < list.Count; t++)
        {
            Transform tmp = list[t];

            int r = rnd.Next(t, list.Count);
            list[t] = list[r];
            list[r] = tmp;
        }
        return list;
    }

    float timeCD;
    void Update()
    {
        for (int i = TargetsOut.Count - 1; i >= 0; i--)
        {
            if (TargetsOut[i].isBroken())
            {
                if (PhotonNetwork.inRoom)
                    GetComponent<PhotonView>().RPC("NetworkDestroy", PhotonTargets.Others, TargetsOut[i].transform.position);
                TargetsOut.RemoveAt(i);
                if(TargetsUp && TargetsOut.Count == 0)
                {
                    TargetsUp = false;
                    OnCompletedSet.Invoke();
                    if (!inGame)
                        Invoke("SpawnRandomTarget", 0.5f);
                }
            }
        }
        if (inGame && timer > 0)
            timer -= Time.deltaTime;
        if (timeCD > 0)
            timeCD -= Time.deltaTime;
    }

    [PunRPC]
    void NetworkDestroy(Vector3 pos)
    {
        for(int i=TargetsOut.Count-1; i>=0; i--)
        {
            if(Vector3.Distance(TargetsOut[i].transform.position, pos) < 1.5)
            {
                TargetsOut[i].Break(true);
                TargetsOut.RemoveAt(i);
            }
        }
    }

    public int currentWave()
    {
        return wave;
    }

    IEnumerator Waves()
    {
        while(timer > 0)//(wave < WaveNum)
        {
            wave++;
            foreach(var v in TargetsOut)
            {
                if (v != null)
                    v.Break();
            }
            SpawnTargets();
            float t = 9999;//WaveTime;
            while(t > 0 && TargsOut() && timer > 0)
            {
                t -= Time.deltaTime;
                yield return true;
            }
            if (timer > 0)
            {
                if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                    GetComponent<PhotonView>().RPC("ClearTargs", PhotonTargets.Others);
                ClearTargs();
                if (ArcheryGameAudio.instance != null)
                    ArcheryGameAudio.instance.EndWaveAudio();
                //yield return new WaitForSeconds(1.5f);
            }
        }
        if (PhotonNetwork.inRoom)
            GetComponent<PhotonView>().RPC("EndGame", PhotonTargets.Others);
        EndGame();
    }

    [PunRPC]
    void ClearTargs()
    {
        foreach (var v in TargetsOut)
        {
            if (v != null && !v.isBroken())
            {
                v.Break();
            }
        }
    }

    bool TargsOut()
    {
        if (TargetsOut.Count == 0)
            return false;
        foreach(var v in TargetsOut)
        {
            if (v != null && !v.isBroken())
                return true;
        }
        return false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(inGame);
            stream.SendNext(wave);
            stream.SendNext(timer);
        }
        else if (stream.isReading)
        {
            inGame = (bool)stream.ReceiveNext();
            wave = (int)stream.ReceiveNext();
            timer = (float)stream.ReceiveNext();
        }
    }

    void OnLeftRoom()
    {
        OnEndGame.Invoke();
        Reset();
    }

    void OnJoinedRoom()
    {
        Reset();
        OnEndGame.Invoke();
    }
}
