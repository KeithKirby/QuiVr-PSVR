using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreObject : MonoBehaviour {

    public string Name;
    public int Score = -1;

    public Text scoreText;
    MutePlayer m;

	public void Init(string nme, int score = 0)
    {
        Name = nme;
        Score = score;
        scoreText.text = Name + " - " + Score;
        m = GetComponentInChildren<MutePlayer>();
        if (PhotonNetwork.inRoom)
        {
            m.Init(nme);
        }
        else
            m.gameObject.SetActive(false);
    }

    public void ChangeScore(int newVal)
    {
        if(newVal != Score)
        {
            Score = newVal;
            scoreText.text = Name + " - " + Score;
        }
        if (PhotonNetwork.inRoom)
            m.UpdateGraphic();
    }
}
