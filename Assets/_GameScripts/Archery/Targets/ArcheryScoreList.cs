using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArcheryScoreList : MonoBehaviour {
    public Text ScoreText;
    public Text InfoText;
    
    void Update()
    {
        if(ArcheryGame.instance != null)
        {
            if(ArcheryGame.instance.inGame)
            {
                int totalSeconds = ArcheryGame.instance.GetTimer();

                System.TimeSpan span = new System.TimeSpan(0, 0, totalSeconds);
                string time = string.Format("{0}:{1:00}", (int)span.TotalMinutes, span.Seconds);
                InfoText.text = "Round - " + time;
            }
            else if(InfoText.text != "Archery Scores")
            {
                InfoText.text = "Archery Scores";
            }
            if (ArcheryScore.instance != null && ArcheryScore.instance.Scores.Count > 0)
            {
                ArcheryScore.instance.Scores.Sort((x, y) =>
                {
                    return y.pts.CompareTo(x.pts);
                });
                string s = "";
                foreach (var v in ArcheryScore.instance.Scores)
                {
                    s += v.pName + ":..." + v.pts + "\n\n";
                }
                ScoreText.text = s.Substring(0, s.Length - 2);
            }
            else if(ScoreText.text.Length > 1)
                ScoreText.text = "";
        }
        else if(InfoText.text.Length > 1)
        {
            ScoreText.text = "";
            InfoText.text = "";
        }
    }
}
