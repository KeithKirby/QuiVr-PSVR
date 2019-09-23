using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TimeStopper : MonoBehaviour {

    public float Cooldown;
    float cd;

    public float Duration;
    float durLeft;

    public bool slow;

    public Image DurSprite;
    public Image CooldownSprite;
    public GameObject ScreenEffect;

    public void TryToggleTime()
    {
        /*
        if (slow)
        {
            Debug.Log("Going to Normal Speed");
            ToggleTime();
        } 
        else if(cd <= 0 && (!PhotonNetwork.inRoom || PhotonNetwork.playerList.Length < 2))
        {
            Debug.Log("Slowing Time");
            ToggleTime();
            ToggleEffects(true);
        }
        */
    }

    public void ToggleTime()
    {
        /*
        if (Time.timeScale > 0.5f)
        {
            slow = true;
            durLeft = Duration;
            StopTime();
        } 
        else
        {
            slow = false;
            cd = ((Duration-durLeft)/Duration)*Cooldown;
            Time.timeScale = 1f;
            ToggleEffects(false);
        }
        */  
    }

    /*
    void Update()
    {
        if(slow)
        {
            if (durLeft > 0)
            {
                DurSprite.fillAmount = durLeft / Duration;
                durLeft -= Time.deltaTime / Time.timeScale;
            }
            else
            {
                ToggleTime();
                DurSprite.fillAmount = 0;
            }    
        }
        if (cd > 0)
        {
            cd -= Time.deltaTime;
            CooldownSprite.fillAmount = (Cooldown-cd)/Cooldown;
        }
        else
            CooldownSprite.fillAmount = 0;
            
    }

	public static void StopTime()
    {
        if(!PhotonNetwork.inRoom || PhotonNetwork.playerList.Length < 2)
        {
            Time.timeScale = 0.01f;
        }
    }

    void ToggleEffects(bool on)
    {
        DurSprite.fillAmount = 0;
        ScreenEffect.SetActive(on);
        if (on)
            ScreenEffect.GetComponent<AudioSource>().Play();
    }

    #region Resets
    void OnPhotonPlayerConnected()
    {
        slow = false;
        cd = 0;
        durLeft = 0;
        Time.timeScale = 1f;
        ToggleEffects(false);
    }

    void OnJoinedRoom()
    {
        slow = false;
        cd = 0;
        durLeft = 0;
        Time.timeScale = 1f;
        ToggleEffects(false);
    }

    void OnLeftRoom()
    {
        slow = false;
        cd = 0;
        durLeft = 0;
        Time.timeScale = 1f;
        ToggleEffects(false);
    }

    void OnDestroy()
    {
        Time.timeScale = 1;
        ToggleEffects(false);
    }

    #endregion
    */
}
