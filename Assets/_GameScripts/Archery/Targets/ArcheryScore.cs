using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ArcheryScore : MonoBehaviour {

    public static ArcheryScore instance;

    public GameObject MyPoints;
    public GameObject OpponentPoints;

    public List<ArcheryScoreVal> Scores;

    public ScoreManager scrMgr;

    void Awake ()
    {
        instance = this;
        Scores = new List<ArcheryScoreVal>();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    public void Reset()
    {
        Scores = new List<ArcheryScoreVal>();
        Statistics.ClearCurrent();
        if (PhotonNetwork.inRoom)
        {
            GetComponent<PhotonView>().RPC("ResetNetwork", PhotonTargets.Others);
        }
    }

    [PunRPC]
    void ResetNetwork()
    {
        Scores = new List<ArcheryScoreVal>();
    }

    [PunRPC]
    public void ShowPoints(int val, int team, Vector3 pos)
    {
        GameObject copy;
        if(team == ArcheryGame.instance.GetTeam())
            copy = Instantiate(MyPoints);
        else
        {
            copy = Instantiate(OpponentPoints);
            pos += Vector3.up * 0.4f;
        }
        copy.GetComponent<ArcheryPoints>().ShowPoints(val, pos);
        Destroy(copy, 3f);
    }

    public void GivePoints(int pts, Vector3 pos)
    {
        int team = ArcheryGame.instance.GetTeam();
        ShowPoints(pts, team, pos);
        if (PhotonNetwork.inRoom)
            GetComponent<PhotonView>().RPC("ShowPoints", PhotonTargets.Others, pts, team, pos);
        GivePoints(pts);
    }

    public void GivePoints(int pts)
    {
        if(ArcheryGame.instance.inGame)
            ArcheryGame.instance.IsPlaying = true;
        if (PhotonNetwork.inRoom)
        {
            foreach (var v in Scores)
            {
                if (v.playerID == PhotonNetwork.player.ID)
                {
                    GetComponent<PhotonView>().RPC("UpdateScore", PhotonTargets.All, v.pts + pts, v.playerID, (string)PhotonNetwork.player.CustomProperties["DisplayName"]);
                    return;
                }
            }
            GetComponent<PhotonView>().RPC("UpdateScore", PhotonTargets.All, pts, PhotonNetwork.player.ID, (string)PhotonNetwork.player.CustomProperties["DisplayName"]);
        }
        else
        {
            if(Scores.Count > 0)
            {
                Scores[0].pts += pts;
            }
            else
            {
                ArcheryScoreVal s = new ArcheryScoreVal();
                s.pts = pts;
                s.pName = PlatformSetup.instance.DisplayName;
                Scores.Add(s);
            }
            if(scrMgr != null)
            {
                scrMgr.ChangeScore(Scores[0].pts);
            }
        }
    }

    [PunRPC]
    void UpdateScore(int score, int id, string plrName)
    {
        foreach (var v in Scores)
        {
            if (v.playerID == id)
            {
                v.pts = score;
                return;
            }
        }
        ArcheryScoreVal s = new ArcheryScoreVal();
        s.pts = score;
        s.playerID = id;
        s.pName = plrName;
        Scores.Add(s);
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer plr)
    {
        for(int i=Scores.Count-1; i >= 0; i--)
        {
            if (Scores[i].playerID == plr.ID)
                Scores.RemoveAt(i);
        }
    }
}

[System.Serializable]
public class ArcheryScoreVal
{
    public int pts;
    public int playerID;
    public string pName;
}
