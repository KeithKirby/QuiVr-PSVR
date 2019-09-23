using UnityEngine;
using System.Collections;

public class ItemChanger : MonoBehaviour {

    public GameObject Helmet;
    public GameObject LeftGlove;
    public GameObject RightGlove;
    public GameObject Chestplate;
    public GameObject Quiver;
    public GameObject Bow;
    public GameObject Arrow;
    public GameObject BowDetail;

    public SkinnedMeshRenderer Hood;

    private GameObject curHelmet;
    private GameObject curLeftGlove;
    private GameObject curRightGlove;
    private GameObject curChestplate;
    private GameObject curQuiver;
    private GameObject curBow;
    private GameObject curArrow;
    private GameObject curDetail;

    void Awake()
    {
        if (Helmet != null)
            curHelmet = Helmet.transform.GetChild(0).gameObject;
        if (Chestplate != null)
            curChestplate = Chestplate.transform.GetChild(0).gameObject;
        if(Quiver != null)
            curQuiver = Quiver.transform.GetChild(0).gameObject;
        if(Bow != null)
            curBow = Bow.transform.GetChild(0).gameObject;
        if (Arrow != null)
            curArrow = Arrow.transform.GetChild(0).gameObject;
        if (LeftGlove != null)
            curLeftGlove = LeftGlove.transform.GetChild(0).gameObject;
        if (RightGlove != null)
            curRightGlove = RightGlove.transform.GetChild(0).gameObject;
    }

    public void ChangeHelmet(ArmorOption o, bool shadowsOnly)
    {
        if (Helmet == null)
            return;
        if (curHelmet != null)
            Destroy(curHelmet);
        int id = Mathf.Clamp(o.ObjectID, 0, ItemDatabase.v.Helmets.Length - 1);
        curHelmet = (GameObject)Instantiate(ItemDatabase.v.Helmets[id], Helmet.transform);
        curHelmet.transform.localPosition = Vector3.zero;
        curHelmet.transform.localEulerAngles = Vector3.zero;
        curHelmet.transform.localScale = Vector3.one;
        curHelmet.GetComponent<Helmet>().Setup(o.Colors, !shadowsOnly, Hood);
        if (shadowsOnly)
            curHelmet.GetComponent<Helmet>().ShadowsOnly();
    }

    public void ChangeChest(ArmorOption o, bool shadowsOnly)
    {
        if (Chestplate == null)
            return;
        if (curChestplate != null)
            Destroy(curChestplate);
        int id = Mathf.Clamp(o.ObjectID, 0, ItemDatabase.v.Chestplates.Length - 1);
        curChestplate = (GameObject)Instantiate(ItemDatabase.v.Chestplates[id], Chestplate.transform);
        curChestplate.transform.localPosition = Vector3.zero;
        curChestplate.transform.localScale = Vector3.one;
        curChestplate.transform.localEulerAngles = Vector3.zero;
        curChestplate.GetComponent<Chestplate>().Setup(o.Colors);
        if (shadowsOnly)
            curChestplate.GetComponent<Chestplate>().ShadowsOnly();
    }

    public void ChangeQuiver(ArmorOption o)
    {
        if (Quiver == null)
            return;
        if (curQuiver != null)
            Destroy(curQuiver);
        int id = Mathf.Clamp(o.ObjectID, 0, ItemDatabase.v.Quivers.Length - 1);
        curQuiver = (GameObject)Instantiate(ItemDatabase.v.Quivers[id], Quiver.transform);
        curQuiver.transform.localPosition = Vector3.zero;
        curQuiver.transform.localScale = Vector3.one;
        curQuiver.transform.localEulerAngles = Vector3.zero;
        curQuiver.GetComponent<QuiverPrefab>().Setup(o.Colors);
    }

    public void ChangeBow(ArmorOption o)
    {
        if (Bow == null)
            return;
        if (curBow != null)
            Destroy(curBow);
        int id = Mathf.Clamp(o.ObjectID, 0, ItemDatabase.v.Bows.Length - 1);
        curBow = (GameObject)Instantiate(ItemDatabase.v.Bows[id], Bow.transform);
        curBow.transform.localPosition = Vector3.zero;
        curBow.transform.localScale = Vector3.one;
        curBow.transform.localEulerAngles = Vector3.zero;
        curBow.GetComponent<BowPrefab>().Setup(o.Colors);
    }

    public void ChangeArrow(ArmorOption o)
    {
        if (Arrow == null)
            return;
        if (curArrow != null)
            Destroy(curArrow);
        int id = Mathf.Clamp(o.ObjectID, 0, ItemDatabase.v.Arrows.Length - 1);
        curArrow = (GameObject)Instantiate(ItemDatabase.v.Arrows[id], Arrow.transform);
        curArrow.transform.localPosition = Vector3.zero;
        curArrow.transform.localScale = Vector3.one;
        curArrow.transform.localEulerAngles = Vector3.zero;
        curArrow.GetComponent<ArrowPrefab>().Setup(o.Colors);
    }

    public void ChangeGloves(ArmorOption o, bool dummy)
    {
        ChangeLeftGlove(o, dummy);
        ChangeRightGlove(o, dummy);
    }

    void ChangeLeftGlove(ArmorOption o, bool dummy)
    {
        if (LeftGlove == null)
            return;
        if (curLeftGlove != null)
            Destroy(curLeftGlove);
        int id = Mathf.Clamp(o.ObjectID, 0, ItemDatabase.v.Gloves.Length - 1);
        curLeftGlove = (GameObject)Instantiate(ItemDatabase.v.Gloves[id], LeftGlove.transform);
        curLeftGlove.transform.localPosition = Vector3.zero;
        curLeftGlove.transform.localScale = Vector3.one;
        curLeftGlove.transform.localEulerAngles = Vector3.zero;
        curLeftGlove.GetComponent<GlovePrefab>().Setup(o.Colors, dummy, dummy ? (LayerMask)0 : ItemDatabase.EquippedGlovesMaskStatic);
    }

    void ChangeRightGlove(ArmorOption o, bool dummy)
    {
        if (RightGlove == null)
            return;
        if (curRightGlove != null)
            Destroy(curRightGlove);
        int id = Mathf.Clamp(o.ObjectID, 0, ItemDatabase.v.Gloves.Length - 1);
        curRightGlove = (GameObject)Instantiate(ItemDatabase.v.Gloves[id], RightGlove.transform);
        curRightGlove.transform.localPosition = Vector3.zero;
        curRightGlove.transform.localScale = Vector3.one;
        curRightGlove.transform.localEulerAngles = Vector3.zero;
        curRightGlove.GetComponent<GlovePrefab>().Setup(o.Colors, dummy, dummy ? (LayerMask)0 : ItemDatabase.EquippedGlovesMaskStatic);
    }

    public void ChangeDetail(ArmorOption o)
    {
        if (BowDetail == null)
            return;
        if (curDetail != null)
            Destroy(curDetail);
        int id = Mathf.Clamp(o.ObjectID, 0, ItemDatabase.v.BowDetails.Length - 1);
        curDetail = (GameObject)Instantiate(ItemDatabase.v.BowDetails[id], BowDetail.transform);
        curDetail.transform.localPosition = Vector3.zero;
        curDetail.transform.localScale = Vector3.one;
        curDetail.transform.localEulerAngles = Vector3.zero;
        curDetail.GetComponent<ArrowPrefab>().Setup(o.Colors);
    }
}
