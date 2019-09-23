using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTypeManager : MonoBehaviour {

    public static GameTypeManager instance;

    public pvpmanager PvPMgr;
    public GameBase CanyonMgr;

    public GameObject PvPHolder;
    public GameObject CanyonHolder;

    PhotonView v;

    void Awake()
    {
        instance = this;
        v = GetComponent<PhotonView>();
    }

    [AdvancedInspector.Inspect]
    public void ChangeToPvP()
    {
        if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            ChangePvPNetwork();
            if (PhotonNetwork.inRoom)
                v.RPC("ChangePvPNetwork", PhotonTargets.Others);
        }
    }

    [AdvancedInspector.Inspect]
    public void ChangeToCanyon()
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            ChangeCanyonNetwork();
            if (PhotonNetwork.inRoom)
                v.RPC("ChangeCanyonNetwork", PhotonTargets.Others);
        }
    }

    [PunRPC]
    void ChangePvPNetwork()
    {
        PVPQButton.ToggleButton(false);
        CanyonMgr.PlayingBase = false;
        CanyonMgr.ClearGame();
        CanyonMgr.KillAll();
        CanyonHolder.SetActive(false);
        PvPHolder.SetActive(true);
        PvPMgr.PlayingPVP = true;
        PvPMgr.SetTeam();
        PvPMgr.StartGameDelayed(5);
    }

    [PunRPC]
    void ChangeCanyonNetwork()
    {
        PvPMgr.PlayingPVP = false;
        CanyonHolder.SetActive(true);
        PvPHolder.SetActive(false);
        CanyonMgr.PlayingBase = true;
        CanyonMgr.StartGame();
        PVPQButton.ToggleButton(true);
    }
}
