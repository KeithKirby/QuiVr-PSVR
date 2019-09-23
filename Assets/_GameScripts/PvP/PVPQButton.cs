using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Parse;

public class PVPQButton : MonoBehaviour {

    public Text buttonText;
    public Sprite SearchSprite;
    public static PVPQButton instance;
    Button button;

    bool banned;

    void Awake()
    {
        instance = this;
        button = GetComponent<Button>();
        //if (ParseUser.CurrentUser != null && ParseUser.CurrentUser.ContainsKey("CheatCaught"))
            //banned = ParseUser.CurrentUser.Get<int>("CheatCaught") >= 2;
    }

    public static void ToggleButton(bool val)
    {
        if (instance != null)
            instance.button.interactable = val;
    }

    [AdvancedInspector.Inspect]
    public void Click()
    {
        if (banned)
            return;
        bool valid = PvPMatchmaking.ToggleWantMatch(!PvPMatchmaking.LookingForMatch);
        if (!valid)
            return;
        if (PvPMatchmaking.LookingForMatch)
        {
            buttonText.text = "Leave Queue";
            Notification.Notify(new Note("PvP Queue", "Searching for Opponent...", "", SearchSprite));
            CheckELO();
        }
        else
        {
            buttonText.text = "PvP Queue";
            Notification.Notify(new Note("PvP Queue", "Stopped Searching for PvP Match.", "", SearchSprite));
        }
    }

    void CheckELO()
    {
        /* // dw - remove parseuser
        if(ParseUser.CurrentUser != null && !ParseUser.CurrentUser.ContainsKey("MMR"))
        {
            ParseUser.CurrentUser.Add("MMR", 800);
            ParseUser.CurrentUser.SaveAsync();
        }*/
    }
}
