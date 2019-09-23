using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTesting : MonoBehaviour {

    public string Username;
    public string Pin;
    
    [BitStrap.Button]
	public void GenerateName()
    {
        Debug.Log(User.RandomName());
    }

    [BitStrap.Button]
    public void NewArcadeAcc()
    {
        User.NewArcadeUser();
    }

    [BitStrap.Button]
    public void LeaveArcade()
    {
        User.LeaveArcade();
    }

    [BitStrap.Button]
    public void SaveUser()
    {
        User.SaveUser(Username, Pin, SaveCallback);
    }

    [BitStrap.Button]
    public void LoginUser()
    {
        User.Login(Username, Pin, LoginCallback);
    }

    void LoginCallback(bool success)
    {
        Debug.Log("Logged into Arcade Account: " + success);
    }

    void SaveCallback(string mssg)
    {
        Debug.Log("Save Arcade Callback: " + mssg);
    }

    [BitStrap.Button]
    public void ListSaveNames()
    {
        if (PlayerPrefs.HasKey("ArcadeNames"))
            Debug.Log(string.Join(",",Settings.GetStringArray("ArcadeNames")));
        else
            Debug.Log("No Arcade Users saved");
    }

    [BitStrap.Button]
    public void ClearSavedNames()
    {
        
    }
}
