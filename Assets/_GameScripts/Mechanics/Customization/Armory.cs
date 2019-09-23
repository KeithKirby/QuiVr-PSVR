using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Parse;
using SimpleJSON;
using System.Linq;
using UnityEngine.Events;

public class Armory : MonoBehaviour
{
    public static Outfit currentOutfit;
    public static Armory instance;
    public bool updated;

    public bool s_giveAllItems = true;

    public List<ArmorOption> Options
    {
        get
        {
            return PlayerProfile.Profile.ArmorOptions;
        }
    }

    public static bool ValidFetch = false;

    [System.Serializable]
    public class ItemEvent : UnityEvent<ArmorOption> { }
    public ItemEvent OnEquip;

    int Resource;
    private byte[] iv;
    private byte[] key;

    IEnumerator Start()
    {
        toEquip = new List<ArmorOption>();

        while (!PlayerProfile.Ready)
            yield return null;
        
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        if (currentOutfit == null)
            currentOutfit = new Outfit();
        //PlayerProfile.Profile.ArmorOptions.Clear();
        iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        Reshuffle();
    }

    public ArmorOption[] GetOptions()
    {
        return PlayerProfile.Profile.ArmorOptions.ToArray();
    }

    public ArmorOption[] GetOptions(ItemType type)
    {
        List<ArmorOption> opts = new List<ArmorOption>();
        foreach (var v in PlayerProfile.Profile.ArmorOptions)
        {
            if (v.Type == type)
                opts.Add(v);
        }
        return opts.ToArray();
    }

    public int CurrentIndex(ItemType type)
    {
        ArmorOption[] opts = GetOptions(type);
        ArmorOption worn = Worn(type);
        if (worn != null)
        {
            for (int i = 0; i < opts.Length; i++)
            {
                if (opts[i].Duplicate(worn))
                    return i;
            }
        }
        return -1;
    }

    public void FetchFromServer()
    {
        Debug.Log("FetchFromServer");
        ValidFetch = false;

        currentOutfit = new Outfit();

        //Get current Resource        
        Resource = PlayerProfile.Profile.ArmourResource;

        //Get current outfit
        //Get item option
        bool hasBase = HasBaseItems(Options.ToArray());
        ValidFetch = true;

        if (PlayerProfile.Profile.ArmorOptions.Count == 0)
        {
            // AddAllItems(); // Cheatz
            AddBaseArmor();
        }

        string s = PlayerProfile.Profile.Outfit;
        Debug.LogFormat("PlayerProfile Outfit {0}", s);
        if (s.Length > 1 && hasBase)
        {
            List<ArmorOption> outfit = ArmorOption.ItemList(s);
            foreach (var v in outfit)
            {
                if (OwnsItem(v))
                    toEquip.Add(v);
                else
                {
                    ArmorOption baseItem = BaseItem(v.Type);
                    if (baseItem != null)
                        toEquip.Add(baseItem);
                }
            }
        }
        else
        {
            foreach (var v in CurrentOutfitList())
            {
                toEquip.Add(v);
            }
            SaveOutfit();
            Debug.Log("No Existing Outfit: Giving Base Armor Outfit");
        }
        updated = true;
    }

