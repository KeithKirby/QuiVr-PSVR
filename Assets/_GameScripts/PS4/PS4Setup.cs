using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS4Setup : MonoBehaviour
{
    static PS4Setup _instance;

    // Use this for initialization
    void Awake()
    {
        if (_instance == null)
        {
#if !UNITY_EDITOR
            //Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None); // dw - Disable stack traces when logging for ps4 console
#endif
            //Debug.Log("Setup mono targets");
            _instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            //if (PhotonNetwork.SendMonoMessageTargets == null)
                //PhotonNetwork.SendMonoMessageTargets = new HashSet<GameObject>();
            //PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (_instance == this)
        {
            PlatformSetup.CheckVRUnit();
        }
    }
}