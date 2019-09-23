using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Parse;

public class GameServerInfo : MonoBehaviour {

    public Text Body;
    EMOpenCloseMotion motion;

    void Awake()
    {
        motion = GetComponent<EMOpenCloseMotion>();
    }

    IEnumerator Start()
    {
        while (!SteamManager.Initialized || PlatformSetup.instance == null || !PlatformSetup.instance.serverchecked)
        {
            yield return true;
        }
        CheckForMessage();
    }

    public void CheckForMessage()
    {
        if(gameObject.activeSelf)
        {
            StopCoroutine("CheckServer");
            StartCoroutine("CheckServer");
        }
    }

    IEnumerator CheckServer()
    {
        if (!AppBase.v.serverOnline)
        {
            motion.Open();
            Body.text = "The QuiVr Data Server is currently offline. You can still play, but no progress will be saved.\n\nVisit the Steam Page for more info.";
        }
        else
        {
            // dw - disable motd coz parse removed
            /*
            var conf = ParseConfig.GetAsync();
            while (!conf.IsCompleted)
                yield return null;
            string Msg = "";
            if (AppBase.v.isBeta)
                ParseConfig.CurrentConfig.TryGetValue<string>("BetaMessage", out Msg);
            else
                ParseConfig.CurrentConfig.TryGetValue<string>("ServerMessage", out Msg);
            if (Msg.Length > 5)
            {
                motion.Open();
                Body.text = Msg;
            }
            */
        }
        yield return null;
    }
}
