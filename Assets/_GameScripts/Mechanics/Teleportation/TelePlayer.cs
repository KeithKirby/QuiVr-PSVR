using UnityEngine;
using System.Collections;

public class TelePlayer : MonoBehaviour {

    public static TelePlayer instance;

    public Teleporter currentNode;
    Transform currentPos;
    public GameObject PlayerArea;

    TeleportAudio taud;
    public bool s_tryingTeleport;

    public TeleporterParticles tparts;

    public float lerpSpeed = 25f;

    void Awake () {
        instance = this;
        taud = GetComponent<TeleportAudio>();
    }

    bool tpOff;
    void Update()
    {
        if (currentPos != null && currentNode != null)
        {
            Vector3 wantedPos = currentPos.position - (currentPos.TransformDirection(Vector3.forward) * ForwardOffset());
            if(Vector3.Distance(PlayerArea.transform.position, wantedPos) > 0.05f || currentNode.ForcePosition)
            {
                if (NVR_Player.instance.NonVR)
                    NVR_Player.instance.TPCamMgr.ChangedView();
                Vector3 r = PlayerArea.transform.eulerAngles;
                if ((Mathf.Abs(r.x) < 10 && Mathf.Abs(r.z) < 10) && ((Settings.GetBool("LockRotation") || NVR_Player.instance.NonVR) && !currentNode.ForceRotation))
                {
                    r.y = 0;
                    PlayerArea.transform.eulerAngles = r;
                }
                else
                    PlayerArea.transform.rotation = currentPos.rotation;
                PlayerArea.transform.position = wantedPos;
                TeleporterManager.instance.PlayerMoved();
            }
            if (tparts != null && tparts.On)
                tparts.ParticlesOff();
            if (currentNode != null && currentNode.Disabled && !s_tryingTeleport && TeleporterManager.instance.Areas.Length > TeleporterManager.instance.currentArea)
            {
                if (TeleporterManager.instance != null && TeleporterManager.instance.Areas[TeleporterManager.instance.currentArea].StartTeleporter != null)
                {
                    Teleporter t = TeleporterManager.instance.Areas[TeleporterManager.instance.currentArea].StartTeleporter;
                    if (t != null && !t.Disabled)
                        t.ForceUse();
                }
                else if (TeleporterManager.instance != null)
                {
                    Teleporter node = TeleporterManager.instance.Areas[TeleporterManager.instance.currentArea].TeleportNodes[0].GetComponent<Teleporter>();
                    if (!node.Disabled)
                        node.ForceUse();
                }
                else
                    TeleportClosest();
            }
            else if (!s_tryingTeleport)
                CheckTeleporter();
        }
        else if(!s_tryingTeleport)
            CheckTeleporter();
    }

    public void CheckTeleporter()
    {
        if ((currentNode == null || currentNode.Disabled || currentPos == null) && !s_tryingTeleport)
        {
            TeleportClosest();
        }
    }

    public void Teleport(Teleporter t, Transform position)
    {
        s_tryingTeleport = true;
        if (currentNode != null) // Release position slot
        {
            currentNode.Release(PhotonNetwork.player.ID);
            currentNode.Show();
            currentNode.OnLeave.Invoke(t);
        }

        var fadeController = RenderMode.GetInst();
        fadeController.TeleportTransition(
            () =>
            {
                t.Hide();
                if (taud != null)
                    taud.PlaySound();
                currentNode = t;
                currentPos = position;
                t.OnTeleport.Invoke(t);
                if (tparts != null)
                    tparts.ParticlesOn();                
                s_tryingTeleport = false;
            });
    }

    public bool PoweredUp()
    {
        if(TeleporterManager.instance != null)
        {
            return TeleporterManager.instance.CheckPowered(currentNode);
        }
        return false;
    }

    public SteamVR_PlayArea area;
    float ForwardOffset()
    {
        float o = 0;
        Vector3[] verts = area.vertices;
        foreach (var v in verts)
        {
            o += Mathf.Abs(v.z);
        }
        o /= 4f;
        o *= 0.2f;
        return o;
    }

    public void TeleportClosest(Teleporter ignore=null)
    {
        if (TeleporterManager.instance == null)
            return;
        float dist = float.MaxValue;
        int id = -1;
        for(int i=0; i<TeleporterManager.instance.AllTeleporters.Count; i++)
        {
            Teleporter v = TeleporterManager.instance.AllTeleporters[i];
            if (v != null && PlayerHead.instance != null && v.Positions.Length >= 4 && v != ignore && !v.Disabled && v != TeleporterManager.instance.DeathTP)
            {
                float d = Vector3.Distance(PlayerHead.instance.transform.position, v.transform.position);
                if (d < dist)
                {
                    id = i;
                    dist = d;
                }
            }
        }
        if(id != -1)
            TeleporterManager.instance.AllTeleporters[id].ForceUse();
    }
}