    /*
    public void FetchFromServer()
    {
        ValidFetch = false;
        try
        {
            if(User.ArcadeMode && User.ArcadeUser != null)
            {
                Setup(User.ArcadeUser);
            }
            else if(ParseUser.CurrentUser != null)
            {
                ParseUser.CurrentUser.FetchAsync().ContinueWith(t =>
                {
                    if(!t.IsFaulted && !t.IsCanceled)
                    {
                        currentOutfit = new Outfit();
                        Options = new List<ArmorOption>();
                        var res = t.Result;
                        //Get current Resource
                        res.TryGetValue<int>("Resource", out Resource);
                        //Get current outfit
                        //Get item option
                        bool hasBase = false;
                        ValidFetch = true;
                        if (res.ContainsKey("Inventory"))
                        {
                            string s = res.Get<string>("Inventory");
                            if(ArmorOption.ItemList(s).Count > 0 && HasBaseItems(ArmorOption.ItemList(s).ToArray()))
                            {
                                Options = ArmorOption.ItemList(s);
                                string asp = "-- Loaded Player Items from Server --\n";
                                foreach(var v in Options)
                                {
                                    asp += v.ToString() + "   ";
                                }
                                Debug.Log(asp);
                                hasBase = true;
                            }
                            else
                            {
                                Debug.Log("No items or Base Items not present: " + s);                    
                                AddBaseArmor();
                            }

                        }
                        else
                        {
                            AddBaseArmor();
                            Debug.Log("No Existing Armor: Giving Base Armor Set");
                        }
                        if (res.ContainsKey("Outfit"))
                        {
                            string s = res.Get<string>("Outfit");
                            if (s.Length > 1 && hasBase)
                            {
                                List<ArmorOption> outfit = ArmorOption.ItemList(s);
                                foreach (var v in outfit)
                                {
                                    if (OwnsItem(v))
                                        toEquip.Add(v);
                                    else
                                    {
                                        ArmorOption baseItem = BaseItem(v.Type);
                                        if(baseItem != null)
                                            toEquip.Add(baseItem);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var v in CurrentOutfitList())
                                {
                                    toEquip.Add(v);
                                }
                                SaveOutfit();
                                Debug.Log("No Existing Outfit: Giving Base Armor Outfit");
                            }
                        }
                        else
                        {
                            foreach (var v in CurrentOutfitList())
                            {
                                toEquip.Add(v);
                            }
                            SaveOutfit();
                        }
                        updated = true;
                    }
                    else
                    {
                        foreach (var e in t.Exception.InnerExceptions)
                        {
                            ParseException parseException = (ParseException)e;
                            Debug.Log("Error Updating Inventory Information: " + parseException.Message);
                        }
                    }
                });
            }
        }
        catch {
            Debug.Log("Parse Error Catch - Armory Fetch");        
        } 
    }
    
    void Setup(ParseObject res)
    {
        currentOutfit = new Outfit();
        Options = new List<ArmorOption>();
        //Get current Resource
        res.TryGetValue<int>("Resource", out Resource);
        //Get current outfit
        //Get item option
        bool hasBase = false;
        ValidFetch = true;
        if (res.ContainsKey("Inventory"))
        {
            string s = res.Get<string>("Inventory");
            if (ArmorOption.ItemList(s).Count > 0 && HasBaseItems(ArmorOption.ItemList(s).ToArray()))
            {
                Options = ArmorOption.ItemList(s);
                hasBase = true;
            }
            else
            {
                AddBaseArmor();
            } 
        }
        else
        {
            AddBaseArmor();
        }
        if (res.ContainsKey("Outfit"))
        {
            string s = res.Get<string>("Outfit");
            if (s.Length > 1 && hasBase)
            {
                List<ArmorOption> outfit = ArmorOption.ItemList(s);
                foreach (var v in outfit)
                {
                    toEquip.Add(v);
                }
            }
            else
            {
                foreach (var v in CurrentOutfitList())
                {
                    toEquip.Add(v);
                }
                SaveOutfit();
            }  
        }
        else
        {
            foreach (var v in CurrentOutfitList())
            {
                toEquip.Add(v);
            }
            SaveOutfit();
        }            
        updated = true;
    }
    */

    //Debug to fix Alpha items
    bool HasBaseItems(ArmorOption[] arms)
    {
        if (arms.Length < 7)
            return false;
        foreach (var v in arms)
        {
            if (v.Duplicate(new ArmorOption("Plain Mask", ItemType.Helmet, 0, null)))
            {
                return true;
            }
        }
        return false;
    }

    public void AddItemFromGenerator(ArmorOption item)
    {
        AddItem(item, true, true);
    }

    public void AddItem(ArmorOption item, bool save = true, bool notify = true)
    {
        if (item == null)
        {
            return;
        }
        if (CheckDuplicates(item, notify))
            return;
        Options.Add(item);
        if (save)
            AddItemSync(item.ToString());
        if (notify)
            Notification.Notify(new Note(item, !save));
        if (!HasItem(item.Type))
            EquipItem(item);
    }

    public void RemoveItem(ArmorOption item, bool save = true)
    {
        for (int i = Options.Count - 1; i >= 0; i--)
        {
            var v = Options[i];
            if (item.Equals(v))
                Options.RemoveAt(i);
        }
        if (save)
        {
            var profile = PlayerProfile.Profile;
            profile.Save(PlayerProfile.SaveCategory.Default);
        }
    }

    void AddItemSync(string item)
    {
        var profile = PlayerProfile.Profile;
        profile.Items.Add(item);
        profile.Save(PlayerProfile.SaveCategory.Default);
    }
	
	void ReplaceItemSync(string oldItem, string newItem, System.Action<bool> callback = null)
    {
        var profile = PlayerProfile.Profile;
        profile.Items.Remove(oldItem);
        profile.Items.Add(newItem);
        profile.Save(PlayerProfile.SaveCategory.Default);
        if(null != callback)
            callback(true);

        /*bool arcade = User.ArcadeMode && User.ArcadeUser != null;
        if (ParseUser.CurrentUser != null)
        {
            Dictionary<string, object> ItemDB = new Dictionary<string, object>();
            string userID = arcade ? User.ArcadeUser.ObjectId : ParseUser.CurrentUser.ObjectId;
            ItemDB.Add("userid", userID);
            ItemDB.Add("arcade", arcade);
            ItemDB.Add("olditem", oldItem);
            ItemDB.Add("newitem", newItem);
            ParseCloud.CallFunctionAsync<string>("ReplaceItemV3", ItemDB).ContinueWith(t =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    Debug.Log("Item Replacement Success: " + t.Result);
                    if(callback != null)
                        UnityMainThreadDispatcher.Instance().Enqueue(() => callback(true));
                }
                else
                {
                    Note n = new Note("Error Saving Inventory", "Database Inventory Save Unsuccessful", "", AppBase.v.ErrorIcon);
                    Notification.Notify(n);
                    foreach (var e in t.Exception.InnerExceptions)
                    {
                        ParseException parseException = (ParseException)e;
                        Debug.Log("Unsuccessful Item Replacement Save: " + parseException.Message);
                    }
                    if (callback != null)
                        UnityMainThreadDispatcher.Instance().Enqueue(() => callback(false));
                }
            });
        }
        */
    }

