using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class SingleplayerSpawn : MonoBehaviour {

    public GameObject PlayerPrefab;
    public GameObject PS4Prefab;
    public GameObject MobilePrefab;
    public Teleporter startNode;
    public Teleporter skipLobby;

    public GameObject DevCam;

    public GameObject PlatformPrefab
    {
        get
        {
#if UNITY_PS4
            return PS4Prefab;
#else
            return PlayerPrefab;
#endif
        }
    }

    public bool skipIntro;

    public Vector3 startPos;

    public static SingleplayerSpawn instance;
    public ArrowImpact AutoStartInvoker;
    public UnityEvent OnStartScene;
    public UnityEvent OnPlayerSpawned;

    public bool ForceMobile;

    void Awake()
    {
        instance = this;
        var sf = RenderMode.GetInst();
        if(null!=sf)
            sf.ForceLoadTransition();
    }

	// Use this for initialization
	void Start ()
    {
        SpawnSinglePlayer();
        OnStartScene.Invoke();
    }

    public void SpawnSinglePlayer()
    {
        bool mobile = false;
#if UNITY_STANDALONE || UNITY_WSA || UNITY_PS4
        mobile = ForceMobile;
#else
        mobile = true;
#endif
        if(!mobile)
        {
            StopCoroutine("SpawnPlayer");
            StartCoroutine("SpawnPlayer");
        }
        else
            SpawnMobilePlayer();
    }

    public void SpawnMobilePlayer()
    {
        if (DevCam != null)
            DevCam.SetActive(false);
        GameObject plr = (GameObject)Instantiate(MobilePrefab, startPos, Quaternion.identity);
    }

	public void UseStart()
	{
        //FindObjectOfType<TeleportPlayer> ().Teleport (startNode);
        startNode.ForceUse();
	}

    IEnumerator SpawnPlayer()
    {
        var sf = RenderMode.GetInst();
        sf.LoadTransition = true;
        if (AppBase.v.NoLobby && skipLobby != null)
        {
            startPos = skipLobby.Positions[0].pos.position;
            DevCam.transform.position = startPos;
        }

        // Dev cam pause at start?
        if (DevCam != null)
            DevCam.SetActive(true);
        yield return new WaitForSeconds(1f);
                        
        GameObject plr = (GameObject)Instantiate(PlatformPrefab, startPos, Quaternion.identity);
        if (plr.GetComponent<PhotonView>() != null)
        {
            plr.GetComponent<PlayerSync>().Real.SetActive(true);
            //plr.GetComponentInChildren<MainMenu>().SinglePlayer();
            //plr.GetComponent<PlayerSync>().Talents.SetActive(true);
            Destroy(plr.GetComponent<PlayerSync>().Dummy);
            Destroy(plr.GetComponent<PlayerSync>());
            Destroy(plr.GetComponent<PhotonView>());
        }
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (DevCam != null)
            DevCam.SetActive(false);

        ArrowEffects.EffectsDisabled = false;
        if(Settings.GetBool("ThirdPerson"))
        {
            while(!NVR_Player.isThirdPerson())
            {
                yield return true;
            }
        }

        if (startNode != null)
        {
            if (AppBase.v.NoLobby)
                skipLobby.ForceUse();
            else
                startNode.ForceUse();
        }
        yield return true;
        yield return true;
        if (AppBase.v.AutoStart && AutoStartInvoker != null)
            AutoStartInvoker.DoImpact();

        for(int i=0;i<6;++i)
            yield return new WaitForEndOfFrame();
        sf.LoadTransition = false; // Show game

        OnPlayerSpawned.Invoke();
    }
}
