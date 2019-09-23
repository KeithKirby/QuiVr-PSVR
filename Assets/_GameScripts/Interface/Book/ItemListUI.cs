using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Parse;
using SimpleJSON;
using UnityEngine.UI;

public class ItemListUI : MonoBehaviour {

    public GameObject ItemPrefab;
    ScrollRect scroll;
    public Color OwnedColor = Color.white;
    public Color UnownedColor = Color.gray;
    public Transform ScrollContent;
    public float minY = -45;
    public float maxY = -155f;
    public float range = 80f;
    List<ArmorOption> Options;
    public List<ItemOptionUI> Items;

    public bool open;

    void Awake()
    {
        Options = new List<ArmorOption>();
        scroll = GetComponentInChildren<ScrollRect>();
    }

    IEnumerator Start()
    {
        //Pull info from Parse
        if (PlayerPrefs.HasKey("Items"))
        {
            var items = PlayerPrefs.GetString("Items");
            JsonUtility.FromJsonOverwrite(items, Options);
        }

        /*
        var query = ParseObject.GetQuery("Items");
        bool doneSetup = false;
        query.FindAsync().ContinueWith(t =>
        {
            if (t.IsFaulted || t.IsCanceled)
                Debug.Log("Could not get Item List from Parse");
            else
            {
                IEnumerable<ParseObject> results = t.Result;
                foreach(var v in results)
                {
                    ArmorOption s = ParseObj(v);
                    Options.Add(s);
                }
                doneSetup = true;
            }
        });
        while (!doneSetup)
        {
            yield return true;
        }
        */
        
        while(PlayerProfile.Profile.ArmorOptions.Count < 1)
        {
            yield return true;
        }
        CheckOwned();
        SetupList();
        InvokeRepeating("CheckMasked", 0.25f, 0.025f);
    }

    public void OnOpen()
    {
        open = true;
        scroll.enabled = true;
        foreach (var v in Items)
        {
            v.ToggleItem(true);
        }
        Sort();
    }

    public void OnClose()
    {
        open = false;
        scroll.enabled = false;
        foreach(var v in Items)
        {
            v.ToggleItem(false);
        }
    }

    void CheckOwned()
    {
        if (Armory.instance != null)
        {
            foreach (var ao in Armory.instance.GetOptions())
            {
                bool hasItem = false;
                foreach(var v in Options)
                {
                    if (v.Duplicate(ao))
                        hasItem = true;
                }
                if (!hasItem && ao.ItemName != "None")
                    Options.Add(ao);
            }
        }
    }

    void SetupList()
    {
        Items = new List<ItemOptionUI>();
        foreach(var v in Options)
        {
            ItemOptionUI nI = new ItemOptionUI();
            GameObject newPref = Instantiate(ItemPrefab, ItemPrefab.transform.parent);
            newPref.SetActive(true);
            nI.Setup(v, newPref);
            nI.SetColor(OwnedColor, UnownedColor);
            Items.Add(nI);
        }
        Sort();
    }

    void Sort()
    {
        Items.Sort((x, y) =>
        {
            int owned = y.owned.CompareTo(x.owned) * 10000;
            int type = ((int)x.item.Type).CompareTo((int)y.item.Type)*1000;
            int rarity = x.item.rarity.CompareTo(y.item.rarity);
            return (owned + type + rarity);
        });
        foreach(var v in Items)
        {
            v.background.transform.SetAsLastSibling();
        }
    }
    /*
    ArmorOption ParseObj(ParseObject obj)
    {
        string Name = "Armor Name";
        if (obj.ContainsKey("Name"))
            Name = obj.Get<string>("Name");
        string Colors = " ";
        if (obj.ContainsKey("Colors"))
            Colors = obj.Get<string>("Colors");
        int Rarity = 0;
        if (obj.ContainsKey("Rarity"))
            Rarity = obj.Get<int>("Rarity");
        int ItemType = 0;
        if (obj.ContainsKey("ItemType"))
            ItemType = obj.Get<int>("ItemType");
        int Meshes = 0;
        if (obj.ContainsKey("Meshes"))
            Meshes = obj.Get<int>("Meshes");
        int EffectID = 0;
        if (obj.ContainsKey("EffectID"))
            EffectID = obj.Get<int>("EffectID");
        string ArmorString = Name+"%" + ItemType + "%" + Meshes + "%" + Colors + "%"+Rarity + "|" + EffectID + "^0";
        ArmorOption a = new ArmorOption(ArmorString);
        return a;
    }
    */

    void CheckMasked()
    {
        if(open)
        {
            float scrollAmt = ScrollContent.localPosition.y;
            foreach(var o in Items)
            {
                float y = o.GetPos();
                float diff = y + scrollAmt;
                float opacity = 1;
                if (diff < minY)
                    opacity = 1-((minY - diff)) / range;
                else if (diff > maxY)
                    opacity = 1-((diff - maxY) / range);
                o.SetDissolveAmt(opacity);
            }
        }
    }

    [System.Serializable]
    public class ItemOptionUI
    {
        public ArmorOption item;
        public bool owned;
        ItemOrbDisplay display;
        Text title;
        public Image background;
        BeautifulDissolves.Dissolve[] dissolves;
        List<SkinnedMeshRenderer> meshes;
        ParticleSystem[] particles;

        public void ToggleItem(bool val)
        {
            display.gameObject.SetActive(val);
        }

        public void Setup(ArmorOption ao, GameObject element)
        {
            item = ao;
            display = element.GetComponentInChildren<ItemOrbDisplay>();
            title = element.GetComponentInChildren<Text>();
            background = element.GetComponent<Image>();
            title.text = item.ItemName;
            title.color = ItemDatabase.v.RarityColors[item.rarity];
            dissolves = new BeautifulDissolves.Dissolve[] { };
            if(display != null)
            {
                display.Setup(item);
                dissolves = display.GetComponentsInChildren<BeautifulDissolves.Dissolve>();
                meshes = new List<SkinnedMeshRenderer>();
                SkinnedMeshRenderer[] m = display.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var v in m)
                {
                    bool hasDissolve = false;
                    foreach (var d in dissolves)
                    {
                        if (d.gameObject == v.gameObject)
                            hasDissolve = true;
                    }
                    if (!hasDissolve)
                        meshes.Add(v);
                }
                particles = display.GetComponentsInChildren<ParticleSystem>();
            }           
            CheckOwned();
        }

        public void SetDissolveAmt(float amt)
        {
            if (amt <= 0)
                amt = -1;
            if (dissolves.Length > 0)
            {
                foreach(var v in dissolves)
                {
                    v.SetDissolveAmount(1-amt);
                }
            }
            foreach(var v in particles)
            {
                v.gameObject.SetActive(amt > 0 && owned);
            }
            if(meshes != null)
            {
                foreach(var v in meshes)
                {
                    v.enabled = amt > 0;
                }
            }
        }

        public float GetPos()
        {
            return background.transform.localPosition.y;
        }

        public void CheckOwned()
        {
            owned = Armory.instance.HasDuplicate(item);
        }

        public void SetColor(Color OwnColor, Color UnownedColor)
        {
            if(background != null)
            {
                if (owned)
                    background.color = OwnColor;
                else
                    background.color = UnownedColor;
            }
        }

        public override string ToString()
        {
            if (item != null)
                return item.ItemName;
            return "__Item Name__";
        }
        
        void OnDestroy()
        {
            if(background != null && Application.isPlaying)
                Destroy(background.gameObject);
        }
    }
}
