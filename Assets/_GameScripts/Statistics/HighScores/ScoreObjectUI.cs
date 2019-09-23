using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreObjectUI : MonoBehaviour {

    public Text NameText;
    public Text IDText;
    public Text ScoreText;
    public Text Age;

    public string n;
    public int s;
    public int age;

	public void Setup(int idx, GenericScore score)
    {
        IDText.text = idx + ".";
        NameText.text = score.name;
        ScoreText.lineSpacing = 0.4f;
        //ScoreText.text = "<size=60>" + score.score + "</size>\n";
        ScoreText.text = score.score > 0 ? score.score.ToString() : "";
        if (null != Age)
        {
            if(score.score==0)
                Age.text = "";
            else if (score.age == 1)
                Age.text = "1 Day Ago";
            else if (score.age == 0)
                Age.text = "Today";
            else
                Age.text = score.age.ToString();
        }
        n = score.name;
        s = score.score;
        age = score.age;

        Debug.LogFormat("ScoreObjectUI: NameText:{0} IDText:{1} ScoreText:{2}", NameText.text, IDText.text, ScoreText.text);
    }
}
