using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.Events;
public class OrbAbility : MonoBehaviour {

    int EffectID;
    float EffectValue;
    bool owned;
    [HideInInspector]
    public bool thrown;
    GameObject disp;
    public GameObject StartParticles;

    public UnityEvent OnThrow;
    VRTK_InteractableObject interact;

    void Awake()
    {
        interact = GetComponent<VRTK_InteractableObject>();
        interact.InteractableObjectUngrabbed += Throw;
        if(Settings.GetBool("QuickOrb"))
        {
            interact.holdButtonToGrab = true;
            interact.grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.Touchpad_Press;
        }
    }

    public void Setup(int id, float val, bool owner=false)
    {
        Destroy(gameObject, 30);
        owned = owner;
        EffectID = id;
        EffectValue = val;
        if (disp != null)
            Destroy(disp);
        disp = (GameObject) GameObject.Instantiate(ItemDatabase.GetEffect(EffectID).Display.OrbEffect, transform);
        disp.transform.localPosition = Vector3.zero;
        disp.transform.localScale = Vector3.one;
        disp.transform.localEulerAngles = Vector3.zero;
        if (!owner)
        {
            thrown = true;
            Destroy(gameObject, 20);
        }        
    }

    public void Throw(object sender, InteractableObjectEventArgs e)
    {
        Thrown();
    }

    public void DisableStartParticles()
    {
        if(StartParticles != null)
            StartParticles.SetActive(false);
    }

    public void Thrown()
    {
        thrown = true;
        OnThrow.Invoke();
        GetComponent<VRTK_InteractableObject>().enabled = false;
        OrbManager.instance.SetThrown();
        Destroy(gameObject, 20);
    }

    bool activated;
    void OnCollisionEnter(Collision col)
    {
        if(thrown && col.gameObject.tag != "Player" && !activated)
        {
            activated = true;
            if (owned && AbilityManager.instance != null)
                AbilityManager.instance.UseAbility(EffectID, col.contacts[0].point, false, EffectValue);
            Destroy(gameObject);
        }
    }

    public bool wasThrown()
    {
        return thrown;
    }
}
