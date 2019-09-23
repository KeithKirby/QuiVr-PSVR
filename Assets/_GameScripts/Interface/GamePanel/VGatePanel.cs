using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VGatePanel : MonoBehaviour {

    public Text Total;
    public Text Plr1;
    public Text Plr2;

    float t = 0;
    void Update()
    {
        t += Time.unscaledDeltaTime;
        if (t > 0.5f)
        {
            t = 0;
            UpdateText();
        }
    }

    void UpdateText()
    {
        if (GamePanel.instance == null)
            return;
        GamePanel.instance.curMetric = PlayerMetric.EnmKilled;
        GamePanel.PlayerTuple[] tpls = GamePanel.CurrentValues(PlayerMetric.EnmKilled);
        int total = 0;
        if (tpls.Length > 0)
        {
            Plr1.text = tpls[0].name + ": " + tpls[0].value;
            total = (int)tpls[0].value;
            if (tpls.Length > 1)
            {
                Plr2.text = tpls[1].name + ": " + tpls[1].value;
                total += (int)tpls[1].value;
            }
            else
                Plr2.text = "";
        }
        else
        {
            Plr1.text = "";
            Plr2.text = "";
        }
        Total.text = "Total: " + total;
    }


}
