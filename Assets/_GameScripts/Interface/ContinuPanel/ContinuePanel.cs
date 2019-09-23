using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using U3D.SteamVR.UI;
using VRTK;

public class ContinuePanel : MonoBehaviour {

    public Text Title;
    public Text ContinueButton;
    public Text EndButton;
    public Text Responses; // X  |  ✓
    public static ContinuePanel instance;
    EMOpenCloseMotion motion;
    public int curVote;
    public List<int> Votes;
    bool open;

    public Button YesButton;
    public Button NoButton;

    void Awake()
    {
        instance = this;
        motion = GetComponent<EMOpenCloseMotion>();
        Votes = new List<int>();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    public static void Reset()
    {
        if (instance != null)
        {
            instance.transform.position = Vector3.zero;
            instance.curVote = 0;
            instance.open = false;
            instance.Votes = new List<int>();
            instance.Responses.text = "-";
        }
    }

    public static void Open(int diff=0)
    {
        Reset();
        if (instance != null)
        {
            if (diff < 1)
                diff = (int)GameBase.instance.Difficulty;
            instance.Title.text = "Enemy Defeated! (" + diff + ")";
            instance.open = true;
            instance.motion.Open(true);
            instance.Responses.text = "-";
            instance.UpdateVoteDisplay();
            instance.YesButton.interactable = false;
            instance.NoButton.interactable = false;
            instance.Invoke("EnableButtons", 1.5f);
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                instance.GetComponent<PhotonView>().RPC("OpenNetwork", PhotonTargets.Others, diff);

            //Set positioning
            if(PlayerHead.instance != null)
            {
                instance.transform.position = PlayerHead.instance.transform.position + (PlayerHead.instance.transform.TransformDirection(Vector3.forward) * 2f);
                instance.transform.position = new Vector3(instance.transform.position.x, PlayerHead.instance.transform.position.y, instance.transform.position.z);
                instance.transform.LookAt(instance.transform.position + (instance.transform.position-PlayerHead.instance.transform.position).normalized);
                instance.currentOffset = instance.transform.position - LocalPlayer.instance.PlayArea.transform.position;
            }
            GameObject mainPointer = SteamVR_ControllerManager.freeHand;
            if (mainPointer != null)
            {
                mainPointer.GetComponent<VRTK_InteractTouch>().enabled = false;
                mainPointer.GetComponent<VRTK_InteractGrab>().enabled = false;
                mainPointer.GetComponent<VRTK_InteractUse>().enabled = false;
            }
        }
    }

    public void EnableButtons()
    {
        instance.YesButton.interactable = true;
        instance.NoButton.interactable = true;
    }

    public static void Close()
    {
        if(instance != null)
        {
            instance.motion.Close(true);
            instance.open = false;
            instance.CancelInvoke();
            instance.Invoke("DoReset", 1f);
            if (VRTK_UIPointer.Pointers == null)
                VRTK_UIPointer.Pointers = new System.Collections.Generic.List<VRTK_UIPointer>();
            foreach (var v in VRTK_UIPointer.Pointers)
            {
                v.On = false;
            }
            GameObject mainPointer = SteamVR_ControllerManager.freeHand;
            if (mainPointer != null)
            {
                mainPointer.GetComponent<VRTK_InteractTouch>().enabled = true;
                mainPointer.GetComponent<VRTK_InteractGrab>().enabled = true;
                mainPointer.GetComponent<VRTK_InteractUse>().enabled = true;
            }
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                instance.GetComponent<PhotonView>().RPC("CloseNetwork", PhotonTargets.Others);
        }
    }

    float t = 0;
    float cd = 0;
    Vector3 currentOffset;
    void Update()
    {
        if (open && LocalPlayer.instance != null)
        {
            transform.position = LocalPlayer.instance.PlayArea.position + currentOffset;
            t += Time.unscaledDeltaTime;
            cd += Time.unscaledDeltaTime;
            if (t > 0.5f)
            {
                t = 0;
                instance.Title.text = "Enemy Defeated! (" + (int)GameBase.instance.Difficulty + ")";
            }
            if (curVote == 0)
                ContinueButton.text = "Continue (" + (int)(60 - cd) + ")";
            else
                ContinueButton.text = "Continue";
            if (cd > 60f && curVote == 0)
            {
                VoteYes();
            }
            if(cd >= 70 && PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                GetComponent<PhotonView>().RPC("CompleteVote", PhotonTargets.AllViaServer, 1);
            EnablePointers();  
            if(NVR_Player.isThirdPerson())
            {
                ContinueButton.text += "(Y)";
                if (EndButton.text != null)
                    EndButton.text = "End Game (N)";
                if (Input.GetKeyDown(KeyCode.Y))
                    VoteYes();
                else if (Input.GetKeyDown(KeyCode.N))
                    VoteNo();
            }
        }
        else
            cd = 0;
    }

    void EnablePointers()
    {
        if (VRTK_UIPointer.Pointers == null)
            VRTK_UIPointer.Pointers = new System.Collections.Generic.List<VRTK_UIPointer>();
        foreach (var v in VRTK_UIPointer.Pointers)
        {
            v.On = true;
        }
    }

    void DoReset()
    {
        Reset();
    }

    [AdvancedInspector.Inspect]
    void TestOpen()
    {
        Open();
    }

    [AdvancedInspector.Inspect]
    void TestClose()
    {
        Close();
    }

    [PunRPC]
    void OpenNetwork(int diff)
    {
        Open(diff);
    }

    [PunRPC]
    void CloseNetwork()
    {
        Close();
    }

    #region Voting

    [AdvancedInspector.Inspect]
    public void VoteYes()
    {
        if (curVote == 1)
            return;
        int prevVote = curVote;
        curVote = 1;
        ChangeVote(1, prevVote);
        if (PhotonNetwork.inRoom)
            GetComponent<PhotonView>().RPC("ChangeVote", PhotonTargets.Others, curVote, prevVote);
        DoUpdate();
    }

    [AdvancedInspector.Inspect]
    public void VoteNo()
    {
        if (curVote == -1)
            return;
        int prevVote = curVote;
        curVote = -1;
        ChangeVote(-1, prevVote);
        if (PhotonNetwork.inRoom)
            GetComponent<PhotonView>().RPC("ChangeVote", PhotonTargets.Others, curVote, prevVote);
    }

    [PunRPC]
    void ChangeVote(int vote, int prev)
    {
        Votes.Remove(prev);
        Votes.Add(vote);
        DoUpdate();
    }

    void CountVotes()
    {
        if(PhotonNetwork.inRoom && PhotonNetwork.playerList.Length > 1 && PhotonNetwork.isMasterClient && open)
        {
            int majority = Mathf.FloorToInt((PhotonNetwork.playerList.Length/2f));
            int NoCount = 0;
            int YesCount = 0;
            foreach(var v in Votes)
            {
                if (v == -1)
                    NoCount++;
                else if (v == 1)
                    YesCount++;
            }
            if (NoCount >= majority)
                GetComponent<PhotonView>().RPC("CompleteVote", PhotonTargets.AllViaServer, -1);
            else if (YesCount > majority)
                GetComponent<PhotonView>().RPC("CompleteVote", PhotonTargets.AllViaServer, 1);
        }
        else if((!PhotonNetwork.inRoom || PhotonNetwork.playerList.Length < 2) && Votes.Count > 0 && open)
        {
            CompleteVote(Votes[0]);
        }
    }

    [PunRPC]
    void CompleteVote(int val)
    {
        if(open)
        {
            if (val == 1) //Continue
            {
                EnemyStream.completed = false;
                GameBase.instance.CurrentStream.BeginStream();
            }
            else if (val == -1) //Return to Keep
            {
                if (GameBase.instance)
                    GameBase.instance.EndGame();
            }
            Close();
        }
    }

    #endregion

    #region Network

    void OnJoinedRoom()
    {
        Close();
    }

    void OnLeftRoom()
    {
        Close();
    }

    void OnPhotonPlayerConnected(PhotonPlayer plr)
    {
        if(open && PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("OpenNetwork", plr, GameBase.instance.Difficulty);
        }
    }

    #endregion

    void DoUpdate()
    {
        CountVotes();
        UpdateVoteDisplay();
    }

    void UpdateVoteDisplay(int dummyTotal = 0)
    {
        if (!PhotonNetwork.inRoom)
            return;
        else
        {
            bool shownMine = false;
            int total = PhotonNetwork.playerList.Length;
            string s = "";
            int i = 0;
            foreach(var v in Votes)
            {
                i++;
                if (v == 1)
                {
                    if (curVote == 1 && !shownMine)
                    {
                        shownMine = true;
                        s += "<color=white>✓</color>";
                    } 
                    else
                        s += "✓";
                }
                else if (v == -1)
                {
                    if (curVote == -1 && !shownMine)
                    {
                        shownMine = true;
                        s += "<color=white>X</color>";
                    }
                    else
                        s += "X";
                }
                else
                    s += "-";
                s += "  |  ";
            }
            for(int x = i; x<total; x++)
            {
                s += "-  |  ";
            }
            s = s.Substring(0, s.Length - 5);
            Responses.text = s;
        }
    }
}
