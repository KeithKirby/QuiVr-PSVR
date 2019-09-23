using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour {

    public static GateManager instance;
    public List<Gate> AllGates;
    public int GIndex;
    [HideInInspector]
    public PhotonView view;
    int curRestoreGate = -1;
    public int highestGateRestores = 0;

    #region LifeCycle

    void Awake()
    {
        instance = this;
        view = GetComponent<PhotonView>();
    }

    public void AddGate(Gate g)
    {
        AllGates.Add(g);
        ClearBad();
        SortFinal();
    }

    public void SortFinal()
    {
        AllGates.Sort((x, y) =>
            x.GateIndex.CompareTo(y.GateIndex)//Vector3.Distance(transform.position, x.transform.position).CompareTo(Vector3.Distance(transform.position, y.transform.position))
        );
    }

    public void RemoveGate(Gate g)
    {
        AllGates.Remove(g);
        ClearBad();
        if (this != null)
            SortFinal();
    }

    void ClearBad()
    {
        for (int i = AllGates.Count - 1; i >= 0; i--)
        {
            if (AllGates[i] == null)
                AllGates.RemoveAt(i);
        }
    }

    public void Reset()
    {
        highestGateRestores = 0;
        curRestoreGate = -1;
    }   

    #endregion

    #region NetworkCommands
    public void SendLaser(Gate g, PhotonTargets targ, Vector3 point)
    {
        for (int i = 0; i < AllGates.Count; i++)
        {
            if (AllGates[i] == g)
            {
                view.RPC("DoLaserNetwork", targ, i, point);
                return;
            }
        }
    }
    [PunRPC]
    void DoLaserNetwork(int id, Vector3 point)
    {
        if (AllGates.Count > id)
        {
            AllGates[id].DoLaser(point, point);
        }
    }

    public void SetRotationValue(Gate g, PhotonTargets targ)
    {
        for(int i=0; i<AllGates.Count; i++)
        {
            if(AllGates[i] == g)
            {
                view.RPC("SetRotationNetwork", targ, i, g.Halves[0].transform.rotation, g.Halves[1].transform.rotation, g.isClosed(), g.isDestroyed());
                return;
            }
        }
    }
    [PunRPC]
    void SetRotationNetwork(int id, Quaternion r1, Quaternion r2, bool cl, bool dst)
    {
        if(AllGates.Count > id)
        {
            Gate g = AllGates[id];
            g.Halves[0].transform.rotation = r1;
            g.Halves[1].transform.rotation = r2;
            g.closed = cl;
            g.destroyed = dst;
            if (dst)
            {
                g.DestroyDisplay(true);
            }
        }
    }

    public void ForceRestore(Gate g, PhotonTargets targ)
    {
        for (int i = 0; i < AllGates.Count; i++)
        {
            if (AllGates[i] == g)
            {
                view.RPC("ForceRestoreNetwork", targ, i);
            }
        }
    }
    [PunRPC]
    void ForceRestoreNetwork(int id)
    {
        if (AllGates.Count > id)
        {
            AllGates[id].ForceRestore();
        }
    }

    public void Restore(Gate g, PhotonTargets targ)
    {
        for (int i = 0; i < AllGates.Count; i++)
        {
            if (AllGates[i] == g)
            {
                view.RPC("RestoreNetwork", targ, i);
            }
        }
    }
    [PunRPC]
    void RestoreNetwork(int id)
    {
        if (AllGates.Count > id)
        {
            AllGates[id].ActivateRestore();
        }
    }

    public void HealGate(Gate g, float val)
    {
        for (int i = 0; i < AllGates.Count; i++)
        {
            if (AllGates[i] == g)
            {
                view.RPC("HealGateNetwork", PhotonTargets.AllViaServer, i, val);
            }
        }
    }
    [PunRPC]
    void HealGateNetwork(int id, float val)
    {
        if (AllGates.Count > id)
        {
            AllGates[id].GetComponent<Health>().takeDamageImmediate(-1*val);
        }
    }

    public void AddRevive(Gate g, int val, PhotonTargets targ)
    {
        for (int i = 0; i < AllGates.Count; i++)
        {
            if (AllGates[i] == g)
            {
                view.RPC("AddRevNetwork", targ, i, val);
            }
        }
    }
    [PunRPC]
    void AddRevNetwork(int id, int val)
    {
        if (AllGates.Count > id)
        {
            AllGates[id].AddRevive(val);
        }
    }

    public void TryClose(Gate g, PhotonTargets targ)
    {
        for (int i = 0; i < AllGates.Count; i++)
        {
            if (AllGates[i] == g)
            {
                view.RPC("TryCloseNetwork", targ, i);
            }
        }
    }
    [PunRPC]
    void TryCloseNetwork(int id)
    {
        if (AllGates.Count > id)
        {
            AllGates[id].TryCloseNetwork();
        }
    }

    public void ForceClose(Gate g, PhotonTargets targ)
    {
        for (int i = 0; i < AllGates.Count; i++)
        {
            if (AllGates[i] == g)
            {
                view.RPC("ForceCloseNetwork", targ, i);
            }
        }
    }
    [PunRPC]
    void ForceCloseNetwork(int id)
    {
        if (AllGates.Count > id)
        {
            AllGates[id].ForceClose();
        }
    }

    public void DestroyGate(Gate g, PhotonTargets targ)
    {
        for (int i = 0; i < AllGates.Count; i++)
        {
            if (AllGates[i] == g)
            {
                view.RPC("DestroyGateNetwork", targ, i);
            }
        }
    }
    [PunRPC]
    void DestroyGateNetwork(int id)
    {
        if (AllGates.Count > id)
        {
            AllGates[id].DestroyGate();
        }
    }
    #endregion

    #region Network Lifecycle
    void OnPhotonPlayerConnected(PhotonPlayer plr)
    {
        if (PhotonNetwork.isMasterClient)
        {
            for(int i=0; i<AllGates.Count; i++)
            {
                Gate g = AllGates[i];
                view.RPC("SetRotationNetwork", plr, i, g.Halves[0].transform.rotation, g.Halves[1].transform.rotation, g.isClosed(), g.isDestroyed());
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            string s = GetGateData();
            stream.SendNext(s);
            stream.SendNext(curRestoreGate);
            stream.SendNext(highestGateRestores);
        }
        else
        {
            string s = (string)stream.ReceiveNext();
            SetGateData(s);
            curRestoreGate = (int)stream.ReceiveNext();
            highestGateRestores = (int)stream.ReceiveNext();
        }
    }

    string GetGateData()
    {
        string d = "";
        for(int i=0; i<AllGates.Count; i++)
        {
            Gate g = AllGates[i];
            string s = "";
            Vector3 a1 = g.Halves[0].GateObject.localEulerAngles;
            Vector3 a2 = g.Halves[1].GateObject.localEulerAngles;
            s += a1.x.ToString("n2") + "," + a1.y.ToString("n2") + "," + a1.z.ToString("n2") + "-";
            s += a2.x.ToString("n2") + "," + a2.y.ToString("n2") + "," + a2.z.ToString("n2") + "-";
            s += (g.closed) ? "1-" : "0-";
            s += (g.destroyed) ? "1-" : "0-";
            s += (g.isDisabled()) ? "1-" : "0-";
            s += g.RevAcum + "-";
            s += g.GetHP().currentHP;
            d += s;
            if (i < AllGates.Count - 1)
                d += ":";
        }
        return d;
    }

    void SetGateData(string inpt)
    {
        string[] gts = inpt.Split(':');
        for(int i=0; i<Mathf.Min(AllGates.Count, gts.Length); i++)
        {
            Gate g = AllGates[i];
            string gv = gts[i];
            string[] pts = gv.Split('-');
            if (pts.Length > 6)
            {
                g.Halves[0].GateObject.localEulerAngles = VecFromStr(pts[0]);
                g.Halves[1].GateObject.localEulerAngles = VecFromStr(pts[1]);
                g.closed = pts[2] == "1";
                bool destroyed = pts[3] == "1";
                if (destroyed && !g.destroyed)
                    g.DestroyDisplay(true);
                if (!destroyed && g.destroyed)
                    g.RestoreDisplay(true);
                g.destroyed = destroyed;
                g.disabled = pts[4] == "1";
                int x = 0;
                int.TryParse(pts[5], out x);
                float hp = 1;
                float.TryParse(pts[6], out hp);
                g.RevAcum = x;
                g.GetHP().currentHP = hp;
            }
        }
    }

    Vector3 VecFromStr(string inpt)
    {
        string[] s = inpt.Split(',');
        float x = 0; float y = 0; float z = 0;
        if(s.Length > 2)
        {
            float.TryParse(s[0], out x);
            float.TryParse(s[1], out y);
            float.TryParse(s[2], out z);
        }
        return new Vector3(x,y,z);
    }
    #endregion

    #region Utility
    public int GateID(Gate g)
    {
        if(g != null)
        {
            for (int i = 0; i < AllGates.Count; i++)
            {
                if (AllGates[i] == g)
                    return i;
            }
        }
        return -1;
    }

    public Gate GetGate(int id)
    {
        if (id < 0 || id >= AllGates.Count)
            return null;
        else
            return AllGates[id];
    }

    public int CurrentGateIndex(bool check=true)
    {
        if(check)
        {
            GIndex = 1;
            for (int i = AllGates.Count - 1; i >= 0; i--)
            {
                if (AllGates[i].closed && !AllGates[i].destroyed)
                {
                    GIndex = i;
                    return i;
                }
            }
        }
        return GIndex;
    }

    public void AddRestore(int GateID)
    {
        if (GateID > curRestoreGate)
        {
            curRestoreGate = GateID;
            highestGateRestores = 0;
        }
        else
            highestGateRestores += 1;
    }

    public static Gate CurrentGate()
    {
        if (instance != null)
        {
            return instance.GetGate(instance.CurrentGateIndex());
        }
        return null;
    }
        
    #endregion
}
