using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvPPanel : MonoBehaviour {

    public Text Resource;
    public Text EnmBudget;
    int localResource;
    int localBudget;

    public static PvPPanel instance;

    public GameObject[] Pages;
    public CreatureSpawnButton[] Buttons;
    int PageID;

    EMOpenCloseMotion motion;
    public bool open;

    public AudioClip[] Spawns;

    void Awake()
    {
        motion = GetComponent<EMOpenCloseMotion>();
        instance = this;
    }

    void Start()
    {
        CheckButtons();
        UpdateButtons();
    }

    public void PlaySpawn()
    {
        if(Spawns.Length > 0)
        {
            AudioClip c = Spawns[Random.Range(0, Spawns.Length)];
            VRAudio.PlayClipAtPoint(c, transform.position, 0.8f*VolumeSettings.GetVolume(AudioType.Effects), 1f, 0.9f);
        }
    }

    public bool isOpen()
    {
        return open;
    }

    void Update()
    {
        open = motion.motionState != EMBaseMotion.MotionState.Closed;
        if (!open || pvpmanager.instance == null)
            return;
        if(localResource != pvpmanager.instance.myResource)
        {
            localResource = (int)pvpmanager.instance.myResource;
            Resource.text = "" + localResource;
            CheckButtons();
        }
        if(localBudget != pvpmanager.instance.AllowedEnm())
        {
            localBudget = pvpmanager.instance.AllowedEnm();
            EnmBudget.text = (pvpmanager.instance.EnemyBudget-localBudget) + "/" + pvpmanager.instance.EnemyBudget;
            CheckButtons();
        }
    }

    void CheckButtons()
    {
        foreach(var v in Buttons)
        {
            Button b = v.GetComponent<Button>();
            b.interactable = localResource >= v.Cost && localBudget > 0;
        }
    }

    public void NextPage()
    {
        PageID++;
        if (PageID >= Pages.Length)
            PageID = 0;
        ChangePage();
    }

    public void PrevPage()
    {
        PageID--;
        if (PageID < 0)
            PageID = Pages.Length - 1;
        ChangePage();
    }

    float lastUpdate;
    public void RefreshVals()
    {
        if(Time.time-lastUpdate > 5)
        {
            //RemoteSettings.ForceUpdate();
            lastUpdate = Time.time;
        }
    }

    void ChangePage()
    {
        for(int i=0; i<Pages.Length; i++)
        {
            Pages[i].SetActive(i == PageID);
        }
    }

    public void UpdateButtons()
    {
        foreach(var v in GetComponentsInChildren<CreatureSpawnButton>())
        {
            v.UpdateButtonVals();
        }
    }
}
