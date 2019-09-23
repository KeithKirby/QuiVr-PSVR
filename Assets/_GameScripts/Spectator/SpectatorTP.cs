using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SpectatorTP : MonoBehaviour {

    public bool LaserOn;
    Teleporter CurSelected;
    public Transform LaserGuide;
    public LineRenderer line;

    public VRTK_ControllerEvents ctrlr;

    public Spectator spectator;

	public void StartLaser()
    {
        LaserOn = true;
    }

    public void CancelLaser()
    {
        LaserOn = false;
    }

    public void EndLaser()
    {
        LaserOn = false;
        if(CurSelected != null)
        {
            spectator.NewMove();
            spectator.SetTarget(CurSelected.transform);
        }
    }

    public float ctrVal;

    void Update()
    {
        ctrVal = ctrlr.GetTriggerAxis();
        if (ctrVal < 0.1f && LaserOn)
            CancelLaser();
        else if (ctrVal >= 0.1f && ctrVal < 1 && !LaserOn)
            StartLaser();
        line.SetColors(Color.red, Color.red);
        if (LaserOn)
        {
            Ray r = new Ray(LaserGuide.position, LaserGuide.forward);
            RaycastHit hit;
            Vector3 EndPoint = LaserGuide.position + (LaserGuide.forward * 150);
            if (Physics.Raycast(r, out hit, 150))
            {
                Teleporter t = hit.collider.gameObject.GetComponent<Teleporter>();
                if (t != null)
                {
                    CurSelected = t;
                    line.SetColors(Color.green, Color.green);
                }
                line.SetPositions(new Vector3[] { LaserGuide.position, hit.point });
            }
            else
            {
                CurSelected = null;
                line.SetPositions(new Vector3[] {LaserGuide.position, EndPoint });
            }
                
        }
        else
        {
            line.SetPositions(new Vector3[] {Vector3.zero, Vector3.zero });
            CurSelected = null;
        }
            
    }
}
