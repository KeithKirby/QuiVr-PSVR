using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ArmorRacks : MonoBehaviour {

    public SelectionSpinner[] Selectors;
    public float DistanceThreshold = 5f;

    public GameObject left;
    public GameObject right;

    void FixedUpdate()
    {
        if (NVR_Player.instance != null && NVR_Player.instance.NonVR)
            return;
        if (left == null || right == null || !right.activeInHierarchy || !left.activeInHierarchy)
        {
            left = VRTK_DeviceFinder.GetControllerLeftHand();
            right = VRTK_DeviceFinder.GetControllerRightHand();
            return;
        }
        Vector3 p1 = left.transform.position;
        Vector3 p2 = right.transform.position;
        SelectionSpinner closest = Selectors[0];
        float dist = float.MaxValue;
        foreach (var v in Selectors)
        {
            float minDist = Mathf.Min(Vector3.Distance(v.transform.position, p1), Vector3.Distance(v.transform.position, p2));
            if (minDist > DistanceThreshold || !v.gameObject.activeSelf)
                return;
            if (minDist < dist)
            {
                dist = minDist;
                closest = v;
            }
        }
        if (dist < DistanceThreshold)
        {
            foreach (var v in Selectors)
            {
                if (closest == v)
                {
                    v.ShowClosest(p1, p2);
                }
                else
                {
                    v.HideAll();
                }
            }
        }
        else
        {
            foreach (var v in Selectors)
            {
                v.HideAll();
            }
        }
    }
}
