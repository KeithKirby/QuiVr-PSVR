using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NVR_Positions : MonoBehaviour {

    public Transform HeadHolder;
    public Transform Quiver;
    public Transform QuiverAnchor;
    public Transform BowAnchor;
    public Transform BowHolder;
    public Transform HandRight;
    public Transform HandLeft;
    public Transform BowHandPos;
    public GameObject DummyArrow;
    public GameObject Bow;

    public GameObject ThirdPersonCam;
    public GameObject Ears;
    public GameObject TrackedObj;
    public GameObject Eyes;

    public Transform LHAnchor;
    public Transform RHAnchor;
    public Transform RHArrowAnchor;
    public Transform RHOrbAnchor;
    public Transform HeadAnchor;

    public Transform AimPoint;
    public Transform RotationHolder;

    public float TurnSpeed = 45f;
    public float VerticalSpeed = 2.5f;
    public float ScrollSpeed = 2.5f;
    Vector2 MAxis;

    bool FirstPerson;
    [HideInInspector]
    public bool forceFirstPerson;

    NVR_Player plr;

    void Awake()
    {
        plr = GetComponent<NVR_Player>();
    }

    public void ActivateNVR()
    {
        Destroy(Eyes.GetComponent<Camera>());
        Destroy(TrackedObj.GetComponent<GUILayer>());
        Destroy(TrackedObj.GetComponent<Camera>());
        Destroy(TrackedObj.GetComponentInChildren<SteamVR_TrackedObject>());
        Destroy(Ears.GetComponentInChildren<SteamVR_Ears>());
        Destroy(Ears.GetComponentInChildren<AudioListener>());
        HandLeft.transform.SetParent(HandLeft.transform.parent.parent);
        HandRight.transform.SetParent(HandRight.transform.parent.parent);
        HandRight.gameObject.SetActive(true);
        HandLeft.gameObject.SetActive(true);
        HandLeft.transform.position = LHAnchor.transform.position;
        HandRight.transform.position = RHAnchor.transform.position;
        HandLeft.transform.rotation = LHAnchor.transform.rotation;
        HandRight.transform.rotation = RHAnchor.transform.rotation;
        HeadHolder.transform.SetParent(HeadHolder.parent.parent);
        Bow.transform.SetParent(BowHolder);
        Bow.transform.localPosition = new Vector3(-0.002f, 0.0039f, -0.06792841f);
        Bow.transform.localEulerAngles = new Vector3(56.214f, 0, 180f);
        if (NVR_Player.usingFove)
            BowHandPos.localPosition = new Vector3(-0.192f, 0.254f, 0.685f);
    }

    HandAnim han;
    public void SetPositions()
    {
        HandLeft.transform.position = LHAnchor.position;
        HandLeft.transform.rotation = LHAnchor.rotation;
        if (han == null)
            han = HandLeft.GetComponentInChildren<HandAnim>();
        if (han != null)
        {
            han.empty = false;
            han.GripValue = 1;
        }
        Vector3 rhWantPos = RHAnchor.position;
        Quaternion rhWantRot = RHAnchor.rotation;
        if (DummyArrow.gameObject.activeInHierarchy)
        {
            rhWantPos = RHArrowAnchor.position;
            rhWantRot = RHArrowAnchor.rotation;
        }
        else if(plr.nvbow.orbReadied)
        {
            rhWantPos = RHOrbAnchor.position;
            rhWantRot = RHOrbAnchor.rotation;
        }
        HandRight.transform.position = Vector3.Lerp(HandRight.transform.position, rhWantPos, 9 * Time.unscaledDeltaTime);
        HandRight.transform.rotation = Quaternion.Lerp(HandRight.transform.rotation, rhWantRot, 9 * Time.unscaledDeltaTime);
        HeadHolder.position = HeadAnchor.position;
        HeadHolder.rotation = HeadAnchor.rotation;
        if (Quiver.parent != QuiverAnchor)
        {
            Quiver.transform.SetParent(QuiverAnchor);
            Quiver.transform.localPosition = Vector3.zero;
            Quiver.transform.localRotation = Quaternion.identity;
        }
    }

    #region View Changing
    public void ForceFP(bool on)
    {
        forceFirstPerson = on;
        UpdateView();
    }

    public void ChangeView()
    {
        FirstPerson = !FirstPerson;
        plr.TPCamMgr.ChangedView();
        UpdateView();
    }

    bool inFirstPerson;

    public static void CheckView()
    {
        if (NVR_Player.instance != null && NVR_Player.isThirdPerson())
            NVR_Player.instance.pos.UpdateView();

    }

    public void UpdateView()
    {
        if (!FirstPerson && !forceFirstPerson)
        {
            inFirstPerson = false;
            foreach (var mr in HeadHolder.GetComponentsInChildren<MeshRenderer>())
            {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
            foreach (var mr in HeadHolder.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
            foreach(var pt in HeadHolder.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.EmissionModule em = pt.emission;
                em.enabled = true;
                pt.Clear();
            }
        }
        else
        {
            inFirstPerson = true;
            foreach (var mr in HeadHolder.GetComponentsInChildren<MeshRenderer>())
            {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            foreach (var mr in HeadHolder.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            foreach (var pt in HeadHolder.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.EmissionModule em = pt.emission;
                em.enabled = false;
                pt.Clear();
            }
        }
        RotationHolder.localEulerAngles = Vector3.zero;
        AimPoint.transform.localPosition = new Vector3(AimPoint.transform.localPosition.x, 0, AimPoint.transform.localPosition.z);
    }

    public bool isFirstPerson()
    {
        return inFirstPerson;
    }

    #endregion

    public void ToggleHands(bool val)
    {
        HandLeft.gameObject.SetActive(val);
        HandRight.gameObject.SetActive(val);
    }

    public void MoveAim()
    {
        MAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if(!FirstPerson || !Input.GetMouseButton(1))
            RotationHolder.transform.localEulerAngles += Vector3.up * Time.unscaledDeltaTime * MAxis.x * TurnSpeed;
        AimPoint.transform.position += Vector3.up * Time.unscaledDeltaTime * MAxis.y * VerticalSpeed;
        Vector3 v = AimPoint.transform.localPosition;
        v.y = Mathf.Clamp(v.y, -10, 15);
        AimPoint.transform.localPosition = v;
        HeadAnchor.LookAt(AimPoint);
    }

}
