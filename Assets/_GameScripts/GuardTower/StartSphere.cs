using UnityEngine;
using System.Collections;

public class StartSphere : MonoBehaviour {

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
        transform.position = obj.position + (Vector3.up * 1.25f);
        transform.rotation = obj.rotation;
        transform.position += transform.TransformDirection(Vector3.forward) * 0.8f;
        transform.Rotate(Vector3.up * 180);
    }
}
