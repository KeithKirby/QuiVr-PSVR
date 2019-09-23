using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpectatorUI : MonoBehaviour {

    public GameObject Canvas;
    public Text CurrentDiff;
    public Text Stats;
    public GamePanel refPanel;

    void Start()
    {
        InvokeRepeating("UpdateText", 1f, 1f);
    }

    void UpdateText()
    {
        //Difficulty Text
        if(GameBase.instance != null)
        {
            int d = (int)GameBase.instance.Difficulty;
            if (d > 0)
                CurrentDiff.text = "Current Score: " + d;
            else
                CurrentDiff.text = "Waiting for game to start...";
        }
        int i = 0;
        foreach(var e in refPanel.Stats)
        {
            GamePanel.PlayerTuple[] tpls = GamePanel.CurrentValues((PlayerMetric)i);
            if(tpls.Length > 1 && PhotonNetwork.inRoom)
            {
                float aggrVal = 0;
                int num = 0;
                foreach(var t in tpls)
                {
                    if(t.name != PhotonNetwork.player.name)
                    {
                        aggrVal += t.value;
                        num++;
                    }
                }
                if(num > 0)
                {
                    float realVal = aggrVal / (float)num;
                    Stats.text += "Average " + e.StatName + ": " + (int)realVal + e.Suffix;
                }
            }
            i++;
        }
    }

}
