using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WingUI : MonoBehaviour {

    public GameObject WingPrefab;
    public List<WingOptionUI> WingOptions;
    ScrollRect scroll;
    public Transform ScrollContent;
    public float minY = -45;
    public float maxY = -155f;
    public float range = 80f;
    bool open;

    void Awake()
    {
        scroll = GetComponentInChildren<ScrollRect>();
        WingOptions = new List<WingOptionUI>();
    }

    public void OnOpen()
    {
        open = true;
        scroll.enabled = true;
        SetupList();
        if (Cosmetics.instance.WingID < 0 || Cosmetics.instance.WingID > Cosmetics.WingIDs.Count || Cosmetics.WingIDs[Cosmetics.instance.WingID] > ItemDatabase.v.WingOptions.Length)
            SelectWing(null);
        else
            SelectWing(ItemDatabase.v.WingOptions[Cosmetics.WingIDs[Cosmetics.instance.WingID]]);
    }

    public void OnClose()
    {
        open = false;
        scroll.enabled = false;
        /*
        foreach (var v in WingOptions)
        {
            v.ToggleWings(false);
        }
        */
    }

    public void SetupList()
    {
        if(WingOptions != null)
        {
            foreach(var v in WingOptions)
            {
                Destroy(v.obj);
            }
        }
        WingOptions = new List<WingOptionUI>();
        AddNewOption(null);
        if(Cosmetics.WingIDs != null && Cosmetics.WingIDs.Count > 0)
        {
            foreach (var v in Cosmetics.WingIDs)
            {
                if (v >= 0 && v < ItemDatabase.v.WingOptions.Length)
                    AddNewOption(ItemDatabase.v.WingOptions[v]);
            }
        }
    }

    void CheckMasked()
    {
        if (open)
        {
            float scrollAmt = ScrollContent.localPosition.y;
            foreach (var o in WingOptions)
            {
                float y = o.GetPos();
                float diff = y + scrollAmt;
                bool offScreen = diff < minY || diff > maxY;
                //o.ToggleWings(!offScreen);
            }
        }
    }

    public void AddNewOption(Wings wing)
    {
        GameObject newObj = Instantiate(WingPrefab, WingPrefab.transform.parent);
        newObj.SetActive(true);

        WingOptionUI nui = new WingOptionUI();
        nui.Setup(wing, newObj);
        nui.obj = newObj;
        nui.Fill();
        nui.button.onClick.AddListener(delegate { SelectWing(wing); });
        WingOptions.Add(nui);
    }

    void SelectWing(Wings wing)
    {
        Cosmetics.SetWings(wing);
        foreach (var v in WingOptions)
        {
            if ((v.wing == null && wing == null) || (v.wing != null && wing != null && v.wing.WingMat == wing.WingMat))
            {
                v.CheckBox.SetActive(false);
                v.button.interactable = false;
            }
            else
            {
                v.CheckBox.SetActive(true);
                v.button.interactable = true;
            }
        }
    }

    public class WingOptionUI
    {
        public GameObject obj;
        public Text Title;
        public Button button;
        public GameObject CheckBox;
        public Image[] rends;
        public Wings wing;

        public void Fill()
        {
            Title = obj.GetComponentInChildren<Text>();
            button = obj.GetComponent<Button>();
            Image[] imgs = obj.GetComponentsInChildren<Image>();
            List<Image> wings = new List<Image>();
            foreach (var i in imgs)
            {
                if (i.gameObject.name == "Checkbox")
                    CheckBox = i.gameObject;
            }
            rends = wings.ToArray();
            if (wing != null)
                Title.text = wing.ToString();
            else
                Title.text = "None";
        }

        /*
        public void ToggleWings(bool val)
        {
            foreach(var v in rends)
            {
                v.enabled = val;
            }
        }
        */

        public void Setup(Wings wings, GameObject o)
        {
            Image[] imgs = o.GetComponentsInChildren<Image>();
            List<Image> wrends = new List<Image>();
            foreach (var i in imgs)
            {
                if (i.gameObject.name == "Wing")
                    wrends.Add(i);
            }
            rends = wrends.ToArray();
            wing = wings;
            if(wing != null)
            {
                foreach(var v in rends)
                {
                    v.material = wing.WingMat;
                }
            }
            else
            {
                foreach (var v in rends)
                {
                    v.material = ItemDatabase.v.WingOptions[0].WingMat;
                }
            }
        }

        public float GetPos()
        {
            return obj.transform.localPosition.y;
        }

        public override string ToString()
        {
            if (wing != null)
                return wing.WingMat.name;
            return "__Wing Name__";
        }

    }
}
