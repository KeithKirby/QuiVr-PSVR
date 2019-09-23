using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CmdLineEvents : MonoBehaviour {

    public UnityEvent OnNoLobby;
    public UnityEvent OnArcadeMode;
    public UnityEvent OnOfflineMode;

	IEnumerator Start () {
        yield return true;
        if (AppBase.v.NoLobby)
            OnNoLobby.Invoke();
        if (AppBase.v.ArcadeMode)
            OnArcadeMode.Invoke();
        if (AppBase.v.OfflineMode)
            OnOfflineMode.Invoke();
	}

}
