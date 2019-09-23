using UnityEngine;
using System.Collections;

public class StartAreaHelp : MonoBehaviour {

    //public GameObject ShootText;
    public GameObject TeleportText;

    //public float ShootDelay;
    //public float TPDelay;

    bool counting;
    //public bool shotEnemy;
    public bool teleported;
    bool disabled;
    //float t = 0;

    public void Reset()
    {
        TeleportText.SetActive(false);
        //ShootText.SetActive(false);
        counting = false;
        //shotEnemy = false;
        counting = false;
        disabled = false;
       // t = 0;
    }

    public void Disabled()
    {
        disabled = true;
        TeleportText.SetActive(false);
        //ShootText.SetActive(false);
    }

    public void StartCounting()
    {
        if(!disabled)
            counting = true;
    }

    void Update()
    {
        if(counting && !disabled)
        {
            //t += Time.deltaTime;
            /*
            if (!shotEnemy)
            {
                if (t > ShootDelay && !ShootText.activeSelf)
                {
                    ShootText.SetActive(true);
                }
            }
            */
            if (!teleported)
            {
                if(!TeleportText.activeSelf) // && t > TPDelay)
                {
                    TeleportText.SetActive(true);
                }
            }
        }
    }

    public void DidTeleport()
    {
        teleported = true;
        TeleportText.SetActive(false);
        //t = 0;
    }

    public void DidKill()
    {
        //shotEnemy = true;
        //ShootText.SetActive(false);
        //t = 0;
    }
}
