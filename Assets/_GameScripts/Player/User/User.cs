using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Parse;
public class User : MonoBehaviour {

    public static bool ArcadeMode;
    static bool SavedUser;
    //public static ParseObject ArcadeUser;
    public static PlayerProfile ArcadeUser;

    public static void NewArcadeUser()
    {
        ArcadeMode = true;
        string name = RandomName();
        //ArcadeUser = new ParseObject("ArcadeUser");
        //ArcadeUser["Owner"] = ParseUser.CurrentUser;
        //ArcadeUser["Name"] = name;
        //ArcadeUser["Pin"] = "aaaa";
        SetArcadeUser(false);
    }

    public static void LeaveArcade()
    {
        if(ArcadeMode)
        {
            ArcadeMode = false;
            if (Armory.instance != null)
                Armory.instance.FetchFromServer();
            Statistics.SetUser();
            PlatformSetup.instance.ResetName();
            //if (SavedUser && ArcadeUser != null)
            //{
                //ArcadeUser.SaveAsync();
                //ArcadeUser = null;
            //}
            //else if (ArcadeUser != null)
            //{
                //DeleteUser(DC);
            //}
            SavedUser = false;
            Debug.Log("Profile Logout Successful");
        }
    }

    static void DC(bool res)
    { }

    public static void DeleteUser(System.Action<bool> callback, string pin="aaaa")
    {
        /*if(ArcadeUser != null)
        {
            if (ArcadeUser.Get<string>("Pin") == pin)
            {
                RemoveName(ArcadeUser.Get<string>("Name"));
                ArcadeUser.DeleteAsync();
                SavedUser = false;
                ArcadeUser = null;
                if(callback != null)
                    UnityMainThreadDispatcher.Instance().Enqueue(() => callback(true));
            }
            else if(callback != null)
                UnityMainThreadDispatcher.Instance().Enqueue(() => callback(false));
        }*/
    }

    public static void Login(string username, string pin, System.Action<bool> callback)
    {
        /*var query = ParseObject.GetQuery("ArcadeUser").WhereEqualTo("Name", username);
        query.FirstAsync().ContinueWith(t =>
        {
            if(!t.IsCanceled && !t.IsFaulted)
            {
                ParseObject o = t.Result;
                if (o != null)
                {
                    string p = o.Get<string>("Pin");
                    if (p == pin)
                    {
                        ArcadeMode = true;
                        ArcadeUser = o;
                        Debug.Log("Profile Login Successful: " + username);
                        UnityMainThreadDispatcher.Instance().Enqueue(() => SetArcadeUser(true));
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AddName(username));
                        if(callback != null)
                            UnityMainThreadDispatcher.Instance().Enqueue(() => callback(true));
                    }
                    else 
                    {
                        if (callback != null)
                            UnityMainThreadDispatcher.Instance().Enqueue(() => callback(false));
                        Debug.Log("Login Unsuccessful - Incorrect UserID or PIN");
                    }                       
                }
                else 
                {
                    Debug.Log("Login Unsuccessful - UserID not found");
                    if (callback != null)
                        UnityMainThreadDispatcher.Instance().Enqueue(() => callback(false));
                }                  
            }
            else 
            {
                Debug.Log("Login Unsuccessful - Server Error");
                if (callback != null)
                    UnityMainThreadDispatcher.Instance().Enqueue(() => callback(false));              
            }
        });
        */
    }

    static void SetArcadeUser(bool fromSave)
    {
        /*if(ArcadeUser != null)
        {
            SavedUser = fromSave;
            if (Armory.instance != null)
                Armory.instance.FetchFromServer();
            if (Cosmetics.instance != null)
                Cosmetics.FetchFromServer();
            Statistics.SetUser();
            UpdateName();
        }*/
    }

    public static void UpdateName()
    {
        /*
        string username = ArcadeUser.Get<string>("Name");
        PhotonNetwork.player.name = username;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("SteamID", username);
        PhotonNetwork.player.SetCustomProperties(props);
        PlatformSetup.instance.UserName = username;
        */
    }

