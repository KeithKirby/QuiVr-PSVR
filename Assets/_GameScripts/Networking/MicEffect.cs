using UnityEngine;
using System.Collections;

public class MicEffect : MonoBehaviour {

    public PhotonVoiceSpeaker speaker;
    public AudioSource src;
    public GameObject SpeechEffect;

    bool _speakerActive = false;
    void Update()
    {
        if(speaker != null)
        {
            bool speakerActive = speaker != null && speaker.IsPlaying && PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined && src.volume > 0.1f;
            if (_speakerActive != speakerActive)
            {
                _speakerActive = speakerActive;
                SpeechEffect.SetActive(_speakerActive);
                //if (_speakerActive)
                    //Debug.Log("Started speaking");
                //else
                    //Debug.Log("Stopped speaking");
            }
        }
    }
}
