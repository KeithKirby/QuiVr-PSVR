using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SelectionSpinner : MonoBehaviour {

    [Header("Setup")]
    float SegmentAngle = 60;
    public ItemType itemtype;
    public GameObject[] ItemSelectors;
    public float Drag;
    public float DistanceThreshold;
    public AudioClip[] TickClips;

    //Components
    ArmorSelector[] selectors;
    AudioSource src;
    Transform SelectingHand;
    List<ArmorOption> items;
    VRTK_ControllerActions act;
    VRTK_ControllerEvents evt;
    //Runtime Values
    [Header("Runtime")]
    public float AngleOffset;
    float triggerValue;
    float AngularVel;
    Vector3 lastPt;
    Queue<Vector2> dragpts;
    bool fullGrab;
    bool Active;
    public int curIndex;
    public float centerAngle;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        selectors = new ArmorSelector[ItemSelectors.Length];
        for(int i=0; i<ItemSelectors.Length; i++)
        {
            selectors[i] = new ArmorSelector(ItemSelectors[i], itemtype);
        }
        dragpts = new Queue<Vector2>();
    }

    void OnEnable()
    {
        Active = true;
        RebuildItems();
        EnchantmentPublisher.ItemDisenchanted += EnchantmentPublisher_ItemDisenchanted;

    }

    void OnDisable()
    {
        Active = false;
        EnchantmentPublisher.ItemDisenchanted -= EnchantmentPublisher_ItemDisenchanted;
    }

    void RebuildItems()
    {
        Reset();
        if (Armory.instance != null && Armory.ValidFetch)
        {
            ArmorOption[] itms = Armory.instance.GetOptions(itemtype);
            items = new List<ArmorOption>();
            foreach (var v in itms)
            {
                items.Add(v);
            }
            items.Sort((x, y) => y.rarity.CompareTo(x.rarity));
        }
        FillSlots();
    }

    private void EnchantmentPublisher_ItemDisenchanted(ArmorOption item)
    {
        HandleItemRemoved(item);
    }

    void HandleItemRemoved(ArmorOption item)
    {
        // If an item has been removed from this carousel, then rebuild the slots
        for(int i=0;i< items.Count;++i)
        {
            var itm = items[i];
            if (!Armory.instance.OwnsItem(itm))
            {
                RebuildItems();
            }
        }
    }

    void Reset()
    {
        AngleOffset = 0;
        curState = 0;
        curIndex = 0;
        lastIndex = 0;
        transform.localEulerAngles = Vector3.zero;
    }

    void FillSlots()
    {
        if(items != null && items.Count > 0)
        {
            if (items.Count > 6)
            {
                for(int i=0; i<6; i++)
                {
                    selectors[i].Enable();
                }
                selectors[0].SetItem(items[0], 1);
                selectors[5].SetItem(items[1], 2);
                selectors[4].SetItem(items[2], 3);
                selectors[1].SetItem(items[items.Count-1], items.Count);
                selectors[2].SetItem(items[items.Count - 2], items.Count-1);
                selectors[3].SetItem(items[3], 4); //?
            }
            else
            {
                curIndex = 0;
                if (items.Count > 0)
                {
                    selectors[0].Enable();
                    selectors[0].SetItem(items[0], 1);
                }
                else
                    selectors[0].Disable();
                if (items.Count > 1)
                {
                    selectors[5].Enable();
                    selectors[5].SetItem(items[1], 2);
                }
                else
                    selectors[5].Disable();
                if (items.Count > 2)
                {
                    selectors[1].Enable();
                    selectors[1].SetItem(items[2], 3);
                }
                else
                    selectors[1].Disable();
                if (items.Count > 3)
                {
                    selectors[4].Enable();
                    selectors[4].SetItem(items[3], 4);
                }
                else
                    selectors[4].Disable();
                if (items.Count > 4)
                {
                    selectors[2].Enable();
                    selectors[2].SetItem(items[4], 5);
                }
                else
                    selectors[2].Disable();
                if (items.Count > 5)
                {
                    selectors[3].Enable();
                    selectors[3].SetItem(items[5], 6);
                }
                else
                    selectors[3].Disable();
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                selectors[i].Disable();
            }
        }
    }

    void Update()
    {
        if(Active)
        {
            if (lastChanged != null)
                Debug.DrawRay(lastChanged.position, Vector3.up * 2, Color.green);
            CalcAngleOffset();
            if (SelectingHand == null)
            {
                if (Mathf.Abs(AngularVel) > 0.1f)
                {
                    AngularVel -= (AngularVel * Drag) * Time.deltaTime;
                }
                else
                    AngularVel = 0;
                if (Mathf.Abs(AngleOffset) > 2 && Mathf.Abs(AngularVel) < 10)
                {
                    AngularVel = Mathf.Lerp(AngularVel, -AngleOffset * 2, Time.deltaTime);
                }
                AngularVel = Mathf.Min(AngularVel, 29 / Time.deltaTime);
                transform.localEulerAngles += Vector3.up * Time.deltaTime * AngularVel;
            }
            else if (evt != null)
            {
                float angleDiff = Vector3.SignedAngle(transform.position - lastPt, transform.position - SelectingHand.position, transform.up);
                transform.localEulerAngles += transform.up * angleDiff;
                lastPt = SelectingHand.position;
                AngularVel = angleDiff / Time.deltaTime;
                dragpts.Enqueue(new Vector2(AngularVel, Time.time));
                for (int i = 0; i < 10; i++)
                {
                    if (dragpts.Count > 1 && Time.time - dragpts.Peek().y > 0.1f)
                        dragpts.Dequeue();
                    else
                        break;
                }
                triggerValue = evt.GetTriggerAxis();
                if (!fullGrab && triggerValue > 0.1f)
                    fullGrab = true;
                if (triggerValue < 0.1f && fullGrab)
                {
                    float max = 0;
                    float ang = 0;
                    foreach(var v in dragpts)
                    {
                        if(Mathf.Abs(v.x) > max)
                        {
                            max = Mathf.Abs(v.x);
                            ang = v.x;
                        }
                    }
                    if (max > Mathf.Abs(AngularVel))
                        AngularVel = ang;
                    Unuse();
                }
            }
            if (items != null && items.Count > 6)
                UpdateItemOrbs();
            CheckAngleForClick();
        }
    }

    Transform lastChanged;
    int lastIndex;
    public int curState;
    void UpdateItemOrbs()
    {
        //Set Current Index
        curIndex = ((int)(((360-transform.localEulerAngles.y)+30) / 60f) % 6);
        if (curIndex < 0)
            curIndex = 6 + curIndex;
        if (lastIndex != curIndex)
        {
            bool movedForward = (lastIndex > curIndex && !(lastIndex == 5 && curIndex == 0)) || (lastIndex == 0 && curIndex == 5);
            int leftSide = curIndex + 2;
            if (leftSide > 5)
                leftSide -= 6;
            int rightSide = curIndex - 2;
            if (rightSide < 0)
                rightSide += 6;
            if (movedForward)
            {
                curState++;
                if (curState >= items.Count)
                    curState = 0;
                int modIndex = curState + 2;
                if (modIndex >= items.Count)
                    modIndex -= items.Count;
                selectors[rightSide].SetItem(items[modIndex], modIndex+1);
                lastChanged = selectors[rightSide].orb.transform;
            }              
            else
            {
                curState--;
                if (curState < 0)
                    curState = items.Count-1;
                int modIndex = curState - 2;
                if (modIndex < 0)
                    modIndex += items.Count;
                selectors[leftSide].SetItem(items[modIndex], modIndex+1);
                lastChanged = selectors[leftSide].orb.transform;
            }       
            lastIndex = curIndex;
        }
    }

    public void ShowClosest(Vector3 p1, Vector3 p2)
    {
        float dist = float.MaxValue;
        ArmorSelector closest = selectors[0];
        foreach (var v in selectors)
        {
            float minDist = v.selectUI != null?Mathf.Min(Vector3.Distance(v.selectUI.transform.position, p1), Vector3.Distance(v.selectUI.transform.position, p2)):float.MaxValue;
            if (minDist > DistanceThreshold || !v.selectUI.gameObject.activeSelf)
                return;
            if (minDist < dist)
            {
                dist = minDist;
                closest = v;
            }
        }
        if (dist < 1f)
        {
            foreach (var v in selectors)
            {
                if (closest == v)
                {
                    v.Show();
                }
                else
                {
                    v.Hide();
                }
            }
        }
        else
        {
            foreach (var v in selectors)
            {
                v.Hide();
            }
        }
    }

    public void HideAll()
    {
        foreach (var v in selectors)
        {
            v.Hide();
        }
    }

    void CalcAngleOffset()
    {
        AngleOffset = transform.localEulerAngles.y % 60;
        if (AngleOffset > 30)
            AngleOffset = -1 * (60 - AngleOffset);
    }

    float lastClickAngle;
    bool canClick;
    void CheckAngleForClick()
    {
        if (Mathf.Abs(transform.localEulerAngles.y - lastClickAngle) > 5)
            canClick = true;
        if(canClick && Mathf.Abs(AngleOffset) < 2)
        {
            lastClickAngle = transform.localEulerAngles.y;
            canClick = false;
            if (act != null)
                act.TriggerHapticPulse((ushort)1000);
            PlayClick();
        }
    }

    void PlayClick()
    {
        if(src != null && TickClips.Length > 0)
        {
            src.clip = TickClips[Random.Range(0, TickClips.Length)];
            src.time = 0;
            src.pitch = Random.Range(0.9f, 1.1f);
            src.Play();
        }
    }

	public void Use(object o, InteractableObjectEventArgs e)
    {
        GameObject hand = e.interactingObject;
        if (hand != null)
        {
            fullGrab = false;
            SelectingHand = hand.transform;
            act = hand.GetComponent<VRTK_ControllerActions>();
            evt = hand.GetComponent<VRTK_ControllerEvents>();
            lastPt = SelectingHand.position;
            dragpts.Clear();
            dragpts.Enqueue(new Vector2(0, Time.time));
        }
    }

    public void Unuse()
    {
        SelectingHand = null;
        evt = null;
        act = null;
        dragpts.Clear();
    }

    public class ArmorSelector
    {
        GameObject holder;
        public ItemChangeUI selectUI;
        public ItemOrbDisplay orb;
        public OutfitActivator acti;
        ItemType itemtype;

        public void Show()
        {
            if (selectUI != null)
                selectUI.Show();
        }

        public void Hide()
        {
            if (selectUI != null)
                selectUI.Hide();
        }

        public void SetItem(ArmorOption o, int id)
        {
            orb.Setup(o);
            selectUI.Setup(o);
            foreach(var v in selectUI.ExtraNums)
            {
                v.text = id.ToString();
            }
        }

        public void Disable()
        {
            holder.SetActive(false);
        }

        public void Enable()
        {
            holder.SetActive(true);
        }

        public ArmorSelector(GameObject go, ItemType itype)
        {
            holder = go;
            selectUI = go.GetComponentInChildren<ItemChangeUI>();
            orb = go.GetComponentInChildren<ItemOrbDisplay>();
            acti = go.GetComponentInChildren<OutfitActivator>();
            itemtype = itype;
            if (selectUI != null)
                selectUI.type = itype;
            if (acti != null)
                acti.Type = itype;
        }
    }
}
