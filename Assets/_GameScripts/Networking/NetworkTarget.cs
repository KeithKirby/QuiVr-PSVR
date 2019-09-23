using UnityEngine;
using System.Collections;

public class NetworkTarget : MonoBehaviour {

    public Health hscr;
    PhotonView v;
    float curHP = 100;

    // Use this for initialization
    void Start () {
        v = GetComponent<PhotonView>();
        curHP = hscr.maxHP;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(PhotonNetwork.inRoom && !v.isMine)
        {
            if(curHP != hscr.currentHP)
            {
                hscr.currentHP = curHP;
                hscr.updateHealthBar();
                if(curHP < 0)
                {
                    hscr.OnDeath.Invoke(gameObject);
                }
            }
        }
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(hscr.currentHP);
        }
        else
        {
            curHP = (float)stream.ReceiveNext();
        }
    }
}
