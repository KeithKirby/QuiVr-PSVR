using UnityEngine;
using System.Collections;

public class PlayerEffectsSync : MonoBehaviour {

    PhotonView v;
    public GameObject missExplode;

    void Start()
    {
        v = GetComponent<PhotonView>();
    }

    public void MissExplosion(Vector3 pos)
    {
        if (PhotonNetwork.inRoom)
            v.RPC("missExpNetw", PhotonTargets.Others, pos);
        missExpNetw(pos);
    }

    [PunRPC]
    void missExpNetw(Vector3 pos)
    {
        GameObject newExpl = (GameObject)Instantiate(missExplode, pos, Quaternion.identity);
        Destroy(newExpl, 3f);
    }

}
