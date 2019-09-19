using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffsetCycler : MonoBehaviour
{
    public enum ControllerSide
    {
        Left,
        Right
    }

    public enum ControllerButton
    {
        Triangle,
        Circle,
        Square,
        Cross
    }

    public ControllerSide Controller;
    public ControllerButton Button;

    public BowAim Aim;
    public GameObject Hand;
    public Transform[] Offsets;

    public int CurrentOffset = 0;

	void Update()
    {
        var controller = PS4InputEx.GetMove(Controller == ControllerSide.Left ? 1 : 0);
        if (null != controller)
        {
            switch (Button)
            {
                case ControllerButton.Square:
                    if (controller.MenuA.Down)
                        CyclePosition();
                    break;
                case ControllerButton.Triangle:
                    if (controller.MenuB.Down)
                        CyclePosition();
                    break;                                
                case ControllerButton.Cross:
                    if (controller.CrossButton.Down)
                        CyclePosition();
                    break;
                case ControllerButton.Circle:
                    if (controller.CircleButton.Down)
                        CyclePosition();
                    break;
            }
        }
    }

    void CyclePosition()
    {
        if (++CurrentOffset == Offsets.Length)
            CurrentOffset = 0;
        var co = Offsets[CurrentOffset];
        Hand.transform.localPosition = co.localPosition;
        Hand.transform.localRotation = co.localRotation;

        if (null != Aim)
        {
            var localUp = co.transform.localRotation * new Vector3(0, 0, 1);
            Debug.Log("LocalUp:" + localUp);
            var upDot = Vector3.Dot(new Vector3(0, 1, 0), localUp);
            if (localUp.z > 0.3f)
                Aim.BowOrient = BowOrientation.Vertical;
            else
                Aim.BowOrient = BowOrientation.Horizontal;
        }
    }
}