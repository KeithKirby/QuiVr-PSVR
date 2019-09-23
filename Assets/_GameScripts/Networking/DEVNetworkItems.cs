using UnityEngine;
using System.Collections;

public class DEVNetworkItems : MonoBehaviour {

    public string PlayerName;
    public bool Temporary;
    public ArmorOption DevItem;

    [AdvancedInspector.Inspect]
    public void GiveItem()
    {
        if (PhotonNetwork.inRoom)
        {
            foreach (var v in PhotonNetwork.playerList)
            {
                if (v.NickName == PlayerName)
                {
                    GetComponent<PhotonView>().RPC("SendItem", v, DevItem.ToString(), Temporary);
                    Debug.Log("Sent new item to " + v.NickName);
                    return;
                }
            }
        }
        else
        {
            if (Armory.instance != null && !Armory.instance.HasDuplicate(DevItem))
                Armory.instance.AddItem(DevItem, !Temporary);
        }

    }

    [PunRPC]
    void SendItem(string item, bool temp)
    {
        if (Armory.instance != null)
        {
            Armory.instance.AddItem(new ArmorOption(item), !temp);
        }
    }
}
