using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestingButton : MonoBehaviour {

    public Button[] disableTestButtons;
    Text myLabel;

	void Start ()
    {
        myLabel = GetComponentInChildren<Text>();
        int sid = 2;
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            sid = 1;
            myLabel.text = "Return to Keep";
        }
        GetComponent<ChangeScene>().Scene = "" + sid;
        if(sid == 1)
        {
            foreach(var v in disableTestButtons)
            {
                v.interactable = false;
            }
        }
	}

    public void SetupChange()
    {
        GetComponent<Button>().interactable = false;
        myLabel.text = "Loading Scene";
    }
	
}
