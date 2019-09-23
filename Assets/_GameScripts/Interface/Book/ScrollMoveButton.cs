using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMoveButton : MonoBehaviour {

    public Vector3 Movement;
    public Transform ScrollContent;

    public void DoMovement()
    {
        ScrollContent.transform.localPosition += Movement;
    }
}
