using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MPButton : MonoBehaviour {

    public Text btnText;
    bool inMP;
    public int AllowedLevel = 2;

    void Start()
    {
        /*
        if (SceneManager.GetActiveScene().buildIndex != AllowedLevel)
            GetComponent<Button>().interactable = false;
        */
        if(PhotonNetwork.inRoom)
        {
            inMP = true;
            btnText.text = "Leave Room";
        }
    }

    public void Click()
    {
        if(inMP)
        {
            LeaveMultiplayer.Click();
        }
        else
        {
            if (MPSphere.instance != null)
                MPSphere.instance.Click();
            //else
                //JoinMultiplayer.Click();
        }
    }
}
