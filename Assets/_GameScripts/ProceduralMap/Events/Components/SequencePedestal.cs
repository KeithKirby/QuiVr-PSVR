using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SequencePedestal : MonoBehaviour {

    public GameObject[] RuneDisplays;

    List<GameObject> Runes;
    List<int> CurSeq;
    int pointer;
    bool complete;

    public Color Blank;
    public Color Correct;

    public AudioClip Ok;
    public AudioClip Complete;
    public AudioClip Failed;

    public UnityEvent OnComplete;
    public UnityEvent OnFail;

    void Awake()
    {
        CurSeq = new List<int>();
        Runes = new List<GameObject>();
    }

    [AdvancedInspector.Inspect]
    public void DebugSetup()
    {
        Setup(Random.Range(0, int.MaxValue), Random.Range(3, 6));
    }

    public void TeleportEventResponse(Teleporter t)
    {
        int id = 0;
        if (PhotonNetwork.inRoom)
        {
            for (int i = 0; i < t.Positions.Length; i++)
            {
                if (t.Positions[i].userID == PhotonNetwork.player.ID)
                {
                    id = i;
                }
            }
        }
        Transform obj = t.Positions[id].pos;
        transform.position = obj.position;
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void RandomSetup(int num)
    {
        Setup(Random.Range(0, 99999), num);
    }

    public void Setup(int seed, int num)
    {
        complete = false;
        System.Random r = new System.Random(seed);
        ClearLists();
        for(int i=0; i<num; i++)
        {
            int id = r.Next(0, RuneDisplays.Length);
            CurSeq.Add(id);
            GameObject rdisp = Instantiate(RuneDisplays[id], RuneDisplays[id].transform.parent);
            rdisp.SetActive(true);
            Runes.Add(rdisp);
        }
        CurSeq.Reverse();
    }

	public void RunePress(int id)
    {
        if (CurSeq.Count <= pointer || complete)
            return;
        if(id == CurSeq[pointer])
        {
            Runes[Runes.Count-pointer-1].GetComponent<Image>().color = Correct;
            pointer++;
            if(pointer >= CurSeq.Count)
            {
                complete = true;
                OnComplete.Invoke();
                ClearLists();
                if (Complete != null)
                    VRAudio.PlayClipAtPoint(Complete, transform.position, 1);
            }
            else if (Ok != null)
                VRAudio.PlayClipAtPoint(Ok, transform.position, 1, 0.96f + (0.02f*pointer));
        }
        else
        {
            foreach(var v in Runes)
            {
                if(v != null)
                {
                    Image b = v.GetComponent<Image>();
                    b.color = Blank;
                }
            }
            if (Failed != null)
                VRAudio.PlayClipAtPoint(Failed, transform.position, 1);
            pointer = 0;
            OnFail.Invoke();
        }
    }

    void ClearLists()
    {
        for(int i=0; i<Runes.Count; i++)
        {
            if (Runes[i] != null)
                Destroy(Runes[i]);
        }
        Runes = new List<GameObject>();
        CurSeq = new List<int>();
        pointer = 0;
    }
}
