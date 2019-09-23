using UnityEngine;
using System.Collections;

public class GuardtowerItems : MonoBehaviour {

    public void SetPosition(Teleporter t)
    {
        int id = 0;
        if (PhotonNetwork.inRoom)
        {
            for (int i = 0; i < t.Positions.Length; i++)
            {
                if (t.Positions[i].userID == PhotonNetwork.player.ID)
                {
                    id = i;
                }
            }
        }
        Transform obj = t.Positions[id].pos;
        transform.position = obj.position;
        transform.rotation = obj.rotation;
    }
}