    bool CheckDuplicates(ArmorOption item, bool notify = true)
    {
        bool hasDupe = false;
        var armourOptions = PlayerProfile.Profile.ArmorOptions;
        for (int i = 0; i < armourOptions.Count; i++)
        {
            if (item.Duplicate(armourOptions[i]))
            {
                hasDupe = true;
                ArmorOption old = armourOptions[i].Worse(item);
                ArmorOption nw = armourOptions[i].Better(item);
                if (item.Equals(nw))
                {
                    armourOptions[i] = nw;
                    PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);
                }
                int diffAmt = 0;
                if (GameBase.instance != null && GameBase.instance.Difficulty > 5)
                    diffAmt = ItemGenerator.GetResource(GameBase.instance.Difficulty);
                int value = ItemDatabase.v.ResourceValues[item.rarity];
                value += diffAmt;
                ChangeResource(value);
                if (notify)
                {
                    Notification.Notify(Note.DuplicateResource(value, item));
                    if (MoteParticles.instance != null)
                        MoteParticles.instance.SpawnMotes(value);
                }
            }
        }        
        return hasDupe;
    }

    public bool HasDuplicate(ArmorOption item)
    {
        for (int i = 0; i < Options.Count; i++)
        {
            if (item.Duplicate(Options[i]))
            {
                return true;
            }
        }
        return false;
    }

    public void GiveResource(int value, bool notify = true)
    {
        if (notify)
            Notification.Notify(Note.GainResource(value));
        ChangeResource(value);
    }

    public void RemoveResource(int val, bool notify = true)
    {
        if (notify)
            Notification.Notify(Note.GainResource(-1 * val));
        ChangeResource(-1 * val);
    }

    List<ArmorOption> toEquip;
    void Update()
    {
        if (toEquip.Count > 0)
        {
            ArmorOption o = toEquip[0];
            EquipItem(o, false);
            toEquip.RemoveAt(0);
        }
    }

    public int GetResource()
    {
        return Resource;
    }

    void ChangeResource(int delta)
    {
        Resource += delta;
        if (ValidFetch)
        {
            //if (User.ArcadeMode && User.ArcadeUser != null)
            //{
            //User.ArcadeUser.Increment("Resource", delta);
            //User.TrySave();
            //}
            //else
            //{
            //                ParseUser.CurrentUser.Increment("Resource", delta);
            //ParseUser.CurrentUser.SaveAsync();
            //}
            PlayerProfile.Profile.ArmourResource = Resource;
            PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);
        }
        else
        {
            Note n = new Note();
            n.title = "Error Saving";
            n.body = "User Data Corrupted, please restart";
            n.icon = AppBase.v.ErrorIcon;
            Notification.Notify(n);
        }
    }

    /*
    public void SaveArmor()
    {
        try
        {
            if(ValidFetch)
            {
                if(User.ArcadeMode && User.ArcadeUser != null)
                {
                    string opts = ArmorOption.ListString(Options.ToArray());
                    User.ArcadeUser["Resource"] = Resource;
                    User.ArcadeUser["Inventory"] = opts;
                    User.TrySave();
                }
                else if (ParseUser.CurrentUser != null)
                {
                    string opts = ArmorOption.ListString(Options.ToArray());
                    ParseUser.CurrentUser["Resource"] = Resource;
                    ParseUser.CurrentUser["Inventory"] = opts;
                    ParseUser.CurrentUser.SaveAsync();
                }
            }
            else
            {
                Note n = new Note();
                n.title = "Error Saving";
                n.body = "User Data Corrupted, please restart";
                n.icon = AppBase.v.ErrorIcon;
                Notification.Notify(n);
            }          
        }
        catch
        {
            Debug.LogError("Could not save inventory: User Null");
        }
    }
    */

    public void SaveOutfit()
    {
        try
        {
            if (ValidFetch)
            {
                /*
                if (User.ArcadeMode && User.ArcadeUser != null)
                {
                    User.ArcadeUser["Outfit"] = CurrentOutfit();
                    User.TrySave();
                }
                else if (ParseUser.CurrentUser != null)
                {
                    ParseUser.CurrentUser["Outfit"] = CurrentOutfit();
                    ParseUser.CurrentUser.SaveAsync();
                }
                */
                PlayerProfile.Profile.Outfit = CurrentOutfit();
                PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);
            }

        }
        catch
        { }
    }

    public static string CurrentOutfit()
    {
        List<ArmorOption> oft = new List<ArmorOption>();
        if (currentOutfit != null)
        {
            if (currentOutfit.Arrow != null)
                oft.Add(currentOutfit.Arrow);
            if (currentOutfit.Helmet != null)
                oft.Add(currentOutfit.Helmet);
            if (currentOutfit.Gloves != null)
                oft.Add(currentOutfit.Gloves);
            if (currentOutfit.Chest != null)
                oft.Add(currentOutfit.Chest);
            if (currentOutfit.BowBase != null)
                oft.Add(currentOutfit.BowBase);
            if (currentOutfit.BowDetail != null)
                oft.Add(currentOutfit.BowDetail);
            if (currentOutfit.Quiver != null)
                oft.Add(currentOutfit.Quiver);
            if (currentOutfit.Gem != null)
                oft.Add(currentOutfit.Gem);
        }
        return ArmorOption.ListString(oft.ToArray());
    }

    public static ArmorOption GetEquipped(ItemType t)
    {
        foreach (var v in CurrentOutfitList())
        {
            if (t == v.Type)
                return v;
        }
        return null;
    }

    public static List<ArmorOption> CurrentOutfitList()
    {
        return ArmorOption.ItemList(CurrentOutfit());
    }

    public void EquipItem(ArmorOption item, bool save = true)
    {
        if (item != null)
        {
            ItemType t = item.Type;
            if (t == ItemType.Arrow)
                currentOutfit.Arrow = item;
            else if (t == ItemType.Helmet)
                currentOutfit.Helmet = item;
            else if (t == ItemType.Gloves)
                currentOutfit.Gloves = item;
            else if (t == ItemType.ChestArmor)
                currentOutfit.Chest = item;
            else if (t == ItemType.Quiver)
                currentOutfit.Quiver = item;
            else if (t == ItemType.Gem)
                currentOutfit.Gem = item;
            else if (t == ItemType.BowBase)
                currentOutfit.BowBase = item;
            else if (t == ItemType.BowDetail)
                currentOutfit.BowDetail = item;
            if (PlayerArmor.instance != null)
                PlayerArmor.instance.Equip(item);
            OnEquip.Invoke(item);
            curEffects = null;
            StatCheck.CheckArmor(currentOutfit);
            if (save)
                SaveOutfit();
        }
    }

    static List<EffectInstance> curEffects;
    public static List<EffectInstance> ArmorEffects()
    {
        if (curEffects == null)
        {
            curEffects = new List<EffectInstance>();
            if (currentOutfit.Arrow != null && currentOutfit.Arrow.Effect.Length > 1)
                curEffects.Add(new EffectInstance(currentOutfit.Arrow.Effect));
            if (currentOutfit.Helmet != null && currentOutfit.Helmet.Effect.Length > 1)
                curEffects.Add(new EffectInstance(currentOutfit.Helmet.Effect));
            if (currentOutfit.Gloves != null && currentOutfit.Gloves.Effect.Length > 1)
                curEffects.Add(new EffectInstance(currentOutfit.Gloves.Effect));
            if (currentOutfit.Gem != null && currentOutfit.Gem.Effect.Length > 1)
                curEffects.Add(new EffectInstance(currentOutfit.Gem.Effect));
            if (currentOutfit.BowBase != null && currentOutfit.BowBase.Effect.Length > 1)
                curEffects.Add(new EffectInstance(currentOutfit.BowBase.Effect));
            if (currentOutfit.Quiver != null && currentOutfit.Quiver.Effect.Length > 1)
                curEffects.Add(new EffectInstance(currentOutfit.Quiver.Effect));
            if (currentOutfit.BowDetail != null && currentOutfit.BowDetail.Effect.Length > 1)
                curEffects.Add(new EffectInstance(currentOutfit.BowDetail.Effect));
            if (currentOutfit.Chest != null && currentOutfit.Chest.Effect.Length > 1)
                curEffects.Add(new EffectInstance(currentOutfit.Chest.Effect));
        }
        return curEffects;
    }

    public static bool HasEffectEquipped(int id)
    {
        foreach (var v in ArmorEffects())
        {
            if (v.EffectID == id)
                return true;
        }
        return false;
    }

    public void Reshuffle()
    {
        iv = ItemGenerator.Shuffle(iv);
        key = ItemGenerator.Shuffle(key);
    }

    public byte[] GetIV()
    {
        return iv;
    }

    public byte[] GetKey()
    {
        return key;
    }

    public bool HasItem(ItemType t)
    {
        foreach (var v in CurrentOutfitList())
        {
            if (v.Type == t)
                return true;
        }
        return false;
    }

    public ArmorOption Worn(ItemType t)
    {
        foreach (var v in CurrentOutfitList())
        {
            if (v.Type == t)
            {
                return v;
            }
        }
        return null;
    }

    public void ReplaceItem(ArmorOption o, System.Action<bool> callback = null)
    {
        ArmorOption old = null;
        for (int i = 0; i < Options.Count; i++)
        {
            var v = Options[i];
            if (v.Duplicate(o))
            {
                Options[i] = o;
                old = v;
            }
        }
        if (old != null)
            ReplaceItemSync(old.ToString(), o.ToString(), callback);
    }

    void AddBaseArmor()
    {
        List<ArmorOption> baseItems = new List<ArmorOption>();
        baseItems.Add(BaseItem(ItemType.Arrow));
        baseItems.Add(BaseItem(ItemType.ChestArmor));
        baseItems.Add(BaseItem(ItemType.BowBase));
        baseItems.Add(BaseItem(ItemType.Gloves));
        baseItems.Add(BaseItem(ItemType.Quiver));
        baseItems.Add(BaseItem(ItemType.BowDetail));
        baseItems.Add(BaseItem(ItemType.Helmet));
        baseItems.Add(BaseItem(ItemType.Gem));
        foreach (var v in baseItems)
        {
            AddItem(v, false, false);
        }
        //AddItem(BaseItem(ItemType.Helmet), true, false);
        AddItemSync(ArmorOption.ListString(baseItems.ToArray()));
    }

    void AddAllItems()
    {
        List<ArmorOption> baseItems = new List<ArmorOption>();

        var itemGen = GameObject.FindObjectOfType<ItemGenerator>();
        foreach (var drp in itemGen.ValidDrops)
        {
            var item = ItemGenerator.CreateItem(drp);
            baseItems.Add(item);
        }

        baseItems.Add(BaseItem(ItemType.Arrow));
        baseItems.Add(BaseItem(ItemType.ChestArmor));
        baseItems.Add(BaseItem(ItemType.BowBase));
        baseItems.Add(BaseItem(ItemType.Gloves));
        baseItems.Add(BaseItem(ItemType.Quiver));
        baseItems.Add(BaseItem(ItemType.BowDetail));
        baseItems.Add(BaseItem(ItemType.Helmet));
        baseItems.Add(BaseItem(ItemType.Gem));

        foreach (var v in baseItems)
        {
            AddItem(v, false, false);
        }
        AddItemSync(ArmorOption.ListString(baseItems.ToArray()));
    }

    public static ArmorOption BaseItem(ItemType type)
    {
        if (type == ItemType.Arrow)
            return new ArmorOption("Starwood Arrow", ItemType.Arrow, 0, null);
        else if (type == ItemType.ChestArmor)
            return new ArmorOption("Recruit's Chestplate", ItemType.ChestArmor, 0, null);
        else if (type == ItemType.BowBase)
            return new ArmorOption("Starwood Bow", ItemType.BowBase, 0, null);
        else if (type == ItemType.Gloves)
            return new ArmorOption("Forceful Mitts", ItemType.Gloves, 0, null, "2^0", 0);
        else if (type == ItemType.Quiver)
            return new ArmorOption("Starspun Quiver", ItemType.Quiver, 0, null);
        else if (type == ItemType.BowDetail)
            return new ArmorOption("None", ItemType.BowDetail, 0, null);
        else if (type == ItemType.Helmet)
            return new ArmorOption("Plain Mask", ItemType.Helmet, 0, null);
        else if (type == ItemType.Gem)
            return new ArmorOption("Empty Socket", ItemType.Gem, 0, null);
        return null;
    }

    public bool OwnsItem(ArmorOption o)
    {
        foreach (var v in Options)
        {
            if (v.Equals(o))
                return true;
        }
        return false;
    }

}

