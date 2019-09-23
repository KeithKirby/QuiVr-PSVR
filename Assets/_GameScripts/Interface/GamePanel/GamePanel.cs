using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class GamePanel : MonoBehaviour {

    public List<GameObject> PlrObjects;
    public GameObject NamePrefab;
    public Transform Holder;
    public PlayerMetric curMetric;

    public Text StatText;
    public Text PanelTitle;
    
    public static GamePanel instance;

    public StatInfo[] Stats;

    void Awake()
    {
        instance = this;
        UpdateStatText();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    const float UpdRate = 0.5f;
    float t = 0;
    void Update()
    {
        t += Time.unscaledDeltaTime;
        if(t > UpdRate)
        {
            t = 0;
            UpdateList();
        }
    }

    void UpdateList()
    {
        if (GameCanvas.instance == null || !GameCanvas.instance.menuOpen)
            return;
        PlayerTuple[] tpls = CurrentValues(curMetric);
        ClearList();
        for(int i=0; i<Mathf.Min(tpls.Length, PlrObjects.Count); i++)
        {
            CreateNew(tpls[i], PlrObjects[i]);
        }
        UpdateTitleText();
        UpdateStatText();
    }

    void CreateNew(PlayerTuple tpl, GameObject nB)
    {
        nB.SetActive(true);
        nB.transform.localPosition = Vector3.zero;
        nB.transform.localScale = Vector3.one;
        nB.transform.localEulerAngles = Vector3.zero;
        nB.GetComponent<GPanelName>().Setup(tpl, strFromVal(tpl.value));
        PlrObjects.Add(nB);
    }

    void UpdateStatText()
    {
        StatText.text = Stats[(int)curMetric].StatName;
    }

    void UpdateTitleText()
    {
        string s = "Game Info";
        if (GameBase.instance != null && GameBase.instance.inGame && GameBase.instance.Difficulty > 0)
        {
            int gateNum = GateManager.instance.CurrentGateIndex(true) + 1;
            s = "Gate: " + gateNum;
            if ((int)(GameBase.instance.Difficulty / 100) > gateNum)
                s += " (" + (int)(GameBase.instance.Difficulty / 100) + ")";
        }         
        PanelTitle.text = s;
        if (GameBase.CanGetReward())
            PanelTitle.color = Color.white;
        else
            PanelTitle.color = new Color(.75f,0,0);
    }

    string strFromVal(float val)
    {
        return val + Stats[(int)curMetric].Suffix;
    }

    void ClearList()
    {
        for (int i = 0; i < PlrObjects.Count; i++)
        {
            if (PlrObjects != null)
            {
                PlrObjects[i].SetActive(false);
            }
        }
    }

    private static bool IsTalking(PlayerSync p)
    {
        var voiceChat = p.GetComponent<PhotonVoiceSpeaker>();
        //float lastRecvTime = voiceChat.LastRecvTime;
        //if ( Time.time - lastRecvTime <= UpdRate )
        if(voiceChat.IsPlaying)
            return true;
        else
            return false;
    }

    public static PlayerTuple[] CurrentValues(PlayerMetric m)
    {
        List<PlayerTuple> tpls = new List<PlayerTuple>();
        if(PhotonNetwork.inRoom)
        {
            PhotonPlayer[] players = PhotonNetwork.playerList;
            foreach (var v in players)
            {
                if(v != null && !v.CustomProperties.ContainsKey("Spectator") || !(bool)v.CustomProperties["Spectator"])
                {
                    float val = 0;
                    if (v != null && v.CustomProperties[m.ToString()] != null)
                        float.TryParse(v.CustomProperties[m.ToString()].ToString(), out val);
                    PlayerTuple t = new PlayerTuple();
                    if (v.ID == PhotonNetwork.player.ID)
                    {
                        t.muted = PlayerSync.myInstance.selfMute;
                        t.name = PlayerSync.myInstance.PlayerName.text;
                        t.isTalking = IsTalking( PlayerSync.myInstance );
                    }
                    else if (PlayerSync.Others != null)
                    {
                        foreach ( var p in PlayerSync.Others )
                        {
                            if ( p.GetComponent<PhotonView>().ownerId == v.ID )
                            {
                                t.muted = p.muted;
                                t.name = p.PlayerName.text;
                                t.isTalking = IsTalking( p );
                            }
                        }
                    }
                    t.pid = v.ID;
                    t.value = val;
                    tpls.Add(t);
                }               
            }
            //tpls = tpls.OrderBy(x => x.value).ThenBy(x => x.pid).ToList();
            tpls.Sort(
                delegate (PlayerTuple a, PlayerTuple b)
                {
                    int valCmp = a.value.CompareTo(b.value);
                    if (valCmp != 0)
                        return valCmp;
                    else
                        return a.pid.CompareTo(b.pid);
                }
            );
        }
        else
        {
            PlayerTuple t = new PlayerTuple();
            t.name = PlatformSetup.instance.DisplayName;
            t.value = Statistics.GetCurrentFloat(m.ToString());
            tpls.Add(t);
        }
        return tpls.ToArray();
    }

    public static PlayerTuple BestVal(PlayerTuple[] plrs)
    {
        PlayerTuple best = plrs[0];
        foreach(var v in plrs)
        {
            if (v.value > best.value)
                best = v;
        }
        return best;
    }

    [BitStrap.Button]
    public void NextStat()
    {
        int i = (int)curMetric;
        i++;
        if (i > System.Enum.GetNames(typeof(PlayerMetric)).Length-1)
            i = 0;
        curMetric = (PlayerMetric)i;
        UpdateList();
    }

    public void PrevStat()
    {
        int i = (int)curMetric;
        i--;
        if (i < 0)
            i = System.Enum.GetNames(typeof(PlayerMetric)).Length-1;
        curMetric = (PlayerMetric)i;
        UpdateList();
    }

    public struct PlayerTuple
    {
        public int pid;
        public string name;
        public float value;
        public bool muted;
        public bool isTalking;
    }

}

public enum PlayerMetric
{
    Accuracy,
    LongShot,
    HighCombo,
    CritPerc,
    EnmKilled
}

[System.Serializable]
public class StatInfo
{
    public string StatName;
    public string Suffix;
    public string OptKey;
}
