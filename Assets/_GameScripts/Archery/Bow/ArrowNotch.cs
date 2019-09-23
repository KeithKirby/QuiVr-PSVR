using UnityEngine;
using System.Collections;
using VRTK;

public class ArrowNotch : MonoBehaviour {
    private GameObject arrow;
    private VRTK_InteractableObject obj;
    public float nockCooldown;
    bool cshoot;
    bool nocked;
    public GameObject ArrowHolder;
    public Transform ArrowObject;

    Quaternion grabRot;

    private void Start()
    {
        arrow = ArrowHolder.transform.Find("Arrow").gameObject;
        obj = ArrowHolder.GetComponent<VRTK_InteractableObject>();
        //Invoke("CanShoot", nockCooldown);
        cshoot = true;
        GetComponentInParent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(StopDespawn);
        GetComponentInParent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(StartDespawn);
        grabRot = ArrowHolder.transform.localRotation;
    }

    public void StartDespawn(object sender, InteractableObjectEventArgs e)
    {
        if(enabled)
            StartCoroutine("TriggerDespawnTimed");
    }

    public void StopDespawn(object sender, InteractableObjectEventArgs e)
    {
        StopCoroutine("TriggerDespawnTimed");
    }

    IEnumerator TriggerDespawnTimed()
    {
        yield return new WaitForSeconds(5f);
        if (!PhotonNetwork.inRoom)
            Destroy(ArrowHolder);
        else if(ArrowHolder.GetComponent<PhotonView>() != null && ArrowHolder.GetComponent<PhotonView>().isMine)
        {
            PhotonNetwork.Destroy(ArrowHolder);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(cshoot)
        {
            var handle = collider.GetComponentInParent<BowHandle>();

            if (handle != null && obj != null && handle.aim.IsHeld() && obj.IsGrabbed() && handle.arrowNockingPoint != null)
            {
                if (handle.arrowNockingPoint.childCount > 0)
                {
                    Transform g = handle.arrowNockingPoint.GetChild(0);
                    Destroy(g.gameObject);
                }
                nocked = true;
                arrow.transform.parent = handle.arrowNockingPoint;
                CopyNotchToArrow();
                collider.GetComponentInParent<BowAim>().SetArrow(arrow);
                if (PhotonNetwork.inRoom && ArrowHolder.GetComponent<PhotonView>() != null && ArrowHolder.GetComponent<PhotonView>().isMine)
                    PhotonNetwork.Destroy(ArrowHolder);
                else
                    Destroy(ArrowHolder);
            }
        }   
    }

    void FixedUpdate()
    {
        if(!nocked && arrow!= null && arrow.transform.parent == ArrowHolder.transform)
        {
            /*
            if(cshoot && BowHandle.instance != null && obj.IsGrabbed())
            {
                Quaternion bowRot = BowHandle.instance.arrowNockingPoint.rotation;
                bowRot *= Quaternion.Euler(-90, 0, 0);
                Quaternion localArrow = grabRot * ArrowHolder.transform.parent.rotation;
                float dist = Vector3.Distance(ArrowHolder.transform.position, BowHandle.instance.arrowNockingPoint.position)*4f;
                if (dist < 1)
                    ArrowHolder.transform.rotation = Quaternion.Lerp(bowRot, localArrow, dist);
                else
                    ArrowHolder.transform.localRotation = grabRot;
            }
            */
            arrow.transform.localPosition = Vector3.zero;
            arrow.transform.localEulerAngles = Vector3.zero;
        }
    }

    void CanShoot()
    {
        cshoot = true;
    }

    private void CopyNotchToArrow()
    {
        GameObject notchCopy = Instantiate(this.gameObject, this.transform.position, this.transform.rotation) as GameObject;
        notchCopy.name = this.name;
		arrow.GetComponent<Arrow> ().enabled = true;
		arrow.GetComponent<ArrowPhysics> ().enabled = true;
        arrow.GetComponent<Arrow>().SetArrowHolder(notchCopy);
        arrow.GetComponent<Arrow>().OnNock();
    }
}
