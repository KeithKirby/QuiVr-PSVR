using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    public float PSVRMinColor = 0.005f;
    public bool AllowMove = true;

    public float StartFadeTime = 2.0f;

	// Use this for initialization
	void Start () {
#if UNITY_PS4 && !UNITY_EDITOR // Output error in editor but works on console
        UnityEngine.PS4.VR.PlayStationVRSettings.minOutputColor = new Color(PSVRMinColor, PSVRMinColor, PSVRMinColor);
        //Debug.LogFormat("Set minOutputColor({0})", PSVRMinColor);
#endif

        GameGlobeData.enableMovement = AllowMove;
        var fadeCon = RenderMode.GetInst();
        fadeCon.LevelStartFade = true;
        fadeCon.InstantTransition();
        StartCoroutine(FadeInDelay());
	}
	
	// Update is called once per frame
	IEnumerator FadeInDelay()
    {
        yield return new WaitForSecondsRealtime(StartFadeTime);
        var fadeCon = RenderMode.GetInst();
        fadeCon.LevelStartFade = false;
    }
}
