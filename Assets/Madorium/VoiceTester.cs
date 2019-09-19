//using POpusCodec;
using UnityEngine;

public class VoiceTester : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Test opus encoder");
            //var oe = new OpusEncoder(POpusCodec.Enums.SamplingRate.Sampling48000, POpusCodec.Enums.Channels.Stereo, 44000, POpusCodec.Enums.OpusApplicationType.Voip, POpusCodec.Enums.Delay.Delay10ms);
            int i = 0;
        }

        if (Input.GetButtonDown("Fire3"))
        {
            //Debug.Log("Photon Disconnect");
            //PhotonVoiceNetwork.Disconnect();
        }
    }
}