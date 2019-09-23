/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class VRCharacterController : MonoBehaviour {

    public GameObject headSet;
    public GameObject handController1;
    public GameObject handController2;
    public PlayerControllerNetwork headPoint;
    public PlayerControllerNetwork handLeftPoint;
    public PlayerControllerNetwork handRightPoint;
    public TouchPoint leftTouch;
    public TouchPoint rightTouch;

    public PlayerBip player;
    public VRIK characterIK;
    public FullBodyBipedIK fullBodyIK = null;
    // public BipedIK armIK = null;
    // public FBBIKHeadEffector headIK = null;
    // public LookAtIK lookIK = null;
    // public GameObject bodyEffect = null;
    public GameObject leftArmEffect = null;
    public GameObject rightArmEffect = null;
    public GameObject leftFootTarget = null;
    public GameObject rightFootTarget = null;
    public ViveGrip_ControllerHandler leftControllerHandler = null;
    public ViveGrip_ControllerHandler rightControllerHandler = null;

    public Vector3 headControllerOriginalPos;
    public Vector3 headControllerOrginalQua;

    public Vector3 handLeftControllerOriginalPos;
    public Vector3 handLeftControllerOriginalQua; //49,98.5,-101
    public Vector3 handRightControllerOriginalPos;
    public Vector3 handRightControllerOriginalQua; //-49,81.5,79

    [HideInInspector]
    public Vector3 pickLeftTargetPosition = Vector3.zero;
    [HideInInspector]
    public Vector3 pickRightTargetPosition = Vector3.zero;

    [HideInInspector]
    public GameObject leftController = null;
    [HideInInspector]
    public GameObject rightController = null;

    protected GameObject leftHand;
    protected GameObject rightHand;

    protected Vector3 prevHeadPos;
    protected Vector3 currHeadPos;
    protected Vector3 prevHand1Pos;
    protected Vector3 currHand1Pos;
    protected Vector3 prevHand2Pos;
    protected Vector3 currHand2Pos;

    protected Quaternion prevHeadRot;
    protected Quaternion currHeadRot;
    protected Quaternion prevHand1Rot;
    protected Quaternion currHand1Rot;
    protected Quaternion prevHand2Rot;
    protected Quaternion currHand2Rot;

    protected Quaternion rootRotaion = Quaternion.identity;
    protected float vrikTimer = 0;

    public void setVRIKLegTargetEnable(bool en)
    {
        if (en)
        {
            characterIK.solver.leftLeg.positionWeight = 1;
            characterIK.solver.rightLeg.positionWeight = 1;
        }
        else
        {
            characterIK.solver.leftLeg.positionWeight = 0;
            characterIK.solver.rightLeg.positionWeight = 0;
        }
    }

    protected bool _wantShowControllers = true;

    public void ShowControllers(bool show)
    {
        if (_wantShowControllers != show)
        {
            _wantShowControllers = show;
            OnControllerVisibilityChanged();
        }
    }

    virtual public void OnControllerVisibilityChanged()
    {
    }
}
*/