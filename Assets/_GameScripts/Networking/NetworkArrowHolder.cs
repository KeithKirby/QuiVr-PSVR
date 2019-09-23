using UnityEngine;
using System.Collections;
using VRTK;

public class NetworkArrowHolder : MonoBehaviour {

    public float InterpRate = 9;

    PhotonView photonView;
    Vector3 pos;
    Quaternion rot;
    Vector3 vel;
    Vector3 aVel;
    bool isKinematic;

    VRTK_InteractableObject itr;

    bool changingOwner;

    Rigidbody rb;

    void Awake()
    {
        itr = GetComponent<VRTK_InteractableObject>();
        photonView = GetComponent<PhotonView>();
        if (!PhotonNetwork.inRoom)
        {
            Destroy(photonView);
        }
        rb = GetComponent<Rigidbody>();
        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(ChangeOwnerMe);
    }

    void Update()
    {
        if(itr.IsGrabbed() && KnucklesHandControl.UsingFingers)
        {
            GameObject hand = itr.GetGrabbingObject();
            KnucklesHandControl pos = hand.GetComponent<KnucklesHandControl>();
            if (pos != null)
            {
                KnucklesHandPose pose = pos.handPose;
                if (!pos.touchingTrigger() || pose.middle_curl > 0.35f)
                {
                    itr.ForceStopInteracting();
                }
            }
        }
        if (PhotonNetwork.inRoom && !photonView.isMine && !changingOwner)
        {
            if(!NetworkPhysicsObject.IsNAN(pos) && !NetworkPhysicsObject.IsNAN(rot))
            {
                Vector3 wantPos = Vector3.Lerp(transform.position, pos, Time.deltaTime * InterpRate);
                Quaternion wantRot = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * InterpRate);
                if (!NetworkPhysicsObject.IsNAN(wantPos))
                    transform.position = wantPos;
                if (!NetworkPhysicsObject.IsNAN(wantRot))
                    transform.rotation = wantRot;
                if (!NetworkPhysicsObject.IsNAN(vel))
                    rb.velocity = vel;
                if (!NetworkPhysicsObject.IsNAN(aVel))
                    rb.angularVelocity = aVel;
                rb.isKinematic = isKinematic;
            }          
        }
        else if (photonView.isMine)
            changingOwner = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.isKinematic);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else
        {
            pos = (Vector3)stream.ReceiveNext();
            rot = (Quaternion)stream.ReceiveNext();
            isKinematic = (bool)stream.ReceiveNext();
            vel = (Vector3)stream.ReceiveNext();
            aVel = (Vector3)stream.ReceiveNext();
        }
    }

    public void ChangeOwnerMe(object sender, InteractableObjectEventArgs e)
    {
        if(PhotonNetwork.inRoom && photonView != null && photonView.owner.ID != PhotonNetwork.player.ID)
        {
            photonView.RequestOwnership();
            changingOwner = true;
        }     
    }

    public void ChangeDisplay(string val)
    {
        if(PhotonNetwork.inRoom && GetComponent<PhotonView>() != null)
            GetComponent<PhotonView>().RPC("ChangeNetwork", PhotonTargets.Others, val);
    }

    [PunRPC]
    void ChangeNetwork(string opt)
    {
        if (GetComponentInChildren<ArrowDisplay>() != null)
            GetComponentInChildren<ArrowDisplay>().ChangeDisplay(new ArmorOption(opt));
    }

    /*
    public void SetupEffects(EffectInstance[] effects)
    {
        string s = EffectInstance.StringList(effects);
        GetComponent<PhotonView>().RPC("EffectsNetwork", PhotonTargets.Others, s);
    }

    [PunRPC]
    void EffectsNetwork(string efts)
    {
        EffectInstance[] e = EffectInstance.GetList(efts);
        ArrowEffects ei = GetComponentInChildren<ArrowEffects>();
        if(ei != null)
        {
            ei.SetEffects(e, false);
        }
    }
    */

    public void AddEffect(int id, float val)
    {
        if (PhotonNetwork.inRoom)
            GetComponent<PhotonView>().RPC("AddEffectNetwork", PhotonTargets.Others, id, val);
    }

    [PunRPC]
    void AddEffectNetwork(int id, float val)
    {
        if(GetComponentInChildren<ArrowEffects>() != null)
        {
            GetComponentInChildren<ArrowEffects>().AddEffect(id, val, false, false);
        }
    }
}
