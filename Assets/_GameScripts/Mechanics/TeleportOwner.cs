using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class TeleportOwner : MonoBehaviour {

    public bool SpawnOnStart;
    public GameObject[] EnemyPrefabs;
    public Transform[] SpawnPoints;
    public Teleporter teleporter;
    public List<GameObject> EnemiesOut;
    public bool PlayersHaveStart;

    public UnityEvent OnAttacked;
    public UnityEvent OnEnemyTakeover;
    public UnityEvent OnPlayerTakeover;
    public UnityEvent OnReset;

    bool PlayerControlled;
    public bool attacked;
    bool setup;

    public bool Tutorial;

    //MageGroup mgr;

    public TeleportOwner twinTower;

    void Awake()
    {
        EnemiesOut = new List<GameObject>();
        //mgr = GetComponent<MageGroup>();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    void Start()
    {
        GameBase.instance.OnEndGame.AddListener(KillAll);
        GameBase.instance.OnClearGame.AddListener(KillAll);
        GameBase.instance.OnStartGame.AddListener(Reset);
    }

    public void Spawn()
    {
        int[] maxVal = new int[] { EnemyPrefabs.Length, SpawnPoints.Length };
        for (int i=0; i<Mathf.Min(maxVal); i++)
        {
            GameObject prefab = EnemyPrefabs[i];
            Transform spoint = SpawnPoints[i];
            if (!PhotonNetwork.inRoom)
            {
                GameObject newEnemy = (GameObject)Instantiate(prefab, spoint.position, spoint.rotation);
                newEnemy.GetComponent<Health>().OnAttacked.AddListener(Attacked);
                EnemiesOut.Add(newEnemy);
                CreatureManager.AddCreature(newEnemy.GetComponent<Creature>());
                setup = true;
            }
            else if(PhotonNetwork.isMasterClient)
            {
                GameObject newEnemy = PhotonNetwork.InstantiateSceneObject("NewEnemies/" + prefab.name, spoint.position, spoint.rotation, 0, null);
                GetComponent<PhotonView>().RPC("SetNetwork", PhotonTargets.All, newEnemy.GetComponent<PhotonView>().viewID);
            }
        }
    }

    public void Reset()
    {
        setup = false;
        attacked = false;
        KillAll();
        EnemiesOut = new List<GameObject>();
        if (!PlayersHaveStart)
            EnemyTakeover();
        else
            PlayerTakeover();
        OnReset.Invoke();
    }

    void Update()
    {
        CheckEnemies();
        for (int i = EnemiesOut.Count-1; i >= 0; i--)
        {
            GameObject g = EnemiesOut[i];
            if(g.GetComponent<Health>().isDead() || g == null)
            {
                EnemiesOut.RemoveAt(i);
            }
        }
        if(EnemiesOut.Count <= 0 && EnemyPrefabs.Length > 0 && !PlayerControlled && setup)
        {
            PlayerTakeover();
        }
    }

    void CheckEnemies()
    {
        for(int i=EnemiesOut.Count-1; i>=0; i--)
        {
            if (EnemiesOut[i] == null || EnemiesOut[i].GetComponent<Health>().isDead())
                EnemiesOut.RemoveAt(i);
        }
    }

    public void Attacked(GameObject enm)
    {
        if(PhotonNetwork.inRoom)
        {
            if(PhotonNetwork.playerList.Length > 2)
            {
                if (twinTower != null)
                    twinTower.Attacked();
            }
        }
        Attacked();
    }

    public void Attacked()
    {
        if(!attacked)
        {
            attacked = true;
            OnAttacked.Invoke();
        }
    }

    public void EnemyTakeover()
    {
        if(SpawnOnStart)
            Spawn();
        PlayerControlled = false;
        if(teleporter != null)
        {
            teleporter.DisableTeleport();
        }
        OnEnemyTakeover.Invoke();
    }

    public void PlayerTakeover()
    {
        PlayerControlled = true;
        if (teleporter != null)
        {
            teleporter.EnableTeleport();
        }
        OnPlayerTakeover.Invoke();
    }

    [PunRPC]
    void SetNetwork(int viewID)
    {
        var pv = PhotonView.Find(viewID);
        if (pv != null)
        {
            EnemiesOut.Add(pv.gameObject);
            CreatureManager.AddCreature(pv.GetComponent<Creature>());
            pv.GetComponent<Health>().OnAttacked.AddListener(Attacked);
            setup = true;
        }
    }

    void OnJoinedRoom()
    {
        setup = false;
        KillAll();
        if(PhotonNetwork.isMasterClient)
        {
            Reset();
        }
    }

    void OnPhotonPlayerConnected(PhotonPlayer p)
    {
        if(PhotonNetwork.isMasterClient)
        {
            foreach(var v in EnemiesOut)
            {
                if (v.GetComponent<PhotonView>() != null)
                    GetComponent<PhotonView>().RPC("SetNetwork", p, v.GetComponent<PhotonView>().viewID);
            }
        }
    }

    public void KillAll()
    {
        foreach(var v in EnemiesOut)
        {
            if (!v.GetComponent<Health>().isDead())
                v.GetComponent<Health>().Kill();
        }
    }
}
