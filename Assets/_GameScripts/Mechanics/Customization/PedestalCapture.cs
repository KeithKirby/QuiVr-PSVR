using UnityEngine;
using System.Collections;
using VRTK;
public class PedestalCapture : MonoBehaviour {

    public ParticleSystem CaptureParticles;
    Rigidbody connected;

    public ItemPedestal itmped;

	void OnTriggerStay(Collider col)
    {
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb == connected)
            return;
        ItemOrbDisplay orb = col.GetComponent<ItemOrbDisplay>();
        VRTK_InteractableObject io = col.GetComponent<VRTK_InteractableObject>();
        if (orb != null && io != null && io.GetGrabbingObject() == null)
        {
            if (connected != null && col.gameObject == connected.gameObject)
                return;
            ReleaseItem();
            CaptureParticles.Play();
            col.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
            orb.Activate();
            connected = rb;
            itmped.SetItem(orb.item);
            CaptureParticles.Play();
        }
    }

    public void Clear()
    {
        if(connected != null)
        {
            ItemOrbDisplay orb = connected.GetComponent<ItemOrbDisplay>();
            orb.Deactivate();
            Destroy(connected.gameObject);
            CaptureParticles.Stop();
        }
    }

    void OnTriggerExit(Collider col)
    {
        ItemOrbDisplay orb = col.GetComponent<ItemOrbDisplay>();
        VRTK_InteractableObject io = col.GetComponent<VRTK_InteractableObject>();
        if(col.GetComponent<Rigidbody>() == connected)
        {
            if(orb != null && io != null && io.GetGrabbingObject() != null)
            {
                orb.Deactivate();
                connected = null;
                ReleaseItem();
            }
        }
    }

    public void ReleaseItem()
    {
        if (connected != null)
            Destroy(connected.gameObject);
        itmped.Clear();
        CaptureParticles.Stop();
    }

    void FixedUpdate()
    {
        if(connected != null)
        {
            Vector3 dir = (transform.position - connected.transform.position).normalized;
            connected.AddForce(dir * Vector3.Distance(transform.position, connected.transform.position) * 150f * Time.fixedDeltaTime);
            if (connected.angularVelocity.magnitude < 1.5f)
                connected.AddTorque(transform.up * Time.fixedDeltaTime * 60f);
            else
                connected.angularVelocity *= 0.9f;
        }
    }
}
