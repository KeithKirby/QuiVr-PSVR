using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStreamInfoDisplay : MonoBehaviour {

    public float UpdateDelay = 2;

    public Text Label;
    public bool UseLabel;
    public bool UseDebugLog;

    string _enemiesTag = "Enemies : ";
    string _clampTag = "Clamp : ";
    
    void DisplayInfo()
    {
        var stream = GameBase.instance.CurrentStream;

        if (null == stream || !stream.streaming)
            return;

        string enemiesCount = stream.EnemiesOut.Count.ToString();
        string clampVal = stream.ClampVal.ToString();

        string msg = _enemiesTag + enemiesCount + "\n" + _clampTag + clampVal;

        if (UseLabel && null != Label)
            Label.text = msg.Replace("\\n", "\n");

        if (UseDebugLog)
            Debug.Log("EnemyStreamInfo:: " + msg);
    }

    private void Start()
    {
        InvokeRepeating("DisplayInfo", 0, UpdateDelay);
    }

    private void OnDestroy()
    {
        CancelInvoke("DisplayInfo");
    }
}