[System.Serializable]
public class Outfit
{
    public ArmorOption Helmet;
    public ArmorOption Chest;
    public ArmorOption Gloves;
    public ArmorOption Arrow;
    public ArmorOption BowBase;
    public ArmorOption Quiver;
    public ArmorOption BowDetail;
    public ArmorOption Gem;

    public Outfit()
    {
        Helmet = Armory.BaseItem(ItemType.Helmet);//new ArmorOption("Plain Mask", ItemType.Helmet, 0, null);
        Chest = Armory.BaseItem(ItemType.ChestArmor);//new ArmorOption("Recruit's Chestplate", ItemType.ChestArmor, 0, null);
        Gloves = Armory.BaseItem(ItemType.Gloves);// new ArmorOption("Forceful Mitts", ItemType.Gloves, 1, null, "2^0", 0);
        Arrow = Armory.BaseItem(ItemType.Arrow);//new ArmorOption("Starwood Arrow", ItemType.Arrow, 0, null);
        BowBase = Armory.BaseItem(ItemType.BowBase);//new ArmorOption("Starwood Bow", ItemType.BowBase, 0, null);
        BowDetail = Armory.BaseItem(ItemType.BowDetail);//new ArmorOption("None", ItemType.BowDetail, 0, null);
        Quiver = Armory.BaseItem(ItemType.Quiver);//new ArmorOption("Starspun Quiver", ItemType.Quiver, 0, null);
        Gem = Armory.BaseItem(ItemType.Gem);//new ArmorOption("Empty Socket", ItemType.Gem, 0, null);
    }

