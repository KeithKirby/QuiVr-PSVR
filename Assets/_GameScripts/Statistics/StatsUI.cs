using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using Parse;
public class StatsUI : MonoBehaviour {

    public Text txt;
    public StatInfo[] Tracked; 

    void Awake()
    {
        if(txt == null)
            txt = GetComponent<Text>();
    }

	void Start()
    {
        InvokeRepeating("UpdateUI", 5f, 2f);
    }

    public void UpdateUI()
    {
        try
        {
            /*if (ParseUser.CurrentUser == null)
            {
                txt.text = "Couldn't fetch stats.";
                return;
            }*/
            string s = "";
            foreach (var v in Tracked)
            {
                string key = v.StatName;
                if (v.OptKey.Length > 0)
                    key = v.OptKey;
                s += v.StatName + ": " + Statistics.GetInt(key) + v.Suffix + "\n";
            }
            txt.text = s;
        }
        catch
        {
            txt.text = "User File Corrupted\n\nGo to:\n\n";
            txt.text += "C:/Users/USERNAME/AppData/LocalLow/Alvios, Inc_/QuiVr\n\n";
            txt.text += "And delete Parse.settings file";
        }
    }
}
