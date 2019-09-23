using UnityEngine;
using System.Collections;
using VRTK;
using System.Collections.Generic;
public class OutfitActivator : MonoBehaviour {

    public ItemType Type;
    public GameObject ItemOrb;
    ItemOrbDisplay disp; 

    List<ItemOrbDisplay> orbsOut;

    void Awake()
    {
        inside = new List<ItemOrbDisplay>();
        orbsOut = new List<ItemOrbDisplay>();
        disp = GetComponent<ItemOrbDisplay>();
    }

    public void Grab(object o, InteractableObjectEventArgs e)
    {
        ArmorOption item = Armory.GetEquipped(Type);
        if (disp != null)
            item = disp.item;
        if (item != null && ItemOrb != null)
        {
            e.interactingObject.GetComponent<VRTK_InteractUse>().ForceStopUsing();
            e.interactingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
            GameObject dOrb = (GameObject)Instantiate(ItemOrb, transform.position, Quaternion.identity);
            e.interactingObject.GetComponent<VRTK_InteractTouch>().ForceTouch(dOrb);
            e.interactingObject.GetComponent<VRTK_InteractGrab>().AttemptGrab();
            dOrb.GetComponent<ItemOrbDisplay>().Setup(item);
            orbsOut.Add(dOrb.GetComponent<ItemOrbDisplay>());
        }
    }

    void FixedUpdate()
    {
        foreach(var v in orbsOut)
        {
            if(!v.isActivated())
            {
                if(v != null)
                {
                    VRTK_InteractableObject io = v.gameObject.GetComponent<VRTK_InteractableObject>();
                    if (io != null && io.GetGrabbingObject() == null)
                    {
                        Rigidbody connected = v.gameObject.GetComponent<Rigidbody>();
                        if (connected != null)
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
            }
        }
    }

    void Update()
    {
        for(int i=inside.Count-1; i>= 0; i--)
        {
            ItemOrbDisplay disp = inside[0];
            if (disp == null)
                inside.RemoveAt(i);
            else
            {
                
                if (disp != null && orbsOut.Contains(disp))
                {
                    VRTK_InteractableObject io = disp.gameObject.GetComponent<VRTK_InteractableObject>();
                    if (io != null && io.GetGrabbingObject() == null)
                    {
                        orbsOut.Remove(disp);
                        if (disp.gameObject != null)
                            Destroy(disp.gameObject);
                    }
                }
            }
        }
    }

    List<ItemOrbDisplay> inside;
    void OnTriggerEnter(Collider col)
    {
        ItemOrbDisplay disp = col.GetComponent<ItemOrbDisplay>();
        if(disp != null)
            inside.Add(disp);
    }

    void OnTriggerExit(Collider col)
    {
        ItemOrbDisplay disp = col.GetComponent<ItemOrbDisplay>();
        if(disp != null)
            inside.Remove(disp);
    }
}
