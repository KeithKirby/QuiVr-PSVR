using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResumeTarget : MonoBehaviour {

    public GameObject Target;
    public Text ResumeText;
    public DifficultyBooster boost;
    int resumeDiff;

    void Start()
    {
        CheckResume();
    }

    public void CheckResume()
    {
        if(!PhotonNetwork.inRoom)
        {
            resumeDiff = PlayerPrefs.GetInt("GameInProgress", 0);
            if(resumeDiff > 200)
            {
                Show();
                ResumeText.text = "Resume Game (Gate " + (int)(resumeDiff / 100) + ")";
                return;
            }
        }
        resumeDiff = 0;
        Hide();
    }

    public void Hit()
    {
        ResumeText.text = "";
        if (!PhotonNetwork.inRoom && resumeDiff > 200)
        {
            GameBase.instance.resumed = true;
            PlayerPrefs.SetInt("GameInProgress", -1);
            PlayerPrefs.Save();
            boost.Boost(resumeDiff / 100);
        }
    }

    public void Hide()
    {
        Target.SetActive(false);
        ResumeText.text = "";
    }

    void Show()
    {
        Target.SetActive(true);
        GetComponent<ArcheryTarget>().Reset();
    }
}
