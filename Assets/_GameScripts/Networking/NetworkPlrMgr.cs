using UnityEngine;
using System.Collections;

public class NetworkPlrMgr : MonoBehaviour
{
    public TelePlayer telep;
    PhotonView ph;
    public Transform Headset;
    public Transform SpectatorHead;
    public GameObject UIEvents;

    bool mine;

    void Start()
    {
        ph = GetComponent<PhotonView>();
        if (!PhotonNetwork.inRoom || ph == null || ph.isMine)
        {
            UIEvents.SetActive(true);
            telep.gameObject.SetActive(true);
            mine = true;
        }
    }

    void Update()
    {
        if (PhotonNetwork.inRoom && ph.isMine && !ConnectAndJoinRandom.ignoreAFK)
        {
            CheckIdleTimeout();
        }
    }

    public float idleTime = 0;
    Vector3 lastPos;
    Vector3 lastSpecPos;
    void CheckIdleTimeout()
    {
        if (Vector3.Distance(Headset.parent.localPosition, lastPos) < 0.01f && Vector3.Distance(SpectatorHead.localPosition, lastSpecPos) < 0.05f)
        {
            #if !UNITY_EDITOR && !DISABLE_MP_IDLE
            idleTime += Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.1f);
            #endif
        }
        else
        {
            lastPos = Headset.parent.localPosition;
            lastSpecPos = SpectatorHead.localPosition;
            idleTime = 0;
        }
        if (idleTime > 120)
        {
            Debug.Log("AFK Timing Out");
            LeaveMultiplayer.Click();
        }
    }
}
