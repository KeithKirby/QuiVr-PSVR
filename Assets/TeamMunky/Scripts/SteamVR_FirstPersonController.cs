using UnityEngine;
using System.Collections;
//using CartWheelCore;
//using EnergyBarToolkit;

public class SteamVR_FirstPersonController : MonoBehaviour
{
#if OPENVR
    [HideInInspector]
    public GameObject model;

	[HideInInspector]
    public SteamVR_TrackedObject trackedController;
    [HideInInspector]
    public TouchPoint touchPoint;

    [HideInInspector]
    public bool indexSet = false;

    [HideInInspector]
	public SteamVR_Controller.Device device = null;

    // public GameObject controllerAttachPoint;
    private PlayerControllerNetwork handPoint;

    private PlayerBip player = null;

    void Awake()
    {
        indexSet = false;
        trackedController = gameObject.GetComponent<SteamVR_TrackedObject>();
    }

    void Start()
    {
        model = transform.Find("Model").gameObject;
    }

    public void SetDeviceIndex(int index)
    {
        indexSet = true;
        device = SteamVR_Controller.Input(index);
    }

    public void InitController(TouchPoint tp, PlayerControllerNetwork hp)
    {
        touchPoint = tp;
        player = touchPoint.owner.GetComponent<PlayerBip>();
        handPoint = hp;
        // StartCoroutine(hideController());
    }

    private IEnumerator hideController()
    {
        yield return new WaitForSeconds(1);

        model.SetActive(false);
    }

