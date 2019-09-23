using UnityEngine;
using System.Collections;

public class OwlMgr : MonoBehaviour {

    public static OwlMgr instance;
    public GameObject OwlPrefab;

    public AudioClip[] IntroMsgs;
    public AudioClip[] IntroMsgsHurry;
    public AudioClip[] IntroMsgsAnnoy;

    public GameObject currentOwl;
    Health cowlHP;

    public int OwlDeaths;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (currentOwl != null)
            cowlHP = currentOwl.GetComponent<Health>();
    }

    public void Reset()
    {
        didFirst = false;
        OwlDeaths = 0;
    }

    float lastMsg;
    void Update()
    {
        if(GameBase.instance != null && GameBase.instance.inIntro() && GameBase.instance.IntroTimeTaken > lastMsg+30 && currentOwl != null)
        {
            lastMsg = GameBase.instance.IntroTimeTaken;
            int id = 1;
            if (lastMsg > 120)
                id = 2;
            PlayIntroMsg(id);
        }
    }

    bool didFirst;
    public void PlayFirstIntro()
    {
        if(!didFirst)
        {
            didFirst = true;
            Invoke("PlayIF", 1f);
        }
    }

    void PlayIF()
    {
        PlayIntroMsg(0);
    }

    public void PlayIntroMsg(int id=0)
    {
        AudioClip[] c = IntroMsgs;
        if (id == 1)
            c = IntroMsgsHurry;
        else if (id == 2)
            c = IntroMsgsAnnoy;
        AudioClip clip = c[Random.Range(0, c.Length)];
        if(cowlHP != null && !cowlHP.isDead())
        {
            currentOwl.GetComponent<OwlSounds>().PlayAudioclip(clip);
        }
    }

    void SpawnNew()
    {
        //TODO
    }

    public void DisableOwl()
    {
        if (currentOwl != null)
            currentOwl.SetActive(false);
    }

    public void EnableOwl()
    {
        if (currentOwl != null)
            currentOwl.SetActive(true);
    }

    public void KillOwl()
    {
        OwlDeaths++;
        currentOwl = null;
        cowlHP = null;
        Invoke("SpawnNew", 5f);
        if (PhotonNetwork.inRoom)
            GetComponent<PhotonView>().RPC("KillOwlNetwork", PhotonTargets.Others);
    }

    [PunRPC]
    void KillOwlNetwork()
    {
        if (currentOwl != null && currentOwl.GetComponent<Health>() != null && !currentOwl.GetComponent<Health>().isDead())
        {
            currentOwl.GetComponent<Health>().takeDamage(1000);
        }
    }
}
