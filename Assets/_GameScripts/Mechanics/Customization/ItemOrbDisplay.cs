using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ItemOrbDisplay : MonoBehaviour
{
    public ArmorOption item;
    VRTK.VRTK_InteractableObject interact;
    public Transform[] DisplayHolders;
    public bool activated;
    public GameObject DestroyParticles;
    Collider[] internalCols;
    public ItemEvent OnSetup;

    GameObject inst;

    void Awake()
    {
        interact = GetComponent<VRTK.VRTK_InteractableObject>();
        internalCols = GetComponentsInChildren<Collider>();
    }

	public void Setup(ArmorOption o)
    {
        item = o;
        int type = (int)o.Type;
        if (type < DisplayHolders.Length)
            SetupDisplay(type);
        OnSetup.Invoke(o);
    }

    void SetupDisplay(int type)
    {
        if (inst != null)
            Destroy(inst);
        inst = (GameObject)Instantiate(ItemDatabase.GetDisplay(item.Type, item.ObjectID), DisplayHolders[type]);
        inst.transform.localScale = Vector3.one;
        inst.transform.localEulerAngles = Vector3.zero;
        inst.transform.localPosition = Vector3.zero;
        foreach(var t in DisplayHolders)
        {
            foreach (var v in t.GetComponentsInChildren<Collider>())
            {
                v.enabled = false;
            }
        }
        SetItemValues(inst);
        Collider c = GetComponent<Collider>();
        if(c != null)
            c.enabled = true;
    }

    void SetItemValues(GameObject o)
    {
        if (item.Type == ItemType.Arrow)
            o.GetComponent<ArrowPrefab>().Setup(item.Colors);
        else if (item.Type == ItemType.BowBase)
            o.GetComponent<BowPrefab>().Setup(item.Colors);
        else if (item.Type == ItemType.Helmet)
            o.GetComponent<Helmet>().Setup(item.Colors, false, null);
        else if (item.Type == ItemType.Gloves)
            o.GetComponent<GlovePrefab>().Setup(item.Colors, false, 0);
        else if (item.Type == ItemType.ChestArmor)
            o.GetComponent<Chestplate>().Setup(item.Colors);
        else if (item.Type == ItemType.Quiver)
            o.GetComponent<QuiverPrefab>().Setup(item.Colors);
    }

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }

    public bool isActivated()
    {
        return activated;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col != null && col.gameObject != null && col.gameObject.tag != "Player" && interact != null && !interact.IsGrabbed() && !activated)
            Destroy(gameObject);
    }

    bool isQuitting;
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (!isQuitting && DestroyParticles != null)
        {
#if !UNITY_EDITOR
            DestroyParts();
#endif
        }
    }

    public void DestroyParts()
    {
        Instantiate(DestroyParticles, transform.position, Quaternion.identity);
    }
}
