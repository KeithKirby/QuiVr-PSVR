using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour {

    public Text Display;
    public bool OpenAtStart;
    public AudioClip ValidClick;
    public AudioClip InvalidClick;

    public UnityEvent OnAbort;
    public StringEvent OnSet;

    public int CharLimit;
    public string Value;

    AudioSource src;
    EMOpenCloseMotion motion;
    bool open;
    HandAnim ha;
    [System.Serializable]
    public class StringEvent : UnityEvent<string> {}

    public Text ResponseText;

    void Awake()
    {
        Value = "";
        motion = GetComponentInParent<EMOpenCloseMotion>();
        src = GetComponent<AudioSource>();
    }

    public void AddValue(string val)
    {
        if(Value.Length < CharLimit)
        {
            Value += val;
            PlayValid();
            UpdateText();
            ResponseText.text = "";
        }
        else
        {
            PlayInvalid();
        }
    }

    public void Del()
    {
        if (Value.Length > 0)
        {
            Value = Value.Substring(0, Value.Length - 1);
            PlayValid();
            UpdateText();
            ResponseText.text = "";
        }
        else
        {
            PlayInvalid();
        }
    }

    public void Abort()
    {
        OnAbort.Invoke();
    }

    void UpdateText()
    {
        if (Value.Length > 1)
            Value = Value[0].ToString().ToUpper() + Value.Substring(1, Mathf.Min(Value.Length-1, CharLimit-1)).ToLower();
        else if (Value.Length > 0)
            Value = Value[0].ToString().ToUpper();
        Display.text = Value;
    }

    public void Close()
    {
        motion.Close();
        open = false;
        if (SteamVR_ControllerManager.freeHand != null)
        {
            SteamVR_ControllerManager.freeHand.GetComponentInChildren<HandAnim>().inMenu = 0;
        }
        ResponseText.text = "";
    }

    public void Open()
    {
        motion.Open();
        if (SteamVR_ControllerManager.freeHand != null)
            ha = SteamVR_ControllerManager.freeHand.GetComponentInChildren<HandAnim>();
        open = true;
        Value = "";
        Display.text = "";
        if (User.ArcadeUser != null)
        {
            //Value = User.ArcadeUser.Get<string>("Name");
            Value = User.ArcadeUser.ArcadeName;
            Display.text = Value;
        }
        ResponseText.text = "";
    }

    bool checking;
    public void TryAccept()
    {
        if(!checking)
        {
            checking = true;
            if(!User.hasSaved() && User.ArcadeUser != null)
                User.CheckUsername(Value, AcceptCallback);
            else
            {
                UpdateText();
                OnSet.Invoke(Value);
                ResponseText.text = "";
                Value = "";
            }    
        }
    }

    public void AcceptCallback(string response)
    {
        checking = false;
        if(response.Length <= 1)
        {
            UpdateText();
            OnSet.Invoke(Value);
            motion.Close();
            ResponseText.text = "";
            Value = "";
        }
        else
        {
            ResponseText.text = response;
        }
    }

    public void SetRandomName()
    {
        ResponseText.text = "";
        Value = User.RandomName();
        UpdateText();
    }

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