    public List<ArmorOption> toList()
    {
        List<ArmorOption> opts = new List<ArmorOption>();
        opts.Add(Helmet);
        opts.Add(Chest);
        opts.Add(Gloves);
        opts.Add(Arrow);
        opts.Add(BowBase);
        opts.Add(Quiver);
        //opts.Add(BowDetail);
        //opts.Add(Gem);
        return opts;
    }

    public void Replace(ArmorOption item)
    {
        if (item.Type == ItemType.Helmet)
            Helmet = item;
        else if (item.Type == ItemType.Gloves)
            Gloves = item;
        else if (item.Type == ItemType.ChestArmor)
            Chest = item;
        else if (item.Type == ItemType.BowBase)
            BowBase = item;
        else if (item.Type == ItemType.Arrow)
            Arrow = item;
        else if (item.Type == ItemType.Quiver)
            Quiver = item;
        else if (item.Type == ItemType.BowDetail)
            BowDetail = item;
    }

    public bool HasEffectID(int id)
    {
        if (Helmet.GetEffect().EffectID == id)
            return true;
        else if (Chest.GetEffect().EffectID == id)
            return true;
        else if (Gloves.GetEffect().EffectID == id)
            return true;
        else if (Quiver.GetEffect().EffectID == id)
            return true;
        else if (Arrow.GetEffect().EffectID == id)
            return true;
        else if (BowBase.GetEffect().EffectID == id)
            return true;
        return false;
    }

