using UnityEngine;
using System.Collections;
using VRTK;
using System;
using UnityEngine.Events;

public class NetworkPhysicsObject : MonoBehaviour {

    public float InterpRate = 9;

    PhotonView photonView;
    Vector3 pos;
    Quaternion rot;
    Vector3 vel;
    Vector3 aVel;
    bool isKinematic;

    Rigidbody rb;

    bool grabbed;

    VRTK_InteractableObject io;

    void Awake()
    {
        io = GetComponent<VRTK_InteractableObject>();
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    void Start()
    {
        pos = transform.position;
        rot = transform.rotation;
        if(io != null)
            io.InteractableObjectUngrabbed +=  delegate { StartCoroutine("MakeKinematicDelayed"); };
    }

    IEnumerator MakeKinematicDelayed()
    {
        yield return true;
        yield return true;
        rb.isKinematic = false;
    }

    bool changingOwner;

    void OnJoinedRoom()
    {
        if(io != null)
            GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(ChangeOwnerMe);
    }

    void Update()
    {
        if (PhotonNetwork.inRoom && !photonView.isMine && !changingOwner)
        {
            if (io != null && io.IsGrabbed())
                io.ForceStopInteracting();
            Vector3 wantpos = Vector3.Lerp(transform.position, pos, Time.deltaTime * InterpRate);
            if (!IsNAN(wantpos))
                transform.position = wantpos;
            Quaternion wantrot = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * InterpRate);
            if (!IsNAN(wantrot))
                transform.rotation = wantrot;
            if (!IsNAN(vel))
                rb.velocity = vel;
            if (!IsNAN(aVel))
                rb.angularVelocity = aVel;
            rb.isKinematic = isKinematic;
        }
        else if (photonView.isMine)
            changingOwner = false;
    }

    public static bool IsNAN(Vector3 v)
    {
        if (Single.IsNaN(v.x) || Single.IsNaN(v.y) || Single.IsNaN(v.z))
            return true;
        return false;
    }

    public static bool IsNAN(Quaternion q)
    {
        if (Single.IsNaN(q.x) || Single.IsNaN(q.y) || Single.IsNaN(q.z) || Single.IsNaN(q.w))
            return true;
        return false;
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
        if (PhotonNetwork.inRoom)
        {
            changingOwner = true;
            photonView.RequestOwnership();
        }        
    }
}
