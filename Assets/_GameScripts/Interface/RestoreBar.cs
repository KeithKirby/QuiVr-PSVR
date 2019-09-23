using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestoreBar : MonoBehaviour {

    public Image ProgressBar;
    Gate g;
    Canvas canvas;
    LookAt look;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        look = GetComponent<LookAt>();
        g = GetComponentInParent<Gate>();
        canvas.enabled = false;
        look.visible = false;
        if (g != null)
            InvokeRepeating("CheckRestore", 1f, Random.Range(1.273f, 1.98751f));
    }

    void CheckRestore()
    {
        if (g.isDestroyed() && g.RevAcum > 0)
        {
            canvas.enabled = true;
            ProgressBar.fillAmount = (float)g.RevAcum / (float)g.CalculateReviveReq();
            look.visible = false;
        }
        else
        {
            canvas.enabled = false;
            look.visible = true;
        }
    }

}
