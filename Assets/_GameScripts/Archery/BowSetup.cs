using UnityEngine;
using System.Collections;
using VRTK;
using Valve.VR;

public class BowSetup : MonoBehaviour {

    public GameObject MessageObject;
    public VRTK_ControllerEvents Left;
    public VRTK_ControllerEvents Right;

    public GameObject bow;
    public GameObject quiver;

    public GameObject[] quiverMounts;

    public static BowSetup instance;

    bool initSetup;

    BowAim ba;

    // Use this for initialization
    IEnumerator Start () {
        instance = this;
        ba = bow.GetComponent<BowAim>();
        yield return true;
        if (!Settings.HasKey("LeftHanded"))
        {
            yield return new WaitForSeconds(0.5f);
            MessageObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            Left.TriggerPressed += SelectHand;
            Right.TriggerPressed += SelectHand;
        }
        else
        {
            while (!Right.gameObject.activeSelf || !Left.gameObject.activeSelf)
            {
                yield return true;
            }
            yield return true;
            yield return true;
            if (Settings.GetBool("LeftHanded"))
            {
                Setup(Right.gameObject, true);
            }  
            else
            {
                Setup(Left.gameObject, false);
            }
            initSetup = true;
        }
        InvokeRepeating("CheckBow", 1f, 1f);
    }

    void CheckBow()
    {
        if (!ba.IsHeld())
            RedoSetup();
    }

    void SelectHand(object o, ControllerInteractionEventArgs e)
    {
        initSetup = true;
        if (((VRTK_ControllerEvents)o).gameObject == Left.gameObject)
        {
            Setup(Right.gameObject, true);
            Settings.Set("LeftHanded", true);
        }
        else
        {
            Setup(Left.gameObject, false);
            Settings.Set("LeftHanded", false);
        }
        Left.TriggerPressed -= SelectHand;
        Right.TriggerPressed -= SelectHand;
    }

    void Setup(GameObject bowHand, bool left)
    {
        //Debug.Log("Grabbing Bow and Quiver");
        bow.GetComponent<Rigidbody>().isKinematic = false;
        quiver.GetComponent<Rigidbody>().isKinematic = false;
        MessageObject.SetActive(false);
        bowHand.GetComponent<VRTK_InteractTouch>().ForceTouch(bow);
        bowHand.GetComponent<VRTK_InteractGrab>().AttemptGrab();
        quiver.GetComponent<Quiver>().AllowHand(left);
        if (left)
        {
            if(!Settings.GetBool("HipMount"))
                quiver.GetComponent<Quiver>().Mount(quiverMounts[0]); //Shoulder Left
            else
                quiver.GetComponent<Quiver>().Mount(quiverMounts[1]); //Hip Left
        }    
        else
        {
            if (!Settings.GetBool("HipMount"))
                quiver.GetComponent<Quiver>().Mount(quiverMounts[2]); //Shoulder Right
            else
                quiver.GetComponent<Quiver>().Mount(quiverMounts[3]); //Hip Right
        }
    }

    public void Reset()
    {
        if (!initSetup)
            return;
        RedoSetup();
    }

    public void RedoSetup()
    {
        if (Settings.GetBool("LeftHanded"))
        {
            Setup(Right.gameObject, true);
        }
        else
        {
            Setup(Left.gameObject, false);
        }
    }
}
