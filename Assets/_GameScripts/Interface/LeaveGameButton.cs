using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LeaveGameButton : MonoBehaviour {

    Text txt;
    Button btn;
    public Text ConfirmText;
    public GameObject LeavePanel;


    void Awake()
    {
        btn = GetComponent<Button>();
        txt = GetComponentInChildren<Text>();
        if (GameBase.instance != null)
            GameBase.instance.OnStartDifficulty.AddListener(CheckStatus);
        CheckStatus();
    }
	
	public void CheckStatus()
    {
        if (!PhotonNetwork.inRoom && (GameBase.instance == null || GameBase.instance.Difficulty < 1 || !GameBase.instance.inGame))
        {
            txt.gameObject.SetActive(false);
            gameObject.SetActive(false);
            txt.text = "Quit Game";
        }
        else if (!PhotonNetwork.inRoom)
        {
            txt.gameObject.SetActive(true);
            gameObject.SetActive(true);
            txt.text = "End Game";
        }
        else
        {
            txt.gameObject.SetActive(true);
            gameObject.SetActive(true);
            txt.text = "Leave Game";
        }
    }

    public void Click()
    {
        LeavePanel.SetActive(true);
        if(PhotonNetwork.inRoom)
        {
            ConfirmText.text = "Are you sure you want to leave the game?";
        }
        else
        {
            if (GameBase.instance != null && (GameBase.instance.Difficulty < 1 || !GameBase.instance.inGame))
                ConfirmText.text = "Are you sure you want to Quit?";
            else if (GameBase.instance != null)
                ConfirmText.text = "Are you sure you want to end your game?";

        }
    }

    public void Confirm()
    {
        if(PhotonNetwork.inRoom)
            LeaveMultiplayer.Click();
        else
        {
            if(GameBase.instance != null)
            {
                if (GameBase.instance.Difficulty < 1 || !GameBase.instance.inGame)
                    Application.Quit();
                else
                {
                    int diff = (int)GameBase.instance.Difficulty;
                    bool altered = GameBase.instance.Altered;
                    GameBase.instance.Altered = true;
                    GameBase.instance.ForceEndGame();
                    if(!altered && !GameBase.instance.resumed)
                    {
                        PlayerPrefs.SetInt("GameInProgress", diff);
                        PlayerPrefs.Save();
                    }
                    ToggleMenu.instance.Toggle();

                }
            }
        }
        LeavePanel.SetActive(false);
    }

    public void Cancel()
    {
        LeavePanel.SetActive(false);
    }
}
