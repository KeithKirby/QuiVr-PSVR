using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NVR_WTInterface : MonoBehaviour {

    public Teleporter MainTP;
    public bool On;
    public NVR_UI_Item[] UIitems;
    public int StartIndex;
    NVR_UI_Item CurrentItem;
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    void Awake()
    {
        foreach(var v in UIitems)
        {
            v.Setup();
            v.ToggleOutline(false);
        }
        MainTP.OnTeleport.AddListener(delegate { EnterWatchtower(); });
        MainTP.OnLeave.AddListener(delegate { LeaveWatchtower(); });
    }

	public void EnterWatchtower()
    {
        if(NVR_Player.isThirdPerson())
        {
            On = true;
            foreach (var v in UIitems)
            {
                v.Deselect();
                v.ToggleOutline(false);
            }
            SelectAt(StartIndex);
            NVR_Player.instance.ToggleHands(false);
            NVR_Player.instance.ForceFirstPerson(true);
            OnActivate.Invoke();
        }
    }

    public void LeaveWatchtower()
    {
        On = false;
        if (NVR_Player.isThirdPerson())
        {
            NVR_Player.instance.ToggleHands(true);
            NVR_Player.instance.ForceFirstPerson(false);
        }
        OnDeactivate.Invoke();
    }

    void Update()
    {
        if(On)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                Move(MoveDir.Up);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                Move(MoveDir.Down);
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                Move(MoveDir.Left);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                Move(MoveDir.Right);
            if (Input.GetKeyDown(KeyCode.Space) && CurrentItem != null)
                CurrentItem.Activate();

        }
    }

    void Move(MoveDir dir)
    {
        if(CurrentItem != null)
        {
            GameObject o = CurrentItem.Up;
            if (dir == MoveDir.Down)
                o = CurrentItem.Down;
            else if (dir == MoveDir.Left)
                o = CurrentItem.Left;
            else if (dir == MoveDir.Right)
                o = CurrentItem.Right;
            SelectAt(GetIndex(o));
        }
    }

    int GetIndex(GameObject obj)
    {
        int id = -1;
        for(int i=0; i<UIitems.Length; i++)
        {
            if (UIitems[i].Main == obj)
                return i;
        }
        return id;
    }

    public void SelectAt(int index)
    {
        if(index >= 0 && index < UIitems.Length)
        {
            for(int i=0; i<UIitems.Length; i++)
            {
                if(i == index)
                {
                    if(UIitems[i] != CurrentItem)
                    {
                        if(CurrentItem != null)
                            CurrentItem.Deselect();
                        CurrentItem = UIitems[i];
                        CurrentItem.Select();
                    }
                }
            }
        }
    }

    enum MoveDir
    {
        Up,
        Down,
        Left,
        Right
    }

    [System.Serializable]
    public class NVR_UI_Item
    {
        Highlight outline;
        public GameObject Main;
        public GameObject Up;
        public GameObject Down;
        public GameObject Left;
        public GameObject Right;

        public UnityEvent OnSelect;
        public UnityEvent OnDeselect;
        public UnityEvent OnActivate;

        public void Setup()
        {
            outline = Main.GetComponent<Highlight>();
        }

        public void Select()
        {
            OnSelect.Invoke();
            ToggleOutline(true);
        }

        public void Deselect()
        {
            OnDeselect.Invoke();
            ToggleOutline(false);
        }

        public void Activate()
        {
            OnActivate.Invoke();
            if (outline != null && outline.CheckRenderer())
            {
                outline.AddMat();
            }         
        }

        public void ToggleOutline(bool val)
        {
            if(outline != null)
                outline.enabled = val;
        }

        public override string ToString()
        {
            string s = "Not Set";
            if (Main != null)
                s = Main.name;
            return s;
        }
    }


}
