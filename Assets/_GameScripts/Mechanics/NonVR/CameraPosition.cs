using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {

    public NVR_Player plr;
    public Transform CamHolder;
    public Transform Heads;
    public Transform FPHolder;
    public Transform[] HeadCameras;
    public Transform FPSPlayer;
    public Transform TPCamFollow;
    Vector3 StartPos;
    public Vector3 Movement;

    Quaternion wantRot;

    void Start()
    {
        StartPos = CamHolder.localPosition;
        wantRot = FPSPlayer.localRotation;
    }

	void Update()
    {
        if(plr.NonVR)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                wantRot *= Quaternion.Euler(0, -45, 0);
            if (Input.GetKeyDown(KeyCode.E))
                wantRot *= Quaternion.Euler(0, 45, 0);
            if (Input.GetKey(KeyCode.A))
                wantRot *= Quaternion.Euler(0, -45*Time.deltaTime, 0);
            if (Input.GetKey(KeyCode.D))
                wantRot *= Quaternion.Euler(0, 45 * Time.deltaTime, 0);
        }
        if(plr.NonVR && !plr.FirstPersonMode())
        {
            if(Heads.parent != CamHolder)
            {
                Heads.SetParent(CamHolder);
                Heads.localScale = Vector3.one;
                Heads.localPosition = Vector3.zero;
                Heads.localEulerAngles = Vector3.zero;
            }
            //Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //CamHolder.localPosition += Movement*Time.unscaledDeltaTime;
            //Scrolling
            if(!plr.nvbow.isAiming())
            {
                if (Vector3.Distance(TPCamFollow.position, transform.position) > 1f && Input.GetAxis("Mouse ScrollWheel") > 0)
                    TPCamFollow.position += (TPCamFollow.forward * Input.GetAxis("Mouse ScrollWheel") * Time.unscaledDeltaTime * 50f);
                else if (Vector3.Distance(TPCamFollow.position, transform.position) < 7f && Input.GetAxis("Mouse ScrollWheel") < 0)
                    TPCamFollow.position += (TPCamFollow.forward * Input.GetAxis("Mouse ScrollWheel") * Time.unscaledDeltaTime * 50f);
            }
            CamHolder.position = TPCamFollow.position;
            if(Input.GetMouseButton(1))
                wantRot *= Quaternion.Euler(0, Time.unscaledDeltaTime * Input.GetAxis("Mouse X") * plr.pos.TurnSpeed, 0);
                //transform.localEulerAngles += Vector3.up * Time.unscaledDeltaTime * Input.GetAxis("Mouse X") * plr.pos.TurnSpeed;

            if (Input.GetKeyDown(KeyCode.Q))
                plr.pos.RotationHolder.localEulerAngles -= Vector3.up * 45f;
            if(Input.GetKeyDown(KeyCode.E))
                plr.pos.RotationHolder.localEulerAngles += Vector3.up * 45f;
            if (Input.GetKey(KeyCode.A))
                plr.pos.RotationHolder.localEulerAngles -= Vector3.up * 45f * Time.deltaTime;
            if (Input.GetKey(KeyCode.D))
                plr.pos.RotationHolder.localEulerAngles += Vector3.up * 45f * Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, wantRot, 5 * Time.deltaTime);
        }
        else if(plr.FirstPersonMode())
        {
            //Set Parent correctly to FPS
            //Q & E change behavior
            if (Heads.parent != FPHolder.parent)
            {
                Heads.SetParent(FPHolder.parent);
                Heads.localScale = Vector3.one;
                Vector3 offset = Heads.position - GetValidHead().position;
                Heads.position = FPHolder.position + offset;
                Heads.localEulerAngles = Vector3.zero;
            }
            if (Input.GetMouseButton(1))
                wantRot *= Quaternion.Euler(0, Time.unscaledDeltaTime * Input.GetAxis("Mouse X") * plr.pos.TurnSpeed, 0);
                //FPSPlayer.localEulerAngles += Vector3.up * Time.unscaledDeltaTime * Input.GetAxis("Mouse X") * plr.pos.TurnSpeed;
            /*
            if (Input.GetKeyDown(KeyCode.Q))
                FPSPlayer.localEulerAngles -= Vector3.up * 45f;
            if (Input.GetKeyDown(KeyCode.E))
                FPSPlayer.localEulerAngles += Vector3.up * 45f;
            */
            FPSPlayer.localRotation = Quaternion.Lerp(FPSPlayer.localRotation, wantRot, 5 * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
            ResetPosition();
    }

    public void ChangedView()
    {
        if(plr.FirstPersonMode())
        {
            wantRot = FPSPlayer.localRotation;
        }
        else
        {
            wantRot = transform.localRotation;
        }
    }

    public void ResetPosition()
    {
        if(plr.FirstPersonMode())
        {
            Vector3 offset = Heads.position - GetValidHead().position;
            Heads.position = FPHolder.position + offset;
            wantRot = FPSPlayer.localRotation;
        }
        else
        {
            CamHolder.localPosition = StartPos;
            transform.localEulerAngles = Vector3.zero;
            wantRot = transform.localRotation;
        }
    }

    Transform GetValidHead()
    {
        foreach(var v in HeadCameras)
        {
            if (v.gameObject.activeInHierarchy)
                return v;
        }
        return HeadCameras[0];
    }
}
