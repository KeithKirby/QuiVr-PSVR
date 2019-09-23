using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;
using SimpleJSON;
#if !UNITY_PS4
//using Parse;
#endif

public class ItemGenerator : MonoBehaviour
{
    public ItemDropData DropData;
    public ItemDrop[] ValidDrops;

    static public ItemGenerator s_inst = null;

    void Awake()
    {
        if(null==s_inst)
        {
            s_inst = this;
            GetValidDrops();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    [AdvancedInspector.Inspect]
    public void TestGiveItem()
    {
        int r = ItemGenerator.GetRatity(GameBase.instance.Difficulty);
        var itemGen = GameObject.FindObjectOfType<ItemGenerator>();
        itemGen.GetRandomItem(r, ItemGenerator.Encrypt("" + r), (ao)=> { });
    }

    void GetValidDrops()
    {
        var validDrops = new List<ItemDrop>();
        foreach(var drop in DropData.Drops)
            if(ItemDatabase.IsValidDrop(drop))
                validDrops.Add(drop);
        ValidDrops = validDrops.ToArray();
    }

    public void GetRandomItem(int rarity, string validator, Action<ArmorOption> action, bool invalidResource=true)
    {
        if(""+rarity != Decrypt(validator))
        {
            action.Invoke(null);
            Debug.LogError("Invalid token to generate item");
            return;
        }
        Dictionary<string, object> ItemRarity = new Dictionary<string, object>
        {
            { "rarity", rarity }
        };

        // dw - need drop tables to do item drops
#if true || UNITY_PS4
        GetItemUsingDropTable(rarity, validator, action, invalidResource);
#else
        /*
        ParseCloud.CallFunctionAsync<string>("GetItem", ItemRarity).ContinueWith(t =>
        {
            if (!t.IsFaulted && !t.IsCanceled)
            {
                JSONNode jsObj = JSON.Parse(t.Result);
                UnityMainThreadDispatcher.Instance().Enqueue(() => action.Invoke(new ArmorOption(jsObj)));
            }
            else
            {
                foreach (var e in t.Exception.InnerExceptions)
                {
                    ParseException parseException = (ParseException)e;
                    Debug.Log("Unsuccessful Item Request: " + parseException.Message);
                }
                action.Invoke(null);
                if(invalidResource)
                    Armory.instance.GiveResource(150);
            }
        });
        */
#endif
    }
    
    static public ArmorOption CreateItem(ItemDrop drp)
    {
        int ancient = 0;
        if (ItemPedestal.CanReroll(drp.EffectID, drp.Rarity))
            ancient = ItemDatabase.RollAncient();
        string effects = ItemDatabase.RandomEffect(drp.EffectID, ancient);
        return new ArmorOption(drp.Name, drp.Type, drp.Meshes, drp.Colors, effects, drp.Rarity, ancient);
    }

    void GetItemUsingDropTable(int rarity, string validator, Action<ArmorOption> action, bool invalidResource = true)
    {
        //{ "Rarity":4,"Name":"Gloves of the Past","EffectID":7,"ItemType":3,"Meshes":"9","Colors":""}
        var matches = new List<ItemDrop>();
        var itemGen = GameObject.FindObjectOfType<ItemGenerator>();
        for(int i=0;i<itemGen.ValidDrops.Length;++i)
        {
            var drop = itemGen.ValidDrops[i];
            if(rarity==drop.Rarity)
                matches.Add(drop);
        }
        
        if(matches.Count>0)
        {
            matches.Shuffle();
            action(CreateItem(matches[0]));
        }
        else
        {
            action(null);
            if (invalidResource)
                Armory.instance.GiveResource(150);
        }
        
    }

    public static byte[] Shuffle(byte[] list)
    {
        System.Random rnd = new System.Random();
        for (int t = 0; t < list.Length; t++)
        {
            byte tmp = list[t];
            
            int r = rnd.Next(t, list.Length);
            list[t] = list[r];
            list[r] = tmp;
        }
        return list;
    }

    public static int GetResource(float difficulty)
    {
        return Mathf.Clamp((int)((difficulty*1.05f)/2f), 50, 2500);
    }

    public static bool ItemOnLoss(float difficulty)
    {
        return UnityEngine.Random.Range(0, Mathf.Max(1, 1000 - difficulty)) < 100f; //200-difficulty
    }

    public static string Encrypt(string text)
    {
        SymmetricAlgorithm algorithm = DES.Create();
        ICryptoTransform transform = algorithm.CreateEncryptor(Armory.instance.GetKey(), Armory.instance.GetIV());
        byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
        byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
        return Convert.ToBase64String(outputBuffer);
    }

    public static string Decrypt(string text)
    {
        SymmetricAlgorithm algorithm = DES.Create();
        ICryptoTransform transform = algorithm.CreateDecryptor(Armory.instance.GetKey(), Armory.instance.GetIV());
        byte[] inputbuffer = Convert.FromBase64String(text);
        byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
        Armory.instance.Reshuffle();
        return Encoding.Unicode.GetString(outputBuffer);
    }

    public static int GetRatity(float difficulty)
    {
        if(AppBase.v.isDemo)
        {
            if (difficulty > 500 && UnityEngine.Random.Range(0, 100) < 15f)
                return 2;
            return 1;
        }
        if(difficulty > 20)
        {
            float Cap = 1000f;
            float f = UnityEngine.Random.Range(0, Cap);//(Mathf.Clamp(Cap - difficulty, 5, Cap)));
            float perc = (f / Cap) *100;
            for(int i = ItemDatabase.v.RarityPercents.Length-1; i >= 0; i--)
            {
                if (perc <= ItemDatabase.v.RarityPercents[i])
                {
                    if(i < 4 || !AppBase.v.isDemo)
                        return i;
                }
            }
        }
        return 1;
    }
}