    public EffectInstance GetEffect(int id)
    {
        if (Helmet.GetEffect().EffectID == id)
            return Helmet.GetEffect();
        else if (Chest.GetEffect().EffectID == id)
            return Chest.GetEffect();
        else if (Gloves.GetEffect().EffectID == id)
            return Gloves.GetEffect();
        else if (Quiver.GetEffect().EffectID == id)
            return Quiver.GetEffect();
        else if (Arrow.GetEffect().EffectID == id)
            return Arrow.GetEffect();
        else if (BowBase.GetEffect().EffectID == id)
            return BowBase.GetEffect();
        return null;
    }
}

[System.Serializable]
public enum ItemType
{
    Arrow,
    ChestArmor,
    Helmet,
    Gloves,
    BowBase,
    BowDetail,
    Quiver,
    Gem
}

[System.Serializable]
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Artifact,
    Unique
}

[System.Serializable]
public class ArmorOption
{
    public string ItemName = "Undefined";
    public ItemType Type = ItemType.Helmet;
    public int rarity = 0;
    public int ObjectID;
    public Color[] Colors = { };
    public string Effect = "0^0";
    public int ancient = 0;
    [HideInInspector]
    public EffectInstance eft = null;

    public ArmorOption(string name, ItemType type, int objID, Color[] colors, string effects = "0^0", int rare = 0, int isancinet = 0)
    {
        ItemName = name;
        ObjectID = objID;
        if (colors != null)
            Colors = colors;
        Type = type;
        Effect = effects;
        GetEffect();
        rarity = rare;
        ancient = isancinet;
        CheckEffect();
    }

    public ArmorOption(string inpt)
    {
        string[] eSplit = inpt.Split('|');
        if (eSplit.Length == 2)
        {
            Effect = eSplit[1];
            GetEffect();
            string[] values = eSplit[0].Split('%');
            if (values.Length > 3)
            {
                ItemName = values[0];
                int type = 0;
                int.TryParse(values[1], out type);
                Type = (ItemType)type;
                GetObjectID(values[2]);
                GetColors(values[3]);
                if (values.Length > 4)
                    int.TryParse(values[4], out rarity);
                if (values.Length > 5)
                    int.TryParse(values[5], out ancient);
            }
            CheckEffect();
        }
    }

    public bool Duplicate(ArmorOption o)
    {
        if (o.rarity != rarity || Type != o.Type || !ItemName.ToLower().Trim().Equals(o.ItemName.ToLower().Trim()) || ObjectID != o.ObjectID)
        {
            return false;
        }
        if (GetEffect().EffectID != o.GetEffect().EffectID)
        {
            return false;
        }
        return true;
    }

    public EffectInstance GetEffect()
    {
        if (eft == null || (eft.EffectID == 0 && Effect.Length > 1 && Effect[0] != '0'))
        {
            if (Effect.Length < 3)
                eft = new EffectInstance("0^0");
            else
                eft = new EffectInstance(Effect);
        }
        return eft;
    }

    public ArmorOption Better(ArmorOption o)
    {
        //Check Ancient Value
        if (o.ancient > ancient)
            return o;
        else if (ancient > o.ancient)
            return this;

        //Check Ability Value
        EffectInstance current = new EffectInstance(Effect);
        EffectInstance check = new EffectInstance(o.Effect);
        bool CurrentBetter = false;
        EffectInstance checkEffect = o.GetEffect();
        ItemEffect eft = ItemDatabase.GetEffect(checkEffect.EffectID);
        if (eft.randomType == RandomType.Recharge && current.EffectValue <= check.EffectValue)
            CurrentBetter = true;
        else if (eft.randomType != RandomType.Recharge && current.EffectValue >= check.EffectValue)
            CurrentBetter = true;
        if (CurrentBetter)
            return this;
        return o;
    }

    public ArmorOption Worse(ArmorOption o)
    {
        if (Better(o).Equals(o))
            return this;
        return o;
    }

    public ArmorOption(JSONNode node)
    {
        ItemName = node["Name"].Value;
        ObjectID = node["Meshes"].AsInt;
        GetColors(node["Colors"].Value);
        Type = (ItemType)node["ItemType"].AsInt;
        int effectID = node["EffectID"].AsInt;
        rarity = node["Rarity"].AsInt;
        if (ItemPedestal.CanReroll(effectID, rarity))
            ancient = ItemDatabase.RollAncient();
        Effect = ItemDatabase.RandomEffect(effectID, ancient);
        GetEffect();
        CheckEffect();
    }

    public void CheckEffect()
    {
        if (ItemDatabase.v == null)
            return;
        EffectInstance e = GetEffect();
        ItemEffect effect = ItemDatabase.GetEffect(e.EffectID);
        Vector2 Range = effect.Range;
        if (ancient > 0)
            Range = effect.AncientRange;
        if ((e.EffectValue > Range.y && ancient < 2) || e.EffectValue > Range.y + 1)
        {
            if (ancient < 2)
                e.EffectValue = Range.y;
            else
                e.EffectValue = effect.PrimalVal;
        }
        else if (e.EffectValue < Range.x)
            e.EffectValue = Range.x;
        if (ancient == 2)
            e.EffectValue = effect.PrimalVal;
        Effect = e.ToString();
        eft = e;
    }

