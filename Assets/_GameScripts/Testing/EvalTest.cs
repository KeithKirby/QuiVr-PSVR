using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Parse;
using UnityEngine.Networking;

public class EvalTest : MonoBehaviour {

    //public PvPMessageType msg;

    public ArmorOption itemOne;
    public ArmorOption itemTwo;

    [AdvancedInspector.Inspect]
    public void Run()
    {
        if (itemOne.Better(itemTwo).Equals(itemOne))
            Debug.Log("Item One is Better than Item Two");
        else
            Debug.Log("Item Two is Better than Item One");

        if (itemOne.Worse(itemTwo).Equals(itemOne))
            Debug.Log("Item One is Worse than Item Two");
        else
            Debug.Log("Item Two is Worse than Item One");
    }

    /*
    IEnumerator RunEnum()
    {
        string bodyData = "{\"type\":\"" + msg.ToString() + "\", \"teamid\": \"" + TeamID + "\", \"members\":\"" + GetString(Members) + "\",\"grpMMR\":" + grpMMR + ",\"version\":\"0.0.0\",\"matchType\":\"2\" }";
        UnityWebRequest request = new UnityWebRequest("http://34.198.149.161:3000/PvPQueue", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.Send();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }
    */

    string GetString(string[] array)
    {
        string s = "";
        foreach(var v in array)
        {
            s += v.Replace(',','.') + ",";
        }
        return s.Substring(0, s.Length-1);
    }

    public enum PvPMessageType
    {
        JOIN,
        CANCEL,
        CHECK,
        ACCEPT
    }
}


