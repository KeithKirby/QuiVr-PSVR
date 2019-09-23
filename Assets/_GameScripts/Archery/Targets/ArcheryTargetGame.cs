using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ArcheryTargetGame : MonoBehaviour {

    public GameObject TargetPrefab;
    public int Waves;
    public GameObject PlayerHead;
    int curWave;
    int CurrentScore;
    int HighScore;
    bool ended;
    bool playing; 
    List<GameObject> TargetsOut;
    public Text scoreText;

    void Start()
    {
        TargetsOut = new List<GameObject>();
    }

	public void NewGame()
    {
        CurrentScore = 0;
        curWave = 1;
        ended = false;
        playing = true;
        SpawnWave(4);
    }

    void Update()
    {
        if(curWave >= Waves && !ended && playing)
        {
            ended = true;
            playing = false;
            Debug.Log("Final Score: " + CurrentScore);
        }
        else if(curWave < Waves && AllShot() && playing)
        {
            curWave++;
            SpawnWave(4);
        }
        scoreText.text = "Score: " + CurrentScore + "\n" + "High Score: " + HighScore;
    }

    void SpawnWave(int num)
    {
        TargetsOut = new List<GameObject>();
        for(int i=0; i<num; i++)
        {
            GameObject nT = (GameObject)Instantiate(TargetPrefab);
            TargetsOut.Add(nT);
            nT.GetComponentInChildren<ArcheryTarget>().Init(this);
            nT.transform.position = new Vector3(Random.Range(10f, -10f), Random.Range(2f, 9f), Random.Range(13f, 19f));
            nT.transform.LookAt(transform.position - (transform.position - PlayerHead.transform.position).normalized);
            Destroy(nT, 10);
        }
    }

	public void AddScore (int pts)
    {
        CurrentScore += pts;
        if(CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt("AlphaTargetHigh1", HighScore);
            PlayerPrefs.Save();
        }
	}

    bool AllShot()
    {
        foreach(var v in TargetsOut)
        {
            if(v != null)
            {
                ArcheryTarget t = v.GetComponentInChildren<ArcheryTarget>();
                if (t != null && !t.isBroken())
                    return false;
            }    
        }
        return true;
    }
}