    void GetObjectID(string id)
    {
        int x = 0;
        int.TryParse(id, out x);
        ObjectID = x;
    }

    void GetColors(string ids)
    {
        List<Color> clrs = new List<Color>();
        if (ids.Length < 3)
        {
            Colors = clrs.ToArray();
            return;
        }
        string[] ColorBlocks = ids.Split(':');
        foreach (var cb in ColorBlocks)
        {
            string[] vals = cb.Split(',');
            float r = 0, g = 0, b = 0, a = 0;
            if (vals.Length > 3)
            {
                float.TryParse(vals[0], out r);
                float.TryParse(vals[1], out g);
                float.TryParse(vals[2], out b);
                float.TryParse(vals[3], out a);
            }
            clrs.Add(new Color(r, g, b, a));
        }
        Colors = clrs.ToArray();
    }

    public string GetName(bool richText = true)
    {
        string iname = I2.Loc.ScriptLocalization.Get("Items/" + ItemName);
        if (iname.Length < 2)
            iname = ItemName;
        if (ancient == 1)
        {
            if (richText)
                return "<color=#6d3a00>Hallowed</color> " + iname;
            else
                return "Hallowed " + iname;
        }
        else if (ancient == 2)
        {
            if (richText)
                return "<color=#ba6300>Primordial</color> " + iname;
            else
                return "Primordial " + iname;
        }

        return iname;
    }

    public override string ToString()
    {
        string s = ItemName;
        string cls = " ";
        if (Colors != null)
        {
            foreach (var v in Colors)
            {
                cls += v.r + "," + v.g + "," + v.b + "," + v.a + ":";
            }
        }
        if (cls.Length > 1)
            cls = cls.Substring(0, cls.Length - 1);
        s += "%" + (int)Type + "%" + ObjectID + "%" + cls + "%" + rarity + "%" + ancient;
        s += "|" + Effect;
        return s;
    }

    public static string ListString(ArmorOption[] items)
    {
        string s = "";
        foreach (var v in items)
        {
            s += v.ToString() + "$";
        }
        if (s.Length < 2)
            return "";
        return s.Substring(0, s.Length - 1);
    }

    public static List<ArmorOption> ItemList(string s)
    {
        List<ArmorOption> list = new List<ArmorOption>();
        if (s.Length < 2)
            return list;
        string[] itms = s.Split('$');
        foreach (var v in itms)
        {
            if (v.Length > 5)
                list.Add(new ArmorOption(v));
        }
        return list;
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(ArmorOption))
        {
            ArmorOption o = (ArmorOption)obj;
            return ItemName.Equals(o.ItemName) && o.Type == Type && o.GetEffect().EffectID == GetEffect().EffectID;
        }
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return ItemName.GetHashCode() * 17 * GetEffect().EffectID.GetHashCode();
    }
}

[System.Serializable]
public class ItemEffect
{
    public string EffectName;
    [TextArea(4, 10)]
    public string EffectDetail;
    public int EffectID;
    public AbilityDisplay Display;
    public Vector2 Range;
    public Vector2 AncientRange;
    public float PrimalVal;
    public float StaticValue;
    public float VariableValue;
    public GameObject EffectActivation;
    public EffectType type = EffectType.None;
    public ActivationType activation = ActivationType.None;
    public RandomType randomType = RandomType.RandomChance;

    public float GenerateValue(float preVal = -1, int Ancient = 0)
    {
        System.Random rand = new System.Random();
        float min = Range.x; float max = Range.y;
        if (Ancient > 0)
        {
            min = AncientRange.x; max = AncientRange.y;
        }
        if (Ancient > 1)
        {
            return PrimalVal;
        }
        if (preVal > min && preVal <= max && Ancient == 0)
        {
            if (Random.Range(0, 100) > 75)
                return (float)System.Math.Round(Random.Range(preVal, max), 2);//return rand.Next((int)(preVal*100f), (int)(max*100f))/100f;
        }
        return (float)System.Math.Round(Random.Range(min, max), 2);
    }

    public string GetEffectName()
    {
        string ename = I2.Loc.ScriptLocalization.Get("Ability/" + EffectName);
        if (ename.Length > 1)
            return ename;
        return EffectName;
    }

    public string GetDetailInfo(float value)
    {
        float val = value;
        if (randomType == RandomType.Recharge)
            val = VariableValue - val;
        else
            val = VariableValue + val;
        string detail = I2.Loc.ScriptLocalization.Get("Ability/" + EffectName + "_Effect");
        if (detail.Length > 1)
            return detail.Replace("%Value%", "" + val);
        return EffectDetail.Replace("%Value%", "" + val);
    }

