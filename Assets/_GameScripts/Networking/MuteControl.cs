using UnityEngine;
using System.Collections;

public class MuteControl : MonoBehaviour {

    public GameObject muteDisplay;
    public PlayerSync myPlr;
    public SpectatorSync mySpc;
    public Transform[] Controllers;
    public float muteDistance = 0.25f;
    public bool fullMute = false;
    public bool quickMute;

    bool muteVal;

    void Update()
    {
        if(PhotonNetwork.inRoom)
        {
            if (!fullMute && !quickMute && Dist(Controllers[0]) < muteDistance && Dist(Controllers[1]) < muteDistance)
            {
                quickMute = true;
                muteDisplay.SetActive(true);
            }
            else if (!fullMute && quickMute && Dist(Controllers[0]) >= muteDistance && Dist(Controllers[1]) >= muteDistance)
            {
                muteDisplay.SetActive(false);
                quickMute = false;
            }
            muteVal = quickMute;//(fullMute || quickMute);
            if(myPlr != null)
            {
                if (myPlr.selfMute != muteVal)
                {
                    myPlr.SetSelfMute(muteVal);
                }
            }
            else if(mySpc != null)
            {
                if (mySpc.selfMute != muteVal)
                {
                    mySpc.SetSelfMute(muteVal);
                }
            }        
        }
    }

    public bool toggleFullMute()
    {
        fullMute = !fullMute;
        return fullMute;
    }

	float Dist(Transform t)
    {
        if (t == null)
            return float.MaxValue;
        return Vector3.Distance(transform.position, t.position);
    }
}
