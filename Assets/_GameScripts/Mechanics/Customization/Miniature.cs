using UnityEngine;
using System.Collections;
using VRTK;
public class Miniature : MonoBehaviour {

    public Transform leftHand;
    public Transform rightHand;
    public Transform head;

    public bool OtherPlayer;
    public int playerID;

    Mannequin m;
    void Awake()
    {
        m = GetComponent<Mannequin>();
    }

	void Update()
    {
        if(PlayerHead.instance != null && !OtherPlayer)
        {
            Transform h = PlayerHead.instance.transform;
            head.localPosition = new Vector3(0, h.localPosition.y, 0);
            head.localRotation = h.localRotation;
            GameObject left = VRTK_DeviceFinder.GetControllerLeftHand();
            GameObject right = VRTK_DeviceFinder.GetControllerRightHand();
            if(left != null && Vector3.Distance(h.position, left.transform.position) < 10)
            {
                Vector3 offset = left.transform.localPosition - h.localPosition;
                leftHand.localPosition = head.localPosition + offset;
                leftHand.localRotation = left.transform.localRotation;
            }
            if(right != null && Vector3.Distance(h.position, right.transform.position) < 10)
            {
                Vector3 offset = right.transform.localPosition - h.localPosition;
                rightHand.localPosition = head.localPosition + offset;
                rightHand.localRotation = right.transform.localRotation;
            }
        }
        else if(PhotonNetwork.inRoom && OtherPlayer && PhotonNetwork.otherPlayers.Length > playerID)
        {
            CheckArmor();
            if(dummyHead != null)
            {
                head.localPosition = new Vector3(0, dummyHead.localPosition.y, 0);
                head.localRotation = dummyHead.localRotation;

                Vector3 offset = dummyLeft.transform.localPosition - dummyHead.localPosition;
                leftHand.localPosition = head.localPosition + offset;
                leftHand.localRotation = dummyLeft.transform.localRotation;

                offset = dummyRight.transform.localPosition - dummyHead.localPosition;
                rightHand.localPosition = head.localPosition + offset;
                rightHand.localRotation = dummyRight.transform.localRotation;
            }
        }
    }

    PlayerSync p;
    Transform dummyLeft;
    Transform dummyRight;
    Transform dummyHead;
    DummyArmor da;
    void CheckArmor()
    {
        if(p != null && PlayerSync.Others.Count > playerID)
        {
            p = PlayerSync.Others[playerID];
            da = p.GetComponentInChildren<DummyArmor>();
            foreach (var v in da.GetOutfit())
            {
                m.EquipItem(v);
            }
        }
        if(p != null)
        {
            dummyLeft = p.DummyHandLeft;
            dummyRight = p.DummyHandRight;
            dummyHead = p.DummyHead;
        }
    }
}
