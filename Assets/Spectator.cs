using UnityEngine;
using System.Collections;
using VRTK;

public class Spectator : MonoBehaviour {

    public GameObject SpectatorArea;
    public GameObject PlayerArea;

    public AnimationCurve MoveCurve;

    public static bool isSpectator;

    public bool leftIn;
    public bool rightIn;

    VRTK_ControllerEvents Left;
    VRTK_ControllerEvents Right;

    VRTK_ControllerEvents MHand;

    public VRTK_ControllerEvents[] Spect;
    public VRTK_ControllerEvents[] Real;

    float cd;

    PlayerSync psync;
    SpectatorSync ssync;

    void Awake()
    {
        psync = GetComponent<PlayerSync>();
        ssync = GetComponent<SpectatorSync>();
    }

    void Start()
    {
        Left = Real[0];
        Right = Real[1];
        if (SteamVR_ControllerManager.bowHand != null)
            MHand = SteamVR_ControllerManager.bowHand.GetComponent<VRTK_ControllerEvents>();
        else
            MHand = Real[0];
    }

    bool wantMove;
    public void NewMove()
    {
        wantMove = true;
        moving = false;
        movePaused = false;
    }

    float heightAdjust;
    Vector3 targetPos;
    public void SetTarget(Transform targ)
    {
        MapTile m = targ.GetComponentInParent<MapTile>();
        if (m != null)
            heightAdjust = m.CurHeight;
        else
            heightAdjust = 0;
        targetPos = new Vector3(targ.position.x, heightAdjust, targ.position.z);
    }

    bool moving;
    bool movePaused;
	void Update()
    {
        leftIn = MHand.touchpadPressed;//Left.touchpadPressed;
        rightIn = MHand.gripPressed;//Right.touchpadPressed;
        if(rightIn && leftIn && cd <=0 && Settings.GetBool("QuickSpectate"))
        {
            cd = 2;
            ToggleSpectator();
        }
        if(cd >= 0)
            cd -= Time.deltaTime/Time.timeScale;
        if(SpectatorArea.activeSelf && !moving && wantMove)
        {
            float mt = Settings.GetFloat("SpctMoveTime");
            if (mt > 0)
            {
                wantMove = false;
                moving = true;
                StopAllCoroutines();
                StartCoroutine("MoveTimed", mt);
            }
            else
            {
                wantMove = false;
                SpectatorArea.transform.position = targetPos;
                SpectatorArea.transform.rotation = Quaternion.identity;
            }
        }
    }

    private Vector3 velocity = Vector3.zero;
    IEnumerator MoveTimed(float dur)
    {
        Vector3 stPos = SpectatorArea.transform.position;
        while (Vector3.Distance(SpectatorArea.transform.position, targetPos) > 0.05f)
        {
            SpectatorArea.transform.position = Vector3.SmoothDamp(SpectatorArea.transform.position, targetPos, ref velocity, dur);
            yield return true;
        }
        moving = false;
    }

    public void ToggleSpectator()
    {
        if(ssync != null)
            ssync.active = !isSpectator;
        if (psync != null)
            psync.active = isSpectator;
        if(PhotonNetwork.inRoom)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["Spectator"] = isSpectator;
            PhotonNetwork.player.SetCustomProperties(props);
        }
        if(isSpectator)
        {
            SpectatorArea.SetActive(false);
            PlayerArea.SetActive(true);
            isSpectator = false;
            if (SteamVR_ControllerManager.bowHand != null)
                MHand = SteamVR_ControllerManager.bowHand.GetComponent<VRTK_ControllerEvents>();
            else
                MHand = Real[0];
            Left = Real[0];
            Right = Real[1];
            Left.gameObject.SetActive(true);
            Right.gameObject.SetActive(true);
            Invoke("TryResetBow", 0.25f);
            if (Time.timeScale < 0.5f)
                Time.timeScale = 1f;
        }
        else
        {
            SpectatorArea.SetActive(true);
            if(TelePlayer.instance != null && TelePlayer.instance.currentNode != null)
            {
                MapTile m = TelePlayer.instance.currentNode.GetComponentInParent<MapTile>();
                if (m != null)
                    heightAdjust = m.CurHeight;
                else
                    heightAdjust = 0;
            }
            SpectatorArea.transform.position = new Vector3(PlayerArea.transform.position.x, heightAdjust, PlayerArea.transform.position.z);
            SpectatorArea.transform.rotation = PlayerArea.transform.rotation;
            isSpectator = true;
            PlayerArea.SetActive(false);
            MHand = Spect[1];
            Left = Spect[0];
            Right = Spect[1];
            Left.gameObject.SetActive(true);
            Right.gameObject.SetActive(true);
        }
    }

    void TryResetBow()
    {
        if (ReturnBowandQuiver.instance != null)
            ReturnBowandQuiver.instance.ReturnObjects();
    }

}
