using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class AAManager : MonoBehaviour {

    public OwlEvents Aryleth;

    List<GameObject> OwlsOut;
    public int MaxAllowed;

    public Teleporter PlayTP;

    bool playing;

    public OwlType[] Owls;
    int OwlsKilled;

    public Vector3 center;
    public Vector3 variation;
    public bool localVect;

    public EMOpenCloseMotion ScoreList;

    public GameObject SpawnParticles;
    public AudioClip Monologue;
    public AudioClip[] newGameClips;
    public AudioClip[] killBirdClips;
    public AudioClip[] loseGameClips;

    public AudioClip[] spawnClips;

    AudioSource src;
    float volume = 0.6f;

    public AudioSource flavorSource;


    public UnityEvent OnStartEvent;
    public UnityEvent OnEndEvent;

    public static AAManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        src = GetComponent<AudioSource>();
        OwlsOut = new List<GameObject>();
        volume = src.volume;
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    void OnJoinedRoom()
    {
        if(isPlaying())
        {
            ForceEnd();
        }
    }

    [DevConsole.ConsoleCommand]
    public static void OwlGame()
    {
        instance.DevStartGame();
    }

    [AdvancedInspector.Inspect]
    void DevStartGame()
    {
        if (!PhotonNetwork.inRoom && GameBase.instance.Difficulty < 1 && !pvpmanager.instance.PlayingPVP)
            NewGame();
        else
            Debug.Log("Can't start Owl Game right now");
    }

    [AdvancedInspector.Inspect]
    public void StartGameInitial()
    {
        StartCoroutine("InitialStart");
    }

    [AdvancedInspector.Inspect]
    public void ForceEnd()
    {
        StopAllCoroutines();
        ScoreList.Close(true);
        ClearGame(true);
        OnEndEvent.Invoke();
    }

    public bool isPlaying()
    {
        return playing;
    }

    IEnumerator InitialStart()
    {
        if(MusicChanger.instance != null)
            MusicChanger.instance.FadeOutAudio(2);
        if (flavorSource != null && Monologue != null)
        {
            flavorSource.Stop();
            flavorSource.time = 0;
            flavorSource.clip = Monologue;
            flavorSource.Play();
        }
        EnvironmentManager.ChangeEnv(EnvironmentType.Void);
        OnStartEvent.Invoke();
        if (PlayTP != null)
            PlayTP.ForceUse();
        src.volume = volume/1.5f;
        src.time = 0;
        src.Play();
        while(flavorSource.isPlaying)
        {
            yield return true;
        }
        //yield return new WaitForSeconds(16f);
        NewGame(true);
        StartCoroutine("SpawnRepeated");
        GetComponent<AAScoring>().RequestScore();
        while(src.volume < volume)
        {
            src.volume += Time.deltaTime*1.5f;
            yield return true;
            if (src.volume > volume)
                src.volume = volume;
        }
    }

    IEnumerator SpawnRepeated()
    {
        for(int i=0; i<5; i++)
        {
            SpawnOwl();
            yield return new WaitForSeconds(0.15f);
        }
        while(true)
        {
            float seconds = 2.5f - Mathf.Clamp((float)OwlsKilled/25f, 0f, 2f);
            yield return new WaitForSeconds(seconds);
            SpawnOwl();
        }
    }

    public void NewGame(bool firstTime = false)
    {
        ClearGame(false);
        GetComponent<AAScoring>().ChangeScore(0);
        OwlsKilled = 0;
        fadeOutMusic = false;        
        playing = true;        
        if (!firstTime)
        {
            if(MusicChanger.instance != null)
                MusicChanger.instance.FadeOutAudio(2);
            if (PlayTP != null)
                PlayTP.ForceUse();
            src.volume = volume;
            src.time = 0;
            src.Play();
            EnvironmentManager.ChangeEnv(EnvironmentType.Void);
            StartCoroutine("SpawnRepeated");
            if(flavorSource != null)
            {
                flavorSource.Stop();
                flavorSource.time = 0;
                flavorSource.clip = newGameClips[Random.Range(0, newGameClips.Length)];
                flavorSource.Play();
            }
            OnStartEvent.Invoke();
        }
    }

    void SpawnOwl()
    {
        if (!playing || OwlsOut.Count >= MaxAllowed)
            return;
        GameObject prefab = GetOwlToSpawn();
        Vector3 p = Vector3.zero;
        GameObject newOwl = (GameObject)Instantiate(prefab, p, Quaternion.identity);
        if(localVect)
        {
            newOwl.GetComponent<TestWaypoints>().SetupRange(center, variation);
        }
        else
            newOwl.GetComponent<TestWaypoints>().SetupRange(center, variation);
        Vector3 sPos = newOwl.GetComponent<TestWaypoints>().GetPoint();
        newOwl.transform.position = sPos;
        SpawnParticles.transform.position = sPos;
        VRAudio.PlayClipAtPoint(spawnClips[Random.Range(0, spawnClips.Length)], sPos, 1, 1, 1, 5);
        SpawnParticles.GetComponent<ParticleSystem>().Play();
        newOwl.GetComponent<ArrowImpact>().OnHit.AddListener(this.KillOwl);
        OwlsOut.Add(newOwl);
    }

    GameObject GetOwlToSpawn()
    {
        List<GameObject> possible = new List<GameObject>();
        foreach(var v in Owls)
        {
            if(OwlsKilled >= v.threshold)
            {
                for (int i = 0; i < v.Weight; i++)
                    possible.Add(v.Prefab);
            }
        }
        if (possible.Count > 0)
            return possible[Random.Range(0, possible.Count)];
        return null;
    }

    public void KillOwl(ArrowCollision owl)
    {
        OwlsKilled++;
        if(OwlsKilled%4 == 0 && Random.Range(0, 100) < 15 && flavorSource != null && !flavorSource.isPlaying)
        {
            flavorSource.Stop();
            flavorSource.time = 0;
            flavorSource.clip = killBirdClips[Random.Range(0, killBirdClips.Length)];
            flavorSource.Play();
        }
        WorthPoints val = owl.hitObj.GetComponentInParent<WorthPoints>();
        if(val != null)
        {
            int pts = val.Value;
            GetComponent<AAScoring>().AddToScore(pts);
        }
        OwlsOut.Remove(val.gameObject);
    }

    bool fadeOutMusic;
    void Update()
    {
        if(playing && PlayerLife.myInstance != null && PlayerLife.myInstance.isDead)
        {
            EndGame();
        }
        if(fadeOutMusic)
        {
            if (src.volume > 0)
            {
                src.volume -= 0.3f * Time.deltaTime;
            }
            else
                fadeOutMusic = false;
        }
    }

    public void EndGame(bool firstLoad = false)
    {
        ClearGame(true);
        fadeOutMusic = true;
        if (!firstLoad && flavorSource != null)
        {
            flavorSource.Stop();
            flavorSource.time = 0;
            flavorSource.clip = loseGameClips[Random.Range(0, loseGameClips.Length)];
            flavorSource.Play();
        }
        GetComponent<AAScoring>().SubmitIfNewHigh();
        OnEndEvent.Invoke();
    }

    void ClearGame(bool full)
    {
        for (int i = 0; i < OwlsOut.Count; i++)
        {
            if(OwlsOut[i] != null)
            {
                Spawner sp = OwlsOut[i].GetComponentInChildren<Spawner>();
                if (sp != null)
                    sp.ClearAll();
                Destroy(OwlsOut[i]);
            }
        }
        OwlsOut = new List<GameObject>();
        StopCoroutine("SpawnRepeated");
        playing = false;
        if (flavorSource != null)
            flavorSource.Stop();      
        if(full)
        {
            if(MusicChanger.instance != null)
                MusicChanger.instance.FadeInAudio(2);
            EnvironmentManager.ChangeEnv(EnvironmentType.Olympus);
        }
    }
}

[System.Serializable]
public class OwlType
{
    public GameObject Prefab;
    [Range(0,50)]
    public float Weight;
    public int threshold;
}