    void UpdateGrabbableObjects()
    {
        if (touchPoint == null || device == null) return;

        if(GameGlobeData.deviceType == DeviceType.htcvive)
        {
            if (device.GetHairTriggerDown())
            {
                touchPoint.startGrab();
            }
            else if (device.GetHairTrigger())
            {
            }
            else if (device.GetHairTriggerUp())
            {
                touchPoint.endGrab();
            }
        }
        else
        {
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                touchPoint.startGrab();
            }
            else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                touchPoint.endGrab();
            }
        }
        touchPoint.updateGrab(transform.position, transform.rotation);
    }
    
    void LateUpdate()
    {
        if (device == null || !GameGlobeData.isGameManagerReady) return;

        if (GameManager.instance.GameState == GameState.MainMenu)
        {
            if (device.GetHairTriggerDown())
            {
                GameManager.instance.changeGameState(GameState.MeasuringHeight);
            }
        }
        else if (GameManager.instance.GameState == GameState.MeasuringHeight)
        {
            if (device.GetHairTriggerUp())
            {
                GameManager.instance.changeGameState(GameState.MainMenu);
            }
        }
        else if (GameManager.instance.GameState > GameState.OutSide)
        {
            UpdateGrabbableObjects();
        }

        if (GameManager.instance.GameState > GameState.MeasuringHeight && touchPoint != null)
        {
            if (GameGlobeData.deviceType == DeviceType.htcvive)
            {
                Vector2 touchpad = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad));
                if (touchpad.magnitude > 0 && device.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
                {
                    if (((touchPoint.isLeft && !GameManager.instance.rightPlayAreaMoving) || (!touchPoint.isLeft && !GameManager.instance.leftPlayAreaMoving)))
                    {
                        if (touchPoint.isLeft)
                        {
                            GameManager.instance.leftPlayAreaMoving = true;
                        }
                        else
                        {
                            GameManager.instance.rightPlayAreaMoving = true;
                        }
                        Vector3 point = transform.rotation * Vector3.forward;
                        point.y = 0;
                        point.Normalize();
                        Vector3 dir = new Vector3(touchpad.x, 0, touchpad.y);

                        float ang = Vector3.Angle(new Vector3(0, 0, 1), dir);
                        Vector3 cross = Vector3.Cross(new Vector3(0, 0, 1), dir);
                        if (cross.y < 0) ang = -ang;

                        Vector3 move = Quaternion.AngleAxis(ang, Vector3.up) * point;
                        GameManager.instance.MovePlayerArea(move, 3 * Time.deltaTime);
                    }
                }
                else
                {
                    if (touchPoint.isLeft)
                    {
                        GameManager.instance.leftPlayAreaMoving = false;
                    }
                    else
                    {
                        GameManager.instance.rightPlayAreaMoving = false;
                    }
                }
              //  if (touchPoint.grabber == null || (touchPoint.grabber != null && !touchPoint.grabber.HoldingSomething()))
               // {
                    if (device.GetHairTriggerDown())
                    {
                        handPoint.startDrag();
                    }
                    else if (device.GetHairTriggerUp())
                    {
                        handPoint.endDrag();
                    }
               // }
            }else if(GameGlobeData.deviceType == DeviceType.oculus)
            {
                Vector2 touchpad = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad));
                if (touchpad.magnitude > 0.5f)
                {
                    if (((touchPoint.isLeft && !GameManager.instance.rightPlayAreaMoving) || (!touchPoint.isLeft && !GameManager.instance.leftPlayAreaMoving)))
                    {
                        if (touchPoint.isLeft)
                        {
                            GameManager.instance.leftPlayAreaMoving = true;
                        }
                        else
                        {
                            GameManager.instance.rightPlayAreaMoving = true;
                        }
                        Vector3 point = transform.rotation * Vector3.forward;
                        point.y = 0;
                        point.Normalize();
                        Vector3 dir = new Vector3(touchpad.x, 0, touchpad.y);

                        float ang = Vector3.Angle(new Vector3(0, 0, 1), dir);
                        Vector3 cross = Vector3.Cross(new Vector3(0, 0, 1), dir);
                        if (cross.y < 0) ang = -ang;

                        Vector3 move = Quaternion.AngleAxis(ang, Vector3.up) * point;
                        GameManager.instance.MovePlayerArea(move, 3 * Time.deltaTime);
                    }
                }
                else
                {
                    if (touchPoint.isLeft)
                    {
                        GameManager.instance.leftPlayAreaMoving = false;
                    }
                    else
                    {
                        GameManager.instance.rightPlayAreaMoving = false;
                    }
                }

               // if (touchPoint.grabber != null && !touchPoint.grabber.HoldingSomething())
                //{
                    if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
                    {
                        handPoint.startDrag();
                    }
                    else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
                    {
                        handPoint.endDrag();
                    }
                //}
            }

            if (GameGlobeData.multiPlayerRole == MultiPlayerRole.server && GameManager.instance.GameState > GameState.OutSide && touchPoint.isLeft)
            {
                if ((GameGlobeData.deviceType == DeviceType.htcvive && GameManager.instance.players.Count < 2 && !player.getIsFallDown() && device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) || (GameGlobeData.deviceType == DeviceType.oculus && GameManager.instance.players.Count < 2 && !player.getIsFallDown() && device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)))
                {
                    player.setPhoneMenuVisible(true);
                }
                else if ((GameGlobeData.deviceType == DeviceType.htcvive && device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)) || (GameGlobeData.deviceType == DeviceType.oculus && device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)))
                {
                    player.setPhoneMenuVisible(false);
                }
            }else if(GameGlobeData.multiPlayerRole == MultiPlayerRole.client && GameManager.instance.GameState > GameState.OutSide && touchPoint.isLeft)
            {
                if ((GameGlobeData.deviceType == DeviceType.htcvive && !player.getIsFallDown() && device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) || (GameGlobeData.deviceType == DeviceType.oculus && !player.getIsFallDown() && device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)))
                {
                    player.setPhoneMenuVisible(true);
                }
                else if ((GameGlobeData.deviceType == DeviceType.htcvive && device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)) || (GameGlobeData.deviceType == DeviceType.oculus && device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)))
                {
                    player.setPhoneMenuVisible(false);
                }
            }
            if (GameGlobeData.isNetwork && !touchPoint.isLeft)
            {
                if ((GameGlobeData.deviceType == DeviceType.htcvive && device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) || (GameGlobeData.deviceType == DeviceType.oculus && device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)))
                {
                    //GameManager.instance.playerObj.network.startVoiceChat();
                    // GameManager.instance.playerObj.network.StartRecording();
                    GameManager.instance.voiceChat.StartRecording();
                }
                else if ((GameGlobeData.deviceType == DeviceType.htcvive && device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) || (GameGlobeData.deviceType == DeviceType.oculus && device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)))
                {
                    //GameManager.instance.playerObj.network.stopVoiceChat();
                    // GameManager.instance.playerObj.network.StopRecording();
                    GameManager.instance.voiceChat.StopRecording();
                }
            }
        }
    }

#endif
}