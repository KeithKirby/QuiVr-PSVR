using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPCanvas : MonoBehaviour {

    public GameObject PanelRef;
    GameObject PanelInstance;
    
    void Update()
    {
        if(PanelInstance == null && GameCanvas.instance != null)
        {
            CreatePanel();
        }
    }

    void CreatePanel()
    {
        PanelInstance = Instantiate(PanelRef, GameCanvas.instance.transform);
        PanelInstance.transform.localPosition = Vector3.zero;
        PanelInstance.transform.localEulerAngles = Vector3.zero;
        PanelInstance.transform.localScale = Vector3.one;
        PanelInstance.SetActive(true);
        GameCanvas.instance.motion = PanelInstance.GetComponent<EMOpenCloseMotion>();
        GamePanel old = GameCanvas.instance.gameObject.GetComponentInChildren<GamePanel>();
        if (old != null)
            Destroy(old.gameObject);
    }
}
