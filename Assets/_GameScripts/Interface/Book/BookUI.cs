using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookUI : MonoBehaviour {

    public Text TitleText;
    EMOpenCloseMotion baseMotion;
    public List<BookPage> Pages;
    int CurIndex;
    ScrollRect[] InnerScrolls;

    void Start()
    {
        baseMotion = GetComponent<EMOpenCloseMotion>();
        InnerScrolls = GetComponentsInChildren<ScrollRect>();
        foreach (var v in InnerScrolls)
        {
            v.enabled = false;
        }
    }

    [AdvancedInspector.Inspect]
    public void Open()
    {
        if (baseMotion.motionState != EMBaseMotion.MotionState.Open && baseMotion.motionState != EMBaseMotion.MotionState.Opening)
            baseMotion.Open(true);
        if (ToggleMenu.instance != null)
            ToggleMenu.instance.TogglePointers(true);
        SetOpenPage(CurIndex);
        foreach(var v in InnerScrolls)
        {
            v.enabled = true;
        }
    }

    [AdvancedInspector.Inspect]
    public void Close()
    {
        if (baseMotion.motionState != EMBaseMotion.MotionState.Closing && baseMotion.motionState != EMBaseMotion.MotionState.Closed)
            baseMotion.Close(true);
        if (ToggleMenu.instance != null)
            ToggleMenu.instance.TogglePointers(false);
        foreach (var v in InnerScrolls)
        {
            v.enabled = false;
        }
    }

    [AdvancedInspector.Inspect]
    public void NextPage()
    {
        CurIndex++;
        if (CurIndex >= Pages.Count)
        {
            CurIndex = Pages.Count - 1;
            return;
        }
        SetOpenPage(CurIndex);
    }

    [AdvancedInspector.Inspect]
    public void PrevPage()
    {
        CurIndex--;
        if (CurIndex < 0)
        {
            CurIndex = 0;
            return;
        }
        SetOpenPage(CurIndex);
    }

    void SetOpenPage(int idx)
    {
        if(idx >= 0 && idx < Pages.Count)
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                EMOpenCloseMotion m = Pages[i].Moition;
                if(i != idx)
                {
                    if (m.motionState != EMBaseMotion.MotionState.Closing && m.motionState != EMBaseMotion.MotionState.Closed)
                    {
                        m.Close(true);
                        Pages[i].OnClose.Invoke();
                    }
                }
                else
                {
                    if (m.motionState != EMBaseMotion.MotionState.Opening && m.motionState != EMBaseMotion.MotionState.Open)
                    {
                        TitleText.text = Pages[i].Title;
                        m.Open(true);
                        Pages[i].OnOpen.Invoke();
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class BookPage
    {
        public string Title;
        public EMOpenCloseMotion Moition;

        public UnityEngine.Events.UnityEvent OnOpen;
        public UnityEngine.Events.UnityEvent OnClose;

        public override string ToString()
        {
            return Title;
        }
    }
}
