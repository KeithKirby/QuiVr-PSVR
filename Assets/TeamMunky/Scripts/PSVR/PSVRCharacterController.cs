/*using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using RootMotion.Dynamics;
using UnityEngine.SceneManagement;

public class PSVRCharacterController : VRCharacterController
{
#if UNITY_PS4
    public Editor_FirstPersonController EditorController;
    public float ZOffset = 0;

    float _reactivationDelay = 0.0f;
    bool _headInitialised = false;

    // Use this for initialization
    void Start()
    {
        leftController = null;
        rightController = null;

        //characterIK.enabled = false;
        
        //headControllerOriginalPos = new Vector3(0, -0.03f, 0);
        //headControllerOrginalQua = Quaternion.Euler(0, -90, -90);

        //handLeftControllerOriginalPos = new Vector3(0, 0, -0.15f);
        //handRightControllerOriginalPos = new Vector3(0, 0, -0.15f);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _headInitialised = false;
        leftController = null;
        rightController = null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // if (characterIK == null) return;

        var gm = GameManager.instance;
        if (null == gm || gm.GameState <= GameState.MeasuringHeight)
            return;

        if (player == null)
            return;

        if (_reactivationDelay != 0)
        {
            _reactivationDelay -= Time.deltaTime;
            if (_reactivationDelay < 0)
            {
                _reactivationDelay = 0;
                handController1.SetActive(true);
                handController2.SetActive(true);
            }
        }

        if (!player.getIsFallDown())
        {
            bool headIsReady = false;
            bool hand1IsReady = false;
            bool hand2IsReady = false;
            if (headSet.activeInHierarchy)
            {
                headIsReady = true;
                prevHeadPos = currHeadPos;
                currHeadPos = headSet.transform.position;
                prevHeadRot = currHeadRot;
                currHeadRot = headSet.transform.rotation;
            }
            else
            {
                prevHeadPos = currHeadPos = Vector3.zero;
            }
            if (handController1.activeInHierarchy)
            {
                hand1IsReady = true;

            }
            if (handController2.activeInHierarchy)
            {
                hand2IsReady = true;
            }

            if (EditorController != null && !EditorController.Initialised)
            {
                EditorController.InitController(leftTouch, handLeftPoint, rightTouch, handRightPoint);
            }

            vrikTimer += 50 * Time.deltaTime;
            if (vrikTimer > 15)
            {
                characterIK.enabled = true;
                vrikTimer = 30;
            }
            if (headIsReady)
            {
                if (!_headInitialised)
                {
                    _headInitialised = true;
                    characterIK.solver.spine.headTarget = headPoint.transform;
                    headPoint.setController(
                        headSet,
                        headControllerOriginalPos,
                        Quaternion.Euler(0.0f, -90.0f, 0.0f) *
                        Quaternion.Euler(-headControllerOrginalQua.x, headControllerOrginalQua.y, headControllerOrginalQua.z)
                        );
                    characterIK.solver.IKPositionWeight = 1;
                    fullBodyIK.solver.IKPositionWeight = 0;
                }
            }
            if (hand1IsReady)
            {
                if (leftController == null)
                {
                    leftHand = leftTouch.hand;
                    characterIK.solver.leftArm.target = handLeftPoint.transform;
                    prevHand1Pos = currHand1Pos = Vector3.zero;
                    //int leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
                    //  Debug.Log("agsdhgshdgshgdhsgdhsgdhsgdh === left controller  ====== " + leftIndex);
                    if (handController1.GetComponent<PSVR_FirstPersonController>().isSecondaryMoveController)
                    {
                        leftController = handController1;
                    }
                    else if (handController2.GetComponent<PSVR_FirstPersonController>().isSecondaryMoveController)
                    {
                        leftController = handController2;
                    }
                    if (leftController != null)
                    {
                        handLeftPoint.setController(leftController, handLeftControllerOriginalPos, Quaternion.Euler(0.0f, -90.0f, 0.0f));
                        //rjy
                        //leftControllerHandler.trackedObject = leftController.GetComponent<SteamVR_TrackedObject>();
                        var lConComp = leftController.GetComponent<PSVR_FirstPersonController>();
                        lConComp.InitController(leftTouch, handLeftPoint);
                        lConComp.model.SetActive(_wantShowControllers);
                    }
                }
                else
                {
                    prevHand1Pos = currHand1Pos;
                    currHand1Pos = leftController.transform.position;
                    prevHand1Rot = currHand1Rot;
                    currHand1Rot = leftController.transform.rotation;

                    if (characterIK.enabled)
                    {
                        //leftHand.transform.rotation = currHand1Rot * Quaternion.Euler(handLeftPoint.OpenVRoffsetRotation);
                        leftArmEffect.transform.position = currHand1Pos + currHand1Rot * (5 * Vector3.back + 0.7f * Vector3.left);
                        var a = Vector3.Angle(currHand1Rot * (Vector3.up), Vector3.right);
                        if (a < 30 
                            && player.getHandGesture().currentLeft != HandGesture.Gesture.Finger 
                            && player.getHandGesture().currentLeft == HandGesture.Gesture.Hold
                            && player.getHandGesture().currentLeft != HandGesture.Gesture.Point
                            && player.getHandGesture().currentLeft != HandGesture.Gesture.Flat
                            && !leftTouch.grabber.HoldingSomething())
                        {
                            player.getHandGesture().Apply(HandGesture.Hand.Left, HandGesture.Gesture.Finger);
                            if (GameGlobeData.isNetwork && !player.isNetworked)
                            {
                                player.network.changeHandGesture(HandGesture.Hand.Left, HandGesture.Gesture.Finger);
                            }
                            DiscoveryManager.DiscoveryFound(DiscoveryID.Salutz);
                            if (GameGlobeData.isNetwork)
                            {
                                GameManager.instance.sendDiscoveryFound(DiscoveryID.Salutz);
                            }
                        }
                    }
                }
            }
            else
            {
                leftController = null;
            }

            if (hand2IsReady)
            {
                if (rightController == null)
                {
                    rightHand = rightTouch.hand;
                    characterIK.solver.rightArm.target = handRightPoint.transform;
                    prevHand2Pos = currHand2Pos = Vector3.zero;
                    //nt rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                    // Debug.Log("agsdhgshdgshgdhsgdhsgdhsgdh === right controller  ====== " + rightIndex);
                    if (!handController1.GetComponent<PSVR_FirstPersonController>().isSecondaryMoveController)
                    {
                        rightController = handController1;
                    }
                    else if (!handController2.GetComponent<PSVR_FirstPersonController>().isSecondaryMoveController)
                    {
                        rightController = handController2;
                    }
                    if (rightController != null)
                    {
                        handRightPoint.setController(rightController, handRightControllerOriginalPos, Quaternion.Euler(0.0f, 90.0f, 0.0f));
                        //rightControllerHandler.trackedObject = rightController.GetComponent<SteamVR_TrackedObject>();
                        var rConComp = rightController.GetComponent<PSVR_FirstPersonController>();
                        rConComp.InitController(rightTouch, handRightPoint);
                        rConComp.model.SetActive(_wantShowControllers);
                    }
                }
                else
                {
                    prevHand2Pos = currHand2Pos;
                    currHand2Pos = rightController.transform.position;
                    prevHand2Rot = currHand2Rot;
                    currHand2Rot = rightController.transform.rotation;

                    if (characterIK.enabled)
                    {
                        //rightHand.transform.rotation = currHand2Rot * Quaternion.Euler(handRightPoint.OpenVRoffsetRotation);
                        rightArmEffect.transform.position = currHand2Pos + currHand2Rot * (5 * Vector3.back + 0.7f * Vector3.right);
                        if (Vector3.Angle(currHand2Rot * (Vector3.up), Vector3.right) < 30 
                            && player.getHandGesture().currentRight != HandGesture.Gesture.Finger
                            && player.getHandGesture().currentRight == HandGesture.Gesture.Hold
                            && player.getHandGesture().currentRight != HandGesture.Gesture.Point
                            && player.getHandGesture().currentRight != HandGesture.Gesture.Flat
                            && !rightTouch.grabber.HoldingSomething())
                        {
                            player.getHandGesture().Apply(HandGesture.Hand.Right, HandGesture.Gesture.Finger);
                            if (GameGlobeData.isNetwork && !player.isNetworked)
                            {
                                player.network.changeHandGesture(HandGesture.Hand.Right, HandGesture.Gesture.Finger);
                            }
                            DiscoveryManager.DiscoveryFound(DiscoveryID.Salutz);
                            if (GameGlobeData.isNetwork)
                            {
                                GameManager.instance.sendDiscoveryFound(DiscoveryID.Salutz);
                            }
                        }
                    }
                }
            }
            else
            {
                rightController = null;
            }
        }
        else
        {
            characterIK.solver.IKPositionWeight = 0;
            fullBodyIK.solver.IKPositionWeight = 1;
            characterIK.enabled = false;
        }
    }

    public void DisableControls()
    {
        leftController.SetActive(false);
        rightController.SetActive(false);
        _reactivationDelay = 1;
    }

    public override void OnControllerVisibilityChanged()
    {
        if (leftController != null)
            leftController.GetComponent<PSVR_FirstPersonController>().model.SetActive(_wantShowControllers);
        if (rightController != null)
            rightController.GetComponent<PSVR_FirstPersonController>().model.SetActive(_wantShowControllers);
    }
#endif
}

*/