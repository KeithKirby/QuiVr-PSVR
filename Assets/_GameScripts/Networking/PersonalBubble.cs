using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.UI;
public class PersonalBubble : MonoBehaviour {

    public GameObject[] NoArrows;
    public Toggle pbToggle;

    PlayerSync ps;

    void Awake()
    {
        ps = GetComponentInParent<PlayerSync>();
    }

	public void ToggleValue(bool val)
    {
        if(val)
        {
            activated = true;
            foreach(var v in NoArrows)
            {
                v.GetComponent<ArrowImpact>().DestroyArrow = true;
            }
            if (GetComponent<PBubbleEffect>() != null)
            {
                GetComponent<PBubbleEffect>().PulseBubble();
            }
        }
        else
        {
            activated = false;
            foreach (var v in NoArrows)
            {
                v.GetComponent<ArrowImpact>().DestroyArrow = false;
            }
        }
        if(ps != null && PhotonNetwork.inRoom)
            ps.PersonalBubble(val);
    }

    public bool activated;
    VRTK_ControllerEvents lh;
    VRTK_ControllerEvents rh;
    Vector3 lhm;
    Vector3 rhm;
    void Update()
    {
        if (BowAim.instance.GetArrow() != null)
            return;
        CheckHands();
        if (lh == null)
            return;
        Vector3 lvec = mVec(lh.transform.position, lhm);
        Vector3 rvec = mVec(rh.transform.position, rhm);
        bool dir = Vector3.Distance(lhm, rhm) < Vector3.Distance(lh.transform.position, rh.transform.position);
        bool mag = (Vector3.Magnitude(mVec(lhm, lh.transform.position))> 0.03f && Vector3.Magnitude(mVec(rhm, rh.transform.position)) > 0.03f);
        if(XApart(lvec, rvec) && mag && dir && !activated && lh.triggerPressed && rh.triggerPressed)
        {
            Debug.Log("Toggling Value");
            pbToggle.isOn = true;
            pbToggle.onValueChanged.Invoke(true);
        }
        lhm = lh.transform.position;
        rhm = rh.transform.position;
    }

    void CheckHands()
    {
        if (lh == null || !lh.gameObject.activeInHierarchy)
        {
            var hand = VRTK_DeviceFinder.GetControllerLeftHand();
            if (hand != null)
            {
                lh = hand.GetComponent<VRTK_ControllerEvents>();
                lhm = lh.transform.position;
            }
            else
                return;
        }
        if (rh == null || !rh.gameObject.activeInHierarchy)
        {
            rh = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();
            rhm = rh.transform.position;
        }
    }

    bool XApart(Vector3 v1, Vector3 v2)
    {
        return Vector3.Dot(v1.normalized, v2.normalized) < -0.75f;
    }

    Vector3 mVec(Vector3 v1, Vector3 v2)
    {
        return (v1-v2);
    }
}
