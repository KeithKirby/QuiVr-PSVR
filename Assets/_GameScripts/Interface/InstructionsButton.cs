using UnityEngine;
using System.Collections;

public class InstructionsButton : MonoBehaviour {

    EMOpenCloseMotion instr;

	// Use this for initialization
	void Start () {
        GameObject g = GameObject.FindGameObjectWithTag("Instructions");
        if (g != null)
            instr = g.GetComponent<EMOpenCloseMotion>();

    }
	
	// Update is called once per frame
	public void Click () {
        if (instr != null)
            instr.Open(true);
	}
}
