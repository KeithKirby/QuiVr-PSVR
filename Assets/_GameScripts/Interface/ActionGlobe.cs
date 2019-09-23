using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ActionGlobe : MonoBehaviour {

    Text Label;
    MeshRenderer mesh;
    string baseTxt;

    void Awake()
    {
        Label = GetComponentInChildren<Text>();
        mesh = GetComponent<MeshRenderer>();
        if (Label != null)
            baseTxt = Label.text;
    }

	public void Deactivate(string info)
    {
        if(Label != null)
        {
            Label.text = baseTxt + "\n" + info;
            Label.color = Color.grey;
        }
        mesh.material.color = Color.black;
    }
}
