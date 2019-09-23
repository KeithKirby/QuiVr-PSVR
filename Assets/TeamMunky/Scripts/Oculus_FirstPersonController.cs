using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using CartWheelCore;

public class Oculus_FirstPersonController : MonoBehaviour {
    /*
    public bool isLeft;

    [HideInInspector]
    public TouchPoint touchPoint;

    private PlayerControllerNetwork handPoint;
    private PlayerBip player = null;

    // Use this for initialization
    void Start () {
		
	}

    public void InitController(TouchPoint tp, PlayerControllerNetwork hp)
    {
        touchPoint = tp;
        player = touchPoint.owner.GetComponent<PlayerBip>();
        handPoint = hp;
    }

    private bool getTriggerDown()
    {
        return (isLeft) ? OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) : OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
    }
    private bool getTrigger()
    {
        return (isLeft) ? OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) : OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
    }
    private bool getTriggerUp()
    {
        return (isLeft) ? OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) : OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);
    }
    private bool getThumbstick()
    {
        return (isLeft) ? OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown | OVRInput.Button.PrimaryThumbstickLeft | OVRInput.Button.PrimaryThumbstickRight | OVRInput.Button.PrimaryThumbstickUp) : OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown | OVRInput.Button.SecondaryThumbstickLeft | OVRInput.Button.SecondaryThumbstickRight | OVRInput.Button.SecondaryThumbstickUp);
    }
    private Vector2 getThumbstickDirection()
    {
        return (isLeft) ? OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) : OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
    }
    private bool getGripDown()
    {
        return (isLeft) ? OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) : OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
    }
    private bool getGripUp()
    {
        return (isLeft) ? OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger) : OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger);
    }

    void UpdateGrabbableObjects()
    {
        if (touchPoint == null) return;

        if (getGripDown())
        {
            touchPoint.startGrab();
        }
        else if (getGripUp())
        {
            touchPoint.endGrab();
        }

        touchPoint.updateGrab(transform.position, transform.rotation);
    }

    void LateUpdate()
    {
        if (!GameGlobeData.isGameManagerReady) return;
        var gm = GameManager.instance;
        if (null != gm)
        {
            if (GameManager.instance.GameState == GameState.MainMenu)
            {
                if (getTriggerDown())
                {
                    GameManager.instance.changeGameState(GameState.MeasuringHeight);
                }
            }
            else if (GameManager.instance.GameState == GameState.MeasuringHeight)
            {
                if (getTriggerUp())
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
                if (getThumbstick())
                {
                    if (((isLeft && !GameManager.instance.rightPlayAreaMoving) || (!isLeft && !GameManager.instance.leftPlayAreaMoving)))
                    {
                        Vector2 touchpad = getThumbstickDirection();
                        if (touchpad.magnitude > 0.5f)
                        {
                            if (isLeft)
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
                        else
                        {
                            if (isLeft)
                            {
                                GameManager.instance.leftPlayAreaMoving = false;
                            }
                            else
                            {
                                GameManager.instance.rightPlayAreaMoving = false;
                            }
                        }
                    }
                }
                else
                {
                    if (isLeft)
                    {
                        GameManager.instance.leftPlayAreaMoving = false;
                    }
                    else
                    {
                        GameManager.instance.rightPlayAreaMoving = false;
                    }
                }

                // if (touchPoint.grabber != null && !touchPoint.grabber.HoldingSomething())
                //  {
                if (getGripDown())
                {
                    handPoint.startDrag();
                }
                else if (getGripUp())
                {
                    handPoint.endDrag();
                }
                // }

                if (GameGlobeData.multiPlayerRole == MultiPlayerRole.server && GameManager.instance.players.Count < 2 && GameManager.instance.GameState > GameState.OutSide && isLeft)
                {
                    if (OVRInput.GetDown(OVRInput.Button.Start))
                    {
                        player.setPhoneMenuVisible(true);
                    }
                    else if (OVRInput.GetUp(OVRInput.Button.Start))
                    {
                        player.setPhoneMenuVisible(false);
                    }
                }
                else if (GameGlobeData.multiPlayerRole == MultiPlayerRole.client && GameManager.instance.GameState > GameState.OutSide && isLeft)
                {
                    if (OVRInput.GetDown(OVRInput.Button.Start))
                    {
                        player.setPhoneMenuVisible(true);
                    }
                    else if (OVRInput.GetUp(OVRInput.Button.Start))
                    {
                        player.setPhoneMenuVisible(false);
                    }
                }

                if (GameGlobeData.isNetwork)
                {
                    if (getTriggerDown())
                    {
                        GameManager.instance.voiceChat.StartRecording();
                    }
                    else if (getTriggerUp())
                    {
                        GameManager.instance.voiceChat.StopRecording();
                    }
                }
            }
        }
    }
    */
}
