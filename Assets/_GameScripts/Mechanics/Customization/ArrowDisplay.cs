using UnityEngine;
using System.Collections;

public class ArrowDisplay : MonoBehaviour {

    public GameObject ArrowHolder;
    GameObject currentArrow;

    public void Change()
    {
        if(Armory.currentOutfit.Arrow != null)
        {
            ArmorOption o = Armory.currentOutfit.Arrow;
            if (PhotonNetwork.inRoom && GetComponentInParent<NetworkArrowHolder>() != null)
                GetComponentInParent<NetworkArrowHolder>().ChangeDisplay(o.ToString());
            ChangeDisplay(o);
        }
    }

	public void ChangeDisplay(ArmorOption o)
    {
        if (currentArrow == null)
            currentArrow = ArrowHolder.transform.GetChild(0).gameObject;
        if (currentArrow != null)
            Destroy(currentArrow);
        int id = Mathf.Clamp(o.ObjectID, 0, ItemDatabase.v.Arrows.Length - 1);
        currentArrow = (GameObject)Instantiate(ItemDatabase.v.Arrows[id], ArrowHolder.transform);
        currentArrow.transform.localPosition = Vector3.zero;
        currentArrow.transform.localEulerAngles = Vector3.zero;
        currentArrow.GetComponent<ArrowPrefab>().Setup(o.Colors);
        if (GetComponentInParent<ArrowEffects>() != null)
            GetComponentInParent<ArrowEffects>().CurrentArrow = currentArrow.GetComponent<ArrowPrefab>();
    }
}
