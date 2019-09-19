using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PhotonView))]
public class TalentEffects : MonoBehaviour {

    PhotonView v;

    void Start()
    {
        v = GetComponent<PhotonView>();
    }

    public void TryUseEffect(int id)
    {
        if(PhotonNetwork.inRoom)
        {
            v.RPC("NetworkEffect", PhotonTargets.Others, id, true);
        }
        NetworkEffect(id, false);
    }

    void NetworkEffect(int id, bool onlyVisual)
    {
        switch (id)
        {
            case 0:
                Debug.Log("Talent 1");
                break;
            case 1:
                Debug.Log("Talent 2");
                break;
            default:
                Debug.Log("No talent with id " + id);
                break;
        }
    }
}
