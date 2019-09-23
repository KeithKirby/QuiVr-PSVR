using UnityEngine;
using System.Collections;

public class InputVisualiser : MonoBehaviour
{
    // Custom class for holding all the gamepad sprites
    [System.Serializable]
    public class Controller
    {
        public Transform button_cross;
        public Transform button_circle;
        public Transform button_square;
        public Transform button_triangle;

        public Transform button_options;
        public Transform button_touchpad;

        public Transform dpad_down;
        public Transform dpad_right;
        public Transform dpad_up;
        public Transform dpad_left;

        public Transform button_l1;
        public Transform button_r1;
        public Transform button_r1_2; // Aim controller only
        public Transform trigger_l2;
        public Transform trigger_r2;
        public Transform thumbstick_l3;
        public Transform thumbstick_r3;
    }

    public Controller controller;
    public float buttonPressDistance;
    public float triggerPullAngle;
    public float thumbstickAngle;

    void Update()
    {
        controller.button_cross.localPosition = Input.GetButton("Fire1") ? Vector3.up * buttonPressDistance : Vector3.zero;
        controller.button_circle.localPosition = Input.GetButton("Fire2") ? Vector3.up * buttonPressDistance : Vector3.zero;
        controller.button_square.localPosition = Input.GetButton("Fire3") ? Vector3.up * buttonPressDistance : Vector3.zero;
        controller.button_triangle.localPosition = Input.GetButton("Fire4") ? Vector3.up * buttonPressDistance : Vector3.zero;

        controller.button_options.localPosition = Input.GetButton("Aux4") ? Vector3.up * buttonPressDistance : Vector3.zero;
        controller.button_touchpad.localPosition = Input.GetButton("Aux3") ? Vector3.up * buttonPressDistance : Vector3.zero;

        controller.dpad_right.localPosition = Input.GetAxisRaw("DPADHorizontal") > 0 ? Vector3.up * buttonPressDistance : Vector3.zero;
        controller.dpad_left.localPosition = Input.GetAxisRaw("DPADHorizontal") < 0 ? Vector3.up * buttonPressDistance : Vector3.zero;
        controller.dpad_up.localPosition = Input.GetAxisRaw("DPADVertical") > 0 ? Vector3.up * buttonPressDistance : Vector3.zero;
        controller.dpad_down.localPosition = Input.GetAxisRaw("DPADVertical") < 0 ? Vector3.up * buttonPressDistance : Vector3.zero;

        controller.button_l1.localPosition = Input.GetButton("ShoulderLeft") ? -Vector3.forward * buttonPressDistance : Vector3.zero;
        controller.button_r1.localPosition = Input.GetButton("ShoulderRight") ? Vector3.left * buttonPressDistance : Vector3.zero;
        if(controller.button_r1_2 != null)
            controller.button_r1_2.localPosition = Input.GetButton("ShoulderRight") ? Vector3.left * buttonPressDistance : Vector3.zero;

        controller.trigger_l2.localRotation = Quaternion.Euler(new Vector3(Input.GetAxisRaw("TriggerLeft") * triggerPullAngle, 0, 0));
        controller.trigger_r2.localRotation = Quaternion.Euler(new Vector3(-Input.GetAxisRaw("TriggerRight") * triggerPullAngle, 0, 0));

        controller.thumbstick_l3.localRotation = Quaternion.Euler(new Vector3(Input.GetAxisRaw("Vertical") * thumbstickAngle, 0, -Input.GetAxisRaw("Horizontal") * thumbstickAngle));
        controller.thumbstick_r3.localRotation = Quaternion.Euler(new Vector3(Input.GetAxisRaw("Vertical2") * thumbstickAngle, 0, -Input.GetAxisRaw("Horizontal2") * thumbstickAngle));
    }
}
