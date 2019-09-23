using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using VRTK;

public class Keypad : MonoBehaviour {

    public Text Display;
    public bool OpenAtStart;
    public AudioClip ValidClick;
    public AudioClip InvalidClick;

    public UnityEvent OnAbort;
    public StringEvent OnSet;

    public GameObject SecureCam;

    int Length = 4;
    List<int> Values;
    AudioSource src;
    EMOpenCloseMotion motion;

    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }

    void Awake()
    {
        Values = new List<int>();
        motion = GetComponentInParent<EMOpenCloseMotion>();
        src = GetComponent<AudioSource>();
    }

    IEnumerator Start()
    {
        UpdateText();
        while(SteamVR_ControllerManager.freeHand == null)
        {
            yield return true;
        }
        if(OpenAtStart)
            Open();
    }

    public void AddValue(int val)
    {
        if (Values.Count < Length)
        {
            Values.Add(val);
            UpdateText();
            PlayValid();
        }
        else
            PlayInvalid();

    }

    public void Del()
    {
        if (Values.Count > 0)
        {
            Values.RemoveAt(Values.Count - 1);
            PlayValid();
            UpdateText();
        }
        else
            PlayInvalid(); 
    }

    public void TryAccept()
    {
        if (Values.Count == Length)
        {
            string pin = GetPin();
            Debug.Log("Valid PIN: ****");
            PlayValid();
            OnSet.Invoke(pin);
            Values = new List<int>();
            UpdateText();
        }
        else
        {
            PlayInvalid();
            Debug.Log("Invalid PIN");
        }
    }

    string GetPin()
    {
        string s = "";
        foreach(var v in Values)
        {
            s += v;
        }
        return s;
    }

    void UpdateText()
    {
        string s = "";
        int i = 0;
        for(i=0; i<Values.Count; i++)
        {
            if (i < Values.Count - 1)
                s += "* ";
            else
                s += Values[i] + " ";
        }
        for(int j=i; j<Length; j++)
        {
            s += "_ ";
        }
        s = s.Substring(0, s.Length - 1);
        Display.text = s;
    }

    public void Abort()
    {
        OnAbort.Invoke();
    }

    public void Close()
    {
        Display.text = "* * * *";
        SecureCam.SetActive(false);
        motion.Close();
        open = false;
        if(SteamVR_ControllerManager.freeHand != null)
        {
            SteamVR_ControllerManager.freeHand.GetComponentInChildren<HandAnim>().inMenu = 0;
        }
    }
   
    public void Open()
    {
        Values = new List<int>();
        UpdateText();
        SecureCam.SetActive(true);
        motion.Open();
        if(SteamVR_ControllerManager.freeHand != null)
            ha = SteamVR_ControllerManager.freeHand.GetComponentInChildren<HandAnim>();
        open = true;
    }

    bool open;
    HandAnim ha;
    void Update()
    {
        if (open && ha != null)
        {
            ha.inMenu = 1;
        }
    }

    void PlayValid()
    {
        src.Stop();
        src.clip = ValidClick;
        src.Play();
    }

    void PlayInvalid()
    {
        src.Stop();
        src.clip = InvalidClick;
        src.Play();
    }
}
