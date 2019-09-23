using UnityEngine;
using System.Collections;

public class JointeDisabler : MonoBehaviour {

    private float mass = -1;

    public enum JointType
    {
        Character,
        Hinge
    }
    private JointType t;
    private GameObject ConnectedBody;
    private Vector3 Anchor;
    private Vector3 Axis;
    private bool AutoConfigureConnectedAnchor;
    private Vector3 ConnectedAnchor;
    private Vector3 SwingAxis;
    private SoftJointLimit LowTwistLimit;
    private SoftJointLimit HighTwistLimit;
    private SoftJointLimit Swing1Limit;
    private SoftJointLimit Swing2Limit;
    private float BreakForce;
    private float BreakTorque;
    private bool EnableCollision;
    private bool useLimits;
    private JointLimits Limit;

    public void CopyValuesAndDestroyJoint(Joint joint)
    {
        if(joint is CharacterJoint)
        {
            t = JointType.Character;
            CharacterJoint j = joint as CharacterJoint;
            ConnectedBody = j.connectedBody.gameObject;
            Anchor = j.anchor;
            Axis = j.axis;
            AutoConfigureConnectedAnchor = j.autoConfigureConnectedAnchor;
            ConnectedAnchor = j.connectedAnchor;
            SwingAxis = j.swingAxis;
            LowTwistLimit = j.lowTwistLimit;
            HighTwistLimit = j.highTwistLimit;
            Swing1Limit = j.swing1Limit;
            Swing2Limit = j.swing2Limit;
            BreakForce = j.breakForce;
            BreakTorque = j.breakTorque;
            EnableCollision = j.enableCollision;
        }
        else if(joint is HingeJoint)
        {
            t = JointType.Hinge;
            HingeJoint j = joint as HingeJoint;
            ConnectedBody = j.connectedBody.gameObject;
            Anchor = j.anchor;
            Axis = j.axis;
            AutoConfigureConnectedAnchor = j.autoConfigureConnectedAnchor;
            ConnectedAnchor = j.connectedAnchor;
            useLimits = j.useLimits;
            Limit = j.limits;
            BreakForce = j.breakForce;
            BreakTorque = j.breakTorque;
            EnableCollision = j.enableCollision;
        }
        Destroy(joint);
        if (GetComponent<Rigidbody>() != null)
        {
            mass = GetComponent<Rigidbody>().mass;
            Destroy(GetComponent<Rigidbody>());
        }
    }

    public void TryAddRB()
    {
        if (mass > -1)
        {
            if (gameObject.GetComponent<Rigidbody>() == null)
            {
                Rigidbody r = gameObject.AddComponent<Rigidbody>();
                if (r != null)
                    r.mass = mass;
            }
        }
    }

    public void CreateJointAndDestoryThis()
    {
        if(t == JointType.Character)
        {
            CharacterJoint j = gameObject.AddComponent<CharacterJoint>();
            j.connectedBody = ConnectedBody.GetComponent<Rigidbody>();
            j.anchor = Anchor;
            j.axis = Axis;
            j.autoConfigureConnectedAnchor = AutoConfigureConnectedAnchor;
            j.connectedAnchor = ConnectedAnchor;
            j.swingAxis = SwingAxis;
            j.lowTwistLimit = LowTwistLimit;
            j.highTwistLimit = HighTwistLimit;
            j.swing1Limit = Swing1Limit;
            j.swing2Limit = Swing2Limit;
            j.breakForce = BreakForce;
            j.breakTorque = BreakTorque;
            j.enableCollision = EnableCollision;
        }
        else if(t == JointType.Hinge)
        {
            HingeJoint j = gameObject.AddComponent<HingeJoint>();
            j.connectedBody = ConnectedBody.GetComponent<Rigidbody>();
            j.anchor = Anchor;
            j.axis = Axis;
            j.autoConfigureConnectedAnchor = AutoConfigureConnectedAnchor;
            j.connectedAnchor = ConnectedAnchor;
            j.useLimits = useLimits;
            j.limits = Limit;
            j.breakForce = BreakForce;
            j.breakTorque = BreakTorque;
            j.enableCollision = EnableCollision;
        }
        Destroy(this);
    }
}
