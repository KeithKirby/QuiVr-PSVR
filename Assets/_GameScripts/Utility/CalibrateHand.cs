using UnityEngine;
using System.Collections;
using VRTK;

public class CalibrateHand : MonoBehaviour {

    public int controllerID;

    Vector3 lockPos;
    Quaternion lockRot;

    Vector3 defaultLocalPos;
    Quaternion defaultLocalRot;

    Vector3 localPos;
    Quaternion localRot;

    VRTK_ControllerEvents events;

    bool locked;

    public GameObject ControllerDisplay;
    public GameObject Tooltip;
    bool DisplaySetup;

    Transform holder;

    void Start()
    {
        events = GetComponentInParent<VRTK_ControllerEvents>();
        defaultLocalPos = transform.localPosition;
        defaultLocalRot = transform.localRotation;
        localPos = defaultLocalPos;
        localRot = defaultLocalRot;
        if(Settings.HasKey("handPos" + controllerID) && Settings.HasKey("handRot" + controllerID))
        {
            localPos = Settings.GetV3("handPos" + controllerID);
            localRot = Settings.GetQuat("handRot" + controllerID);
            SetPosition();
        }
        else
        {
            Settings.Set("handPos" + controllerID, defaultLocalPos);
            Settings.Set("handRot" + controllerID, defaultLocalRot);
        }

    }

	public void StartCalibration()
    {
        holder = transform.parent;
        transform.SetParent(transform.parent.parent);
        Tooltip.SetActive(true);
        ControllerDisplay.SetActive(true);
        lockPos = transform.position;
        lockRot = transform.rotation;
        locked = true;
        events.TriggerClicked += new ControllerInteractionEventHandler(EndCalibration);
    }

    void Update()
    {
        if (locked)
        {
            StopCoroutine("LPos");
            StartCoroutine("LPos");
            transform.position = lockPos;
            transform.rotation = lockRot;
        }
        else if (DisplaySetup && !locked)
            ControllerDisplay.SetActive(false);
        else if (!DisplaySetup)
            CheckSetup();

    }

    void CheckSetup()
    {
        if (ControllerDisplay.GetComponentInChildren<MeshRenderer>() != null)
            DisplaySetup = true;
    }

    void LateUpdate()
    {
        if(locked)
        {
            transform.position = lockPos;
            transform.rotation = lockRot;
        }
    }

    IEnumerator LPos()
    {
        yield return new WaitForEndOfFrame();
        transform.position = lockPos;
        transform.rotation = lockRot;
    }

    void EndCalibration(object o, ControllerInteractionEventArgs e)
    {
        events.TriggerClicked -= EndCalibration;
        Debug.Log("Ending calibration");
        EndCalibration();
    }

    public Vector3 GetBasePos()
    {
        return localPos;
    }

    public Quaternion GetBaseRot()
    {
        return localRot;
    }

    public void EndCalibration()
    {
        Tooltip.SetActive(false);
        ControllerDisplay.SetActive(false);
        locked = false;
        transform.SetParent(holder);
        localPos = transform.localPosition;
        localRot = transform.localRotation;
        SaveCalibration();
        SetPosition();
    }

    public void HideController()
    {
        ControllerDisplay.SetActive(false);
    }

    public void ResetDefault()
    {
        localPos = defaultLocalPos;
        localRot = defaultLocalRot;
        SaveCalibration();
        SetPosition();
    }

    void SaveCalibration()
    {
        Settings.Set("handPos" + controllerID, localPos);
        Settings.Set("handRot" + controllerID, localRot);
    }

    void SetPosition()
    {
        transform.localPosition = localPos;
        transform.localRotation = localRot;
    }

}
