using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserUI : MonoBehaviour {

    public EMOpenCloseMotion LoggedInPanel;
    public Keyboard UNKeyboard;
    public Keypad PinPad;
    public EMOpenCloseMotion LoginPanel;
    public Dropdown NameSelect;
    public Button ActionButton;
    public Text LoginResponse;
    public Text SaveResponse;

	public void PanelOpened()
    {
        if (User.ArcadeMode && User.ArcadeUser != null)
        {
            LoggedInPanel.Open();
            if (!User.hasSaved())
                ActionButton.GetComponentInChildren<Text>().text = "Save Profile";
            else
                ActionButton.GetComponentInChildren<Text>().text = "Delete Profile";
        }
        else if (LoggedInPanel.motionState == EMBaseMotion.MotionState.Open)
            LoggedInPanel.Close();
    }

    public void CreateNew()
    {
        User.NewArcadeUser();
    }

    public void LoginStart()
    {
        LoginResponse.text = "";
        LoginPanel.Open();
        UNKeyboard.Open();
        NameSelect.ClearOptions();
        string[] nms = Settings.GetStringArray("ArcadeNames");
        List<string> names = new List<string>();
        foreach(var v in nms)
        {
            names.Add(v);
        }
        NameSelect.AddOptions(names);
    }

    public void ActionPressed()
    {
        SaveResponse.text = "";
        if(!User.hasSaved() || User.ArcadeUser == null)
        {
            UNKeyboard.Open();
        }
        else
        {
            PinPad.Open();
        }
    }

    bool dropOpened;
    void Update()
    {
        if(NameSelect.transform.childCount == 4 && !dropOpened)
        {
            LoginResponse.text = "";
            dropOpened = true;
            UNKeyboard.Open();
        }
        else if(NameSelect.transform.childCount == 3)
            dropOpened = false;
    }

    public void PinEntered(string pin)
    {
        if(User.ArcadeUser == null) //Logging In
        {
            User.Login(NameSelect.captionText.text, pin, LoginCallback);
        }
        else if(User.ArcadeUser.Pin != "aaaa") //Deleting User
        {
            if(pin == User.ArcadeUser.Pin)
                User.DeleteUser(DeleteCallback, pin);
        }
        else //Saving new Profile
        {
            TrySave(pin);
        }
        PinPad.Close();
    }

    void DeleteCallback(bool success)
    {
        if(success)
        {
            LoggedInPanel.Close();
            LoginPanel.Close();
        }
        else
        {
            SaveResponse.text = "Delete Failed";
        }
    }

    public void TrySave(string pin)
    {
        if(User.ArcadeUser != null)
            //User.SaveUser(User.ArcadeUser.Get<string>("Name"), pin, SaveProfileCallback);
            User.SaveUser(User.ArcadeUser.ArcadeName, pin, SaveProfileCallback);
    }

    void SaveProfileCallback(string response)
    {
        if(response == "")
        {
            SaveResponse.text = "Saved Successfully";
            ActionButton.GetComponentInChildren<Text>().text = "Delete Profile";
        }
        else
        {
            SaveResponse.text = response;
        }
    }

    void LoginCallback(bool success)
    {
        if(success && User.ArcadeUser != null)
        {
            Debug.Log("User Logged In");
            LoginPanel.Close(true);
            LoggedInPanel.Open(true);
            ActionButton.GetComponentInChildren<Text>().text = "Delete Profile";
        }
        else
        {
            LoginResponse.text = "Login Failed\nUsername or Pin Incorrect";
        }
    }

    public void LoginSelected(int id)
    {
        if(id < NameSelect.options.Count)
        {
            Debug.Log("Name Selected: " + NameSelect.options[id].text);
            NameEntered(NameSelect.options[id].text);
        }
        else
        {
            Debug.Log("Invalid Name ID: " + id);
        }
    }

    public void NameEntered(string name)
    {
        if(User.ArcadeUser == null)
        {
            NameSelect.OnDeselect(new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current));
            NameSelect.captionText.text = name;
            UNKeyboard.Close();
        }
        else
        {
            //User.ArcadeUser["Name"] = name;
            User.ArcadeUser.ArcadeName = name;
            User.UpdateName();
            UNKeyboard.Close();
            SaveResponse.text = "Enter a Pin #";
            PinPad.Open();
        }
    }

    public void LogOut()
    {
        User.LeaveArcade();
    }
}
