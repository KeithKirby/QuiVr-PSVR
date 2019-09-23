using UnityEngine;
using System.Collections;
using VRTK;

public class PushToTalk : MonoBehaviour {

    public Sprite MicNoteSprite;
    public GameObject leftController;
    public GameObject rightController;

    public AudioClip On;
    public AudioClip Off;

    AudioSource src;

    public PhotonVoiceRecorder recorder;

	public IEnumerator Start()
    {
        src = GetComponent<AudioSource>();
        if(PhotonNetwork.inRoom)
        {
            StartCoroutine("SetupController", true);
            StartCoroutine("SetupController", false);
            yield return true;
            bool ptt = Settings.GetBool("PushToTalk");
            recorder.Transmit = !ptt;
            if (!ptt)
                Notification.Notify(new Note("Voice: Open Mic", "Your voice will be transmitted openly", "", MicNoteSprite, 6));
            else
                Notification.Notify(new Note("Voice: Push to Talk", "Grip Controller to Talk", "", MicNoteSprite, 6));
        }
    }

    IEnumerator SetupController(bool left)
    {
        GameObject ctrl = rightController;
        if (left)
            ctrl = leftController;
        while(!ctrl.activeSelf)
        {
            yield return true;
        }
        VRTK_ControllerEvents events = ctrl.GetComponent<VRTK_ControllerEvents>();
        events.GripPressed += GripPressed;
        events.GripReleased += GripReleased;
    }

    void Update()
    {
        if(NVR_Player.isThirdPerson())
        {
            if(PhotonNetwork.inRoom && Settings.GetBool("PushToTalk") && recorder != null)
            {
                if(Input.GetKeyDown(KeyCode.BackQuote))
                {
                    src.clip = On;
                    src.Play();
                    recorder.Transmit = true;
                }
                else if(Input.GetKeyUp(KeyCode.BackQuote))
                {
                    src.clip = Off;
                    src.Play();
                    recorder.Transmit = false;
                }
            }
        }
    }

    public void ToggleValue()
    {
        if(PhotonNetwork.inRoom && recorder != null)
            recorder.Transmit = !Settings.GetBool("PushToTalk");
    }

    void GripPressed(object o, ControllerInteractionEventArgs e)
    {
        if(Settings.GetBool("PushToTalk") && recorder != null && PhotonNetwork.inRoom)
        {
            if(Settings.GetBool("LeftHanded") && ((VRTK_ControllerEvents)o).gameObject == rightController)
            {
                src.clip = On;
                src.Play();
                recorder.Transmit = true;
            }
            else if (!Settings.GetBool("LeftHanded") && ((VRTK_ControllerEvents)o).gameObject == leftController)
            {
                src.clip = On;
                src.Play();
                recorder.Transmit = true;
            }
        }
    }

    void GripReleased(object o, ControllerInteractionEventArgs e)
    {
        if (Settings.GetBool("PushToTalk") && recorder != null && PhotonNetwork.inRoom)
        {
            if (Settings.GetBool("LeftHanded") && ((VRTK_ControllerEvents)o).gameObject == rightController)
            {
                src.clip = Off;
                src.Play();
                recorder.Transmit = false;
            }
            else if (!Settings.GetBool("LeftHanded") && ((VRTK_ControllerEvents)o).gameObject == leftController)
            {
                src.clip = Off;
                src.Play();
                recorder.Transmit = false;
            }
        }
    }


}
