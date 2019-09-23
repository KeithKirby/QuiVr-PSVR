using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoteDisplay : MonoBehaviour {

    Text txt;
    int motes;
    int m;
    void Awake()
    {
        txt = GetComponent<Text>();
    } 

	void Update()
    {
        if(Armory.instance != null)
        {
            m = Armory.instance.GetResource();
            if(motes != m)
            {
                motes = m;
                txt.text = motes + " " + I2.Loc.ScriptLocalization.Get("PlayerItems/Resource");
            }      
        }
    }
}
