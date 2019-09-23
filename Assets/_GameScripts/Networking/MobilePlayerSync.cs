using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MobilePlayerSync : MonoBehaviour {
    public GameObject Dummy;
    public GameObject Real;

    public GameObject OrbPrefab;

    PhotonView view;
    Vector3 pos;
    Quaternion rot;

    public static MobilePlayerSync myInstance;

    void Awake()
    {
        view = GetComponent<PhotonView>();
        if(!PhotonNetwork.inRoom || view.isMine)
        {
            myInstance = this;
            Real.SetActive(true);
            Dummy.SetActive(false);
        }
    }

    IEnumerator Start()
    {
        if(this == myInstance)
        {
            yield return true;
            GameObject obj = GetComponentInChildren<EventSystem>().gameObject;
            obj.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            obj.SetActive(true);
        }
    }

    void Update()
    {
        if(!view.isMine && PhotonNetwork.inRoom)
        {
            Dummy.transform.position = Vector3.Lerp(Dummy.transform.position, pos, Time.unscaledDeltaTime * 9f);
            Dummy.transform.rotation = Quaternion.Lerp(Dummy.transform.rotation, rot, Time.unscaledDeltaTime * 9f);
        }
    }

    public void ThrowOrb(Vector3 pos, Vector3 vel)
    {
        ThrowOrbNetwork(pos, vel, true);
        if(PhotonNetwork.inRoom)
            view.RPC("ThrowOrbNetwork", PhotonTargets.Others, pos, vel, false);
    }

    [PunRPC]
    void ThrowOrbNetwork(Vector3 pos, Vector3 vel, bool mine)
    {
        GameObject o = Instantiate(OrbPrefab, pos, Quaternion.identity);
        OrbAbility oa = o.GetComponent<OrbAbility>();
        o.GetComponent<VRTK.VRTK_InteractableObject>().enabled = false;
        oa.Setup(2, 25, mine);
        oa.thrown = true;
        oa.DisableStartParticles();
        oa.GetComponent<Rigidbody>().AddRelativeForce(vel, ForceMode.Impulse); ;
    }

    public void UseAbility(int id, Vector3 pos, Vector3 lookDir, float value)
    {
        view.RPC("UseAbilityNetworkPos", PhotonTargets.Others, id, pos, lookDir, value);
    }

    [PunRPC]
    void UseAbilityNetworkPos(int id, Vector3 pos, Vector3 lookDir, float value)
    {
        AbilityManager.instance.UseAbility(id, pos, lookDir, true, value);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(Real.transform.position);
            stream.SendNext(Real.transform.rotation);
        }
        else
        {
            pos = (Vector3)stream.ReceiveNext();
            rot = (Quaternion)stream.ReceiveNext();
        }
    }
}