    public string GetStatInfo(float val, int ancient = 0)
    {
        Vector2 range = Range;
        if (ancient > 0)
            range = AncientRange;
        else if (ancient > 1)
            range = new Vector2(PrimalVal, PrimalVal);
        if (type == EffectType.None)
            return "";
        if (randomType == RandomType.Recharge)
            val = VariableValue - val;
        else
            val = VariableValue + val;
        string top = I2.Loc.ScriptLocalization.Get("Ability/RandomChance") + ": " + val + " %";
        if (randomType == RandomType.Recharge)
            top = I2.Loc.ScriptLocalization.Get("Ability/Recharge") + ": " + val + " " + I2.Loc.ScriptLocalization.Get("Seconds");
        else if (randomType == RandomType.Damage)
            top = I2.Loc.ScriptLocalization.Get("Ability/Damage") + ": " + val + " ";
        else if (randomType == RandomType.Healing)
            top = I2.Loc.ScriptLocalization.Get("Ability/Healing") + ": " + val;
        else if (randomType == RandomType.Duration)
            top = I2.Loc.ScriptLocalization.Get("Ability/Duration") + ": " + val + " " + I2.Loc.ScriptLocalization.Get("Seconds");
        else if (randomType == RandomType.Range)
            top = I2.Loc.ScriptLocalization.Get("Ability/Range") + ": " + val + " " + I2.Loc.ScriptLocalization.Get("Meters");
        if (ancient < 2)
        {
            if (randomType == RandomType.Recharge)
                top += " (" + I2.Loc.ScriptLocalization.Get("Ability/Range") + ":" + (VariableValue - range.y) + "-" + (VariableValue - range.x) + ")\n";
            else
                top += " (" + I2.Loc.ScriptLocalization.Get("Ability/Range") + ":" + (VariableValue + range.x) + "-" + (VariableValue + range.y) + ")\n";
        }
        else
        {
            if (randomType == RandomType.Recharge)
                top += " (" + I2.Loc.ScriptLocalization.Get("Ability/Range") + ":" + (VariableValue - PrimalVal) + ")\n";
            else
                top += " (" + I2.Loc.ScriptLocalization.Get("Ability/Range") + ":" + (VariableValue + PrimalVal) + ")\n";
        }

        string bot = I2.Loc.ScriptLocalization.Get("Ability/Activation") + ": ";
        if (type == EffectType.Orb)
            bot += "Orb - Free Hand Move button";
        else if (type == EffectType.DrawChance)
            bot += "On Arrow Draw";
        else if (type == EffectType.Activation)
            bot += "Bow - Pull T button";
        else if (type == EffectType.MissChance)
            bot += "Chance on Miss";
        else if (type == EffectType.None)
        {
            if (activation == ActivationType.EnemyCritical)
                bot += "Critical Strike";
        }
        else if (type == EffectType.Passive)
            bot += I2.Loc.ScriptLocalization.Get("Ability/Passive");
        return (top + bot);
    }

    public override string ToString()
    {
        return EffectName;
    }
}

public enum EffectType
{
    None,
    DrawChance,
    Orb,
    Activation,
    Passive,
    MissChance
}

public enum ActivationType
{
    None,
    HitEnemy,
    HitAnything,
    HitObject,
    EnemyCritical,
    Orb,
    Fire,
    Draw,
    ArrowMod
}

public enum RandomType
{
    RandomChance,
    Damage,
    Healing,
    Recharge,
    Duration,
    Range
}

public class EffectInstance
{
    public int EffectID;
    public float EffectValue;

    public override string ToString()
    {
        string s = "";
        s += EffectID + "^" + EffectValue;
        return s;
    }

    public EffectInstance(int id, float value)
    {
        EffectID = id;
        EffectValue = value;
    }

    public EffectInstance(string s)
    {
        if (s != null)
        {
            string[] vals = s.Split('^');
            if (vals.Length > 1)
            {
                try
                {
                    int.TryParse(vals[0], out EffectID);
                    string[] pts = vals[1].Split('.');
                    int dig = 0;
                    int dec = 0;
                    if (pts.Length > 0)
                        int.TryParse(pts[0], out dig);
                    if (pts.Length > 1)
                    {
                        string dc = pts[1].Substring(0, 2);
                        int.TryParse(dc, out dec);
                    }
                    EffectValue = (float)dig + (dec / 100f);
                }
                catch { }
                //int.TryParse(vals[0], out EffectID);
                //float.TryParse(vals[1], out EffectValue);
            }
        }
    }

    public static string StringList(EffectInstance[] efts)
    {
        string s = "";
        foreach (var v in efts)
        {
            s += v.EffectID + "," + v.EffectValue.ToString("0.00") + ":";
        }
        if (s.Length > 1)
            return s.Substring(0, s.Length - 1);
        return s;
    }

    public static EffectInstance[] GetList(string inpt)
    {
        List<EffectInstance> efts = new List<EffectInstance>();
        string[] parts = inpt.Split(':');
        foreach (var v in parts)
        {
            string[] p = v.Split(',');
            if (p.Length > 1)
            {
                int id = 0;
                float val = 0;
                int.TryParse(p[0], out id);
                float.TryParse(p[1], out val);
                efts.Add(new EffectInstance(id, val));
            }
        }
        return efts.ToArray();
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(EffectInstance))
        {
            EffectInstance o = (EffectInstance)obj;
            return o.EffectID == EffectID && o.EffectValue == EffectValue;
        }
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return EffectID.GetHashCode() * 17 * EffectValue.GetHashCode();
    }
}
