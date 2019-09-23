using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using U3D.SteamVR.UI;
public class VersionFeedback : MonoBehaviour {

    public Text Title;
    public Text Body;
    EMOpenCloseMotion motion;
    bool answered;
    bool currentAnswer;

    public Color NoSelected;

    public Button YesButton;
    public Button NoButton;

    void Awake()
    {
        motion = GetComponent<EMOpenCloseMotion>();
        YesButton.onClick.AddListener(ClickYes);
        NoButton.onClick.AddListener(ClickNo);
    }

    IEnumerator Start()
    {
        // dw - Disable version feedback
        yield return null;
        /*
        while (!SteamManager.Initialized || PlatformSetup.instance == null || !PlatformSetup.instance.serverchecked)
        {
            yield return true;
        }
        if(!AppBase.v.serverOnline)
        {
            motion.Open();
            Title.text = "Server Offline";
            Body.text = "The QuiVr Data Server is currently offline. You can still play, but no progress will be saved.\n\nVisit the Steam Page for more info.";
            YesButton.gameObject.SetActive(false);
            NoButton.gameObject.SetActive(false);
        }
        else
        {
            while(PlatformSetup.instance.Version == null || PlayerHead.instance == null)
            {
                yield return true;
            }
            if (PlatformSetup.instance.Version.ContainsKey("Question"))
            {
                foreach (var v in SteamVRPointer.AllPointers())
                {
                    v.SetEnabled(true);
                }
                string question = PlatformSetup.instance.Version.Get<string>("Question");
                motion.Open();
                Title.text = Application.version + " Feedback";
                Body.text = question;

                int i = PlayerPrefs.GetInt("QFB" + Application.version);
                if (i == 1)
                {
                    answered = true;
                    currentAnswer = true;
                    YesButton.GetComponent<Image>().color = Color.white;
                }
                else if (i == -1)
                {
                    answered = true;
                    currentAnswer = false;
                    NoButton.GetComponent<Image>().color = Color.white;
                }
            }
        }
        */
    }

    public void CheckAndOpen()
    {
        //StopCoroutine("Start");
        //StartCoroutine("Start");
    }

    public void ClickYes()
    {
        /*
        if(answered)
        {
            if (currentAnswer == true)
                return;
            else
                PlatformSetup.instance.IncrementVersionValue("NoVotes", -1);
        }
        answered = true;
        currentAnswer = true;

        YesButton.GetComponent<Image>().color = Color.white;
        NoButton.GetComponent<Image>().color = NoSelected;

        PlatformSetup.instance.IncrementVersionValue("YesVotes", 1);
        PlayerPrefs.SetInt("QFB" + Application.version, 1);
        PlayerPrefs.Save();
        */
    }

    public void ClickNo()
    {
        /*
        if(answered)
        {
            if (currentAnswer == false)
                return;
            else
                PlatformSetup.instance.IncrementVersionValue("YesVotes", -1);
        }
        answered = true;
        currentAnswer = false;

        NoButton.GetComponent<Image>().color = Color.white;
        YesButton.GetComponent<Image>().color = NoSelected;

        PlatformSetup.instance.IncrementVersionValue("NoVotes", 1);
        PlayerPrefs.SetInt("QFB" + Application.version, -1);
        PlayerPrefs.Save();
        */
    }
}
