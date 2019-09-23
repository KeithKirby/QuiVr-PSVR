using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Scoreboard : MonoBehaviour {

    public Text scb;
    string sc;
    public int spScore = 0;
    public int currency;
    int lastScore = -1;

    public GameObject ScorePrefab;

    public List<ScoreObject> Scores;

    void Start()
    {
        Scores = new List<ScoreObject>();
    }

    void Update ()
    {
        sc = "-- Score Board --";
        playerScore[] scores = AllScores();
        RemoveOld(scores);
        AddNewScores(scores);
        UpdateScores(scores);
        Sort();
    }

    void Sort()
    {
        Scores.Sort(
            delegate (ScoreObject p1, ScoreObject p2)
            {
                int compareDate = p2.Score.CompareTo(p1.Score);
                if (compareDate == 0)
                {
                    return p2.Name.CompareTo(p1.Name);
                }
                return compareDate;
            }
        );
        for(int i=0; i<Scores.Count; i++)
        {
            Scores[i].transform.localPosition = new Vector3(0, -125 + (-100*i), 0);
        }
    }

    void UpdateScores(playerScore[] scrs)
    {
        foreach(var s in scrs)
        {
            foreach(var v in Scores)
            {
                if(v.Name == s.username)
                {
                    v.ChangeScore(s.score());
                }
            }
        }
    }

    void AddNewScores(playerScore[] scores)
    {
        if (scores.Length == Scores.Count)
            return;
        bool[] has = new bool[scores.Length];
        ExtensionMethods.Populate<bool>(has, false);
        for (int i = 0; i < Scores.Count; i++)
        {
            for (int j=0; j<scores.Length; j++)
            {
                if (scores[j].username == Scores[i].Name)
                    has[j] = true;
            }
        }
        for (int i = 0; i < has.Length; i++)
        {
            if (!has[i])
                AddNew(scores[i]);
        }
    }

    void AddNew(playerScore score)
    {
        GameObject newScore = (GameObject)Instantiate(ScorePrefab);
        newScore.transform.SetParent(transform);
        newScore.transform.localScale = Vector3.one;
        newScore.transform.localEulerAngles = Vector3.zero;
        newScore.transform.position = Vector3.down * -125;
        newScore.GetComponent<ScoreObject>().Init(score.username, score.score());
        Scores.Add(newScore.GetComponent<ScoreObject>());
    }

    void RemoveOld(playerScore[] scores)
    {
        if (scores.Length == Scores.Count)
            return;
        bool[] has = new bool[Scores.Count];
        ExtensionMethods.Populate<bool>(has, false);
        for (int i=0; i<Scores.Count; i++)
        {
            foreach(var v in scores)
            {
                if (v.username == Scores[i].Name)
                    has[i] = true;
            }
        }
        for(int i=has.Length-1; i>= 0; i--)
        {
            if(!has[i])
            {
                Destroy(Scores[i].gameObject);
                Scores.RemoveAt(i);
            }
        }
    }

    public int currentScore()
    {
        return spScore;
    }

    public int getCurrency()
    {
        return currency;
    }

    public playerScore[] AllScores()
    {
        if (PhotonNetwork.inRoom)
        {
            PhotonPlayer[] players = PhotonNetwork.playerList;
            List<playerScore> scores = new List<playerScore>();
            foreach (var v in players)
            {
                int score = 0;
                if (v != null && v.customProperties["score"] != null)
                    int.TryParse(v.customProperties["score"].ToString(), out score);
                playerScore s = new playerScore(v.name, score);
                scores.Add(s);
            }
            scores.Sort((x, y) => y.score().CompareTo(x.score())); //Now inverted?
            return scores.ToArray();
        }
        else
            return new playerScore[] {new playerScore(PhotonNetwork.player.name, spScore)};
    }

    public int lowestOther()
    {
        if(PhotonNetwork.inRoom && PhotonNetwork.playerList.Length > 1)
        {
            playerScore[] scores = AllScores();
            int lowest = int.MaxValue;
            foreach(var v in scores)
            {
                if(v.score() < lowest && v.username != PhotonNetwork.player.name)
                {
                    lowest = v.score();
                }
            }
            return lowest;
        }
        return 0;
    }

    [PunRPC]
    void ResetScoresNetwork()
    {
        if(PhotonNetwork.inRoom)
        {
            ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
            PlayerCustomProps["score"] = 0;
            PhotonNetwork.player.SetCustomProperties(PlayerCustomProps);
        }
        ClearScoreObjects();
        currency = 0;
        spScore = 0;
        lastScore = -1;
    }

    public void ResetScores()
    {
        if(PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("ResetScoresNetwork", PhotonTargets.All);
        }
        ClearScoreObjects();
        currency = 0;
        spScore = 0;
        lastScore = -1;
    }

    void ClearScoreObjects()
    {
        for(int i= Scores.Count-1; i >= 0; i--)
        {
            Destroy(Scores[i].gameObject);
            Scores.RemoveAt(i);
        }
    }


    void OnJoinedRoom()
    {
        ResetScoresNetwork();
        ChangeScore(lowestOther());
    }

    public void ChanceCurrency(int num)
    {
        currency += num;
    }

    public void ChangeScore(int pts)
    {
        if(PhotonNetwork.inRoom)
        {
            int curScore = spScore;
            ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
            PlayerCustomProps["score"] = spScore + pts;
            PhotonNetwork.player.SetCustomProperties(PlayerCustomProps);
        }
        currency += pts;
        spScore += pts;
    }  
}

public class playerScore
{
    public int scr;
    public string username;

    public playerScore(string n, int score)
    {
        scr = score;
        username = n;
    }

    public int score()
    {
        return scr;
    }
}
