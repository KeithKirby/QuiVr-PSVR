using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Text))]
[RequireComponent(typeof(EMOpenCloseMotion))]
public class TextFlash : MonoBehaviour {

    Text t;
    EMOpenCloseMotion motion;
    public float fadeDelay;
    PhotonView v;

	// Use this for initialization
	void Start () {
        v = GetComponent<PhotonView>();
        t = GetComponent<Text>();
        motion = GetComponent<EMOpenCloseMotion>();
	}

    public void Flash(string newText, float dur)
    {
        t.text = newText;
        motion.Open(true);
        StopCoroutine("Fade");
        StartCoroutine("Fade", dur);
    }

    public void Flash(string newText)
    {
        t.text = newText;
        motion.Open(true);
    }

    IEnumerator Fade(float dur)
    {
        yield return new WaitForSeconds(dur);
        motion.Close();
        StopCoroutine("Fade");
    }
}
