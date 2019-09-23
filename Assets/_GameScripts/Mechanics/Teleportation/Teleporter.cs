using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Teleporter : MonoBehaviour {

    [HideInInspector]
    public PlayerPos[] Positions
    {
        get
        {
            if(ppos == null)
                ppos = GetComponent<PlayerPositions>();
            return ppos.positions;
        }
        set
        {
            if (ppos == null)
                ppos = GetComponent<PlayerPositions>();
            ppos.positions = value;
        }
    }
    PlayerPositions ppos;

    public bool ForcePosition;
    public bool ForceRotation;
    public bool Unboostable;

    [System.Serializable]
    public class TeleporterEvent : UnityEvent<Teleporter> { }
    public TeleporterEvent OnTeleport;
    public TeleporterEvent OnLeave;
    public Color OnColor = Color.white;
    public Color OffColor = new Color(1, 1, 1, 0.1f);
    public bool Disabled;
    public bool ForceDisabled;
    public bool on = true;
    public bool Dynamic;
    ParticleSystem pt;
    LookAt look;
    SpriteRenderer[] sprites;

    void Awake() 
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        if(sprites.Length > 0)
        {
            GameObject ps = Instantiate(Resources.Load("Misc/TParticles") as GameObject);
            ps.transform.SetParent(transform);
            ps.transform.localPosition = Vector3.zero;
            ps.transform.localEulerAngles = Vector3.zero;
            ps.transform.localScale = Vector3.one;
            pt = ps.GetComponent<ParticleSystem>();
        }     
        look = GetComponent<LookAt>();
        if (look == null)
            look = GetComponentInChildren<LookAt>();
        if (look != null)
            look.visible = true;    
        //gameObject.layer = 13; //myArrows Layer
        if (Disabled)
            DisableTeleport();
        else
            EnableTeleport();
        //if (GetComponent<PlayerPositions>() != null)
           // Positions = GetComponent<PlayerPositions>().positions;
        if (GetComponent<ArrowImpact>() != null)
        {
            GetComponent<ArrowImpact>().OnHit.AddListener(TryTeleport);
            GetComponent<ArrowImpact>().DestroyArrow = true;
        }
        if (TeleporterManager.instance != null)
            TeleporterManager.instance.AddTP(this);
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    #region Activation

    void Start()
    {
        InvokeRepeating("CheckSettings", 1f, Random.Range(1.3f, 1.71f));   
    }

    void LateUpdate()
    {
        if (ForceDisabled && !Disabled)
            DisableTeleport();
        if(Disabled && sprites.Length > 0 && sprites[0] != null && sprites[0].enabled)
        {
            foreach(var v in sprites)
            {
                v.enabled = false;
            }
        }
        else if(!Disabled && on && look != null && !look.visible)
        {
            look.visible = true;
        }
    }

    public void ForceDisable(bool val)
    {
        ForceDisabled = val;
    }

    public void DisableTeleport()
    {
        Disabled = true;
        foreach (var s in sprites)
        {
            //s.color = OffColor;
            s.enabled = false;
        }
        foreach (var p in Positions)
        {
            if (p.inUse && p.userID == PhotonNetwork.player.ID)
            {
                TelePlayer.instance.TeleportClosest();
            }
        }
        if (look != null)
            look.visible = false;
        Hide();
    }

    public void EnableTeleport()
    {
        Disabled = false;
        foreach (var s in sprites)
        {
            if(s != null)
            {
                s.color = OnColor;
                s.enabled = true;
            }
        }
        if (look != null)
            look.visible = true;
        CheckSettings();
    }

    #endregion

    #region Mechanics

    public void TryTeleport(ArrowCollision e)
    {
        if (e.didDeflect || !e.isMine || Disabled || PlayerLife.dead() || TelePlayer.instance.currentNode == this)
            return;
        if (TeleporterManager.instance != null)
            TeleporterManager.instance.EnsureArea();
        if(TelePlayer.instance != null && !TelePlayer.instance.s_tryingTeleport)
        {
            if (TelePlayer.instance.currentNode == this)
            {
                Hide();
                return;
            }
            if (PhotonNetwork.inRoom)
            {
                TelePlayer.instance.s_tryingTeleport = true;
                TeleporterManager.instance.NetworkTP(this, PhotonNetwork.player.ID);
            }
            else
            {
                TelePlayer.instance.Teleport(this, getPosition(0));
                Positions[0].inUse = true;
                Positions[0].userID = PhotonNetwork.player.ID;
            }
        }
    }

    public void Release(int playerID)
    {
        if (playerID == PhotonNetwork.player.ID)
            Show();
        if (PhotonNetwork.inRoom)
            TeleporterManager.instance.NetworkRelease(this, playerID);//GetComponent<PhotonView>().RPC("ReleaseNetwork", PhotonTargets.AllViaServer, playerID);
        else
        {
            Positions[0].inUse = false;
            Positions[0].userID = 0;
        }
    }

    public Transform ReserveNextAvailable(int userID)
    {
        int ret = -1;
        for(int i=0; i<Positions.Length; i++)
        {
            if (!Positions[i].inUse && ret < 0)
                ret = i;
        }
        for (int i = 0; i < Positions.Length; i++)
        {
            if (Positions[i].userID == userID)
                ret = i;
        }
        if (ret >= 0)
        {
            Positions[ret].userID = userID;
            Positions[ret].inUse = true;
        }
        if (isFull())
            Hide();
        return ret < 0 ? null : Positions[ret].pos;
    }

    #endregion

    #region Visuals

    void CheckSettings()
    {
        if (pt != null)
        {
            var e = pt.emission;
            bool shouldBeOn = Settings.GetInt("EffectQuality", 1) >= 1 && !Disabled && (look == null || look.visible);    
            if (e.enabled != shouldBeOn)
                e.enabled = shouldBeOn;
        }
        if (TelePlayer.instance != null)
        {
            if (!Disabled && !on && TelePlayer.instance.currentNode != this)
                Show();
        }
    }

    public void Show()
    {
        on = true;
        Collider c = GetComponent<Collider>();
        if (c == null)
            c = GetComponentInChildren<Collider>();
        if (c != null)
            c.enabled = true;
        if(sprites != null)
        {
            foreach (var s in sprites)
            {
                if(s != null)
                    s.enabled = true;
            }
        }
        if (pt != null && Settings.GetInt("EffectQuality", 1) >= 1)
            pt.gameObject.SetActive(true);
        if(look != null)
            look.visible = true;
    }

    public void Hide()
    {
        on = false;
        Collider c = GetComponent<Collider>();
        if (c == null)
            c = GetComponentInChildren<Collider>();
        if (c != null)
            c.enabled = false;
        if(sprites != null)
        {
            foreach (var s in sprites)
            {
                if (s != null)
                    s.enabled = false;
            }
        }
        if (pt != null)
            pt.gameObject.SetActive(false);
        if(look != null)
            look.visible = false;
    }

    #endregion

    #region Networking

    void OnJoinedRoom()
    {
        foreach (var v in Positions)
        {
            v.inUse = false;
        }
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if(PhotonNetwork.isMasterClient)
        {
            string currentValues = "";
            for(int i=0; i<Positions.Length; i++)
            {
                if (Positions[i].inUse)
                    currentValues += 1 + "," + Positions[i].userID + ":";
                else
                    currentValues += 0 + "," + Positions[i].userID + ":";
            }
            if (currentValues.Length > 1 && TeleporterManager.instance != null)
                TeleporterManager.instance.NetSetValues(this, newPlayer, currentValues.Substring(0, currentValues.Length - 1));//GetComponent<PhotonView>().RPC("SetValues", newPlayer, currentValues.Substring(0, currentValues.Length-1));
        }
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (playerHasSpot(player.ID))
            {
                Release(player.ID);
            }
        }
    }

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(Disabled);
        }
        else
        {
            bool disabled = (bool)stream.ReceiveNext();
            if(disabled != Disabled)
            {
                if (disabled)
                    DisableTeleport();
                else
                    EnableTeleport();
            }
        }
    }
    */

    #endregion

    #region Utility

    [AdvancedInspector.Inspect]
    public void ForceUse()
    {
        if (TelePlayer.instance.currentNode == this)
            return;
        if (Disabled && TelePlayer.instance != null)
        {
            TelePlayer.instance.TeleportClosest();
            return;
        }
        ArrowCollision e = new ArrowCollision(Vector3.zero, 0, Vector3.zero, 0, null, false, Vector3.zero, true, false, 0, null, new EffectInstance[] { });
        TryTeleport(e);
    }

    public Transform getPosition(int id)
    {
        return Positions[id].pos;
    }

    public int MyPosition()
    {
        for(int i=0; i<Positions.Length; i++)
        {
            PlayerPos P = Positions[i];
            if (P.inUse)
            {
                if (!PhotonNetwork.inRoom)
                    return 0;
                else if (P.userID == PhotonNetwork.player.ID)
                    return i;
            }
        }
        return -1;
    }

    public bool isFull()
    {
        foreach (var v in Positions)
        {
            if (!v.inUse)
                return false;
        }
        return true;
    }

    public bool hasPlayers()
    {
        if (TelePlayer.instance.currentNode == this) //Single Player In Use Check
            return true;
        foreach (var v in Positions)
        {
            if (v.inUse == true)
                return true;
        }
        return false;
    }

    public int playerCount()
    {
        int i = 0;
        foreach (var v in Positions)
        {
            if (v.inUse)
                i++;
        }
        return i;
    }

    bool playerHasSpot(int pid)
    {
        foreach (var v in Positions)
        {
            if (v.userID == pid)
                return true;
        }
        return false;
    }

    public void ClearPositions()
    {
        foreach (var v in Positions)
        {
            v.inUse = false;
            v.userID = -1;
        }
    }

    public void KillIfUsing()
    {
        if (TelePlayer.instance.currentNode == this && !PlayerLife.dead())
            PlayerLife.Kill();
    }

    #endregion

    bool isQuitting;
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if(!isQuitting)
        {
            if (TeleporterManager.instance != null && TeleporterManager.instance.AllTeleporters != null)
                TeleporterManager.instance.AllTeleporters.Remove(this);
            Disabled = true;
            if (TelePlayer.instance != null)
                TelePlayer.instance.CheckTeleporter();
        } 
    }
}

public class teleportPosition
{
    public int owner;
    public bool inUse;
    public Transform Object;
}