    public static void SaveUser(string username, string pin, System.Action<string> callback)
    {
        /*
        if(ArcadeUser != null)
        {
            Debug.Log("Attempting Arcade User Save");
            var query = ParseObject.GetQuery("ArcadeUser").WhereEqualTo("Name", username);
            Debug.Log("Executing Query");
            query.FirstAsync().ContinueWith(t =>
            {
                if(t.IsFaulted || t.IsCanceled) //No hit
                {
                    ArcadeUser["Name"] = username;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateName());
                    ArcadeUser["Pin"] = pin;
                    ArcadeUser.SaveAsync();
                    UnityMainThreadDispatcher.Instance().Enqueue(() => AddName(username));
                    SavedUser = true;
                    if (callback != null)
                        UnityMainThreadDispatcher.Instance().Enqueue(() => callback(""));
                }
                else
                {
                    ParseObject o = t.Result;
                    if (o == null || o.ObjectId == ArcadeUser.ObjectId) //Just make sure no hit
                    {
                        ArcadeUser["Name"] = username;
                        if (username.Length < 1)
                            ArcadeUser["Name"] = "NoUsername_" + RandomName();
                        UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateName());
                        ArcadeUser["Pin"] = pin;
                        ArcadeUser.SaveAsync();
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AddName(username));
                        SavedUser = true;
                        if (callback != null)
                            UnityMainThreadDispatcher.Instance().Enqueue(() => callback(""));
                    }
                    else if (callback != null)
                        UnityMainThreadDispatcher.Instance().Enqueue(() => callback("Username Taken"));
                }
                
            });
        }
        else if (callback != null)
            UnityMainThreadDispatcher.Instance().Enqueue(() => callback("No user to save"));
            */
    }

    public static string RandomName()
    {
        string s = "";
        string[] Syllables = {"Mon","Fay","Sho","Zag","Bla","Rash","Zen","Zig","Cra","Rel","Gar","Ila","Zi","Mar","Nar","Pog","Log","Bog","Nog","Gla","Qua",
                              "Yoo","Har","Wob","Pik","Achu","Glum","Kek","Bam","Tog","Tag","Tum","Tab","Tad","Tuum","Tho","Chi","Ag","Oom","Lo","Il"};
        int snum = 2;
        for(int i=0; i<snum; i++)
        {
            string syl = Syllables[Random.Range(0, Syllables.Length)];
            if (i > 0)
                syl = syl.ToLower();
            s += syl;
        }
        return s;
    }

    public static void AddName(string name)
    {
        List<string> names = new List<string>();
        if(Settings.HasKey("ArcadeNames"))
        {
            string[] nms = Settings.GetStringArray("ArcadeNames");
            foreach(var v in nms)
            {
                if (v == name)
                    return;
                names.Add(v);
            }
        }
        names.Add(name);
        Settings.Set("ArcadeNames", string.Join(",",names.ToArray()));
    }

    public static void RemoveName(string name)
    {
        List<string> names = new List<string>();
        if (Settings.HasKey("ArcadeNames"))
        {
            string[] nms = Settings.GetStringArray("ArcadeNames");
            foreach (var v in nms)
            {
                names.Add(v);
            }
        }
        names.Remove(name);
        Settings.Set("ArcadeNames", string.Join(",",names.ToArray()));
    }

    public static bool hasSaved()
    {
        return SavedUser;
    }

    public static void TrySave()
    {
        //if (ArcadeUser != null && SavedUser && ArcadeUser.Get<string>("Pin") != "aaaa")
            //ArcadeUser.SaveAsync();
    }

    public static void CheckUsername(string username, System.Action<string> callback)
    {
        /*var query = ParseObject.GetQuery("ArcadeUser").WhereEqualTo("Name", username);
        query.FirstAsync().ContinueWith(t =>
        {
            if (!t.IsCanceled && !t.IsFaulted)
            {
                ParseObject o = t.Result;
                if (o != null)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => callback("Username already taken"));
                }
                else
                    UnityMainThreadDispatcher.Instance().Enqueue(() => callback(""));
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => callback(""));
            }
        });
        */
    }
}
