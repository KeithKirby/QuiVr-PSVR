using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Teleprompter : MonoBehaviour {

    [TextArea(3, 20)]
    public string PromptText;
    string[] Bites;
    public int cIndex = 0;
    public Text pText;
    public GameObject promptPanel;
    // Use this for initialization
    void Start ()
    {
        Bites = PromptText.Split('\n');
        pText.text = Bites[0];
	}
	
	// Update is called once per frame
	public void NextText ()
    {
        if(cIndex >= Bites.Length-1)
        {
            pText.text = "";
        }
        else
        {
            cIndex++;
            pText.text = Bites[cIndex];
        }
	}

    // Update is called once per frame
    public void PrevText()
    {
        if (cIndex <= 0)
        {
            pText.text = "";
        }
        else
        {
            cIndex--;
            pText.text = Bites[cIndex];
        }
    }

    public void TogglePanel()
    {
        promptPanel.SetActive(!promptPanel.activeSelf);
    }
}
