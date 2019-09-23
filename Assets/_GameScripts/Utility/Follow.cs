using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

    public Transform obj;
    public bool isCamera;
    public float moveLimit;
    public bool keepInitialOffset;
    public bool useUpdate;
    public bool followOrientation;

    Vector3 lastPos;

    Vector3 offset;
    void Awake()
    {
        if(obj != null)
            offset = transform.position - obj.position;
    }

	void FixedUpdate()
    {
        if(!useUpdate)
        {
            if (isCamera && (obj == null || !obj.gameObject.activeInHierarchy))
            {
                if (PlayerHead.instance != null)
                    obj = PlayerHead.instance.transform;
                else if(MobilePlayerSync.myInstance != null)
                {
                    obj = MobilePlayerSync.myInstance.Real.transform;
                }
            }
            if (obj != null && (moveLimit < 0.01f || Vector3.Distance(obj.position, lastPos) > moveLimit))
            {
                if (!followOrientation)
                {
                    if (!keepInitialOffset)
                        transform.position = obj.position;
                    else
                        transform.position = obj.position + offset;
                }
                else
                {
                    if (!keepInitialOffset)
                        transform.SetPositionAndRotation(obj.position, obj.rotation);
                    else
                        transform.SetPositionAndRotation(obj.position + offset, obj.rotation);
                }
                lastPos = transform.position;
            }
            else if (obj != null && followOrientation)
                transform.rotation = obj.rotation;
                
        }
    }

    void Update()
    {
        if (useUpdate)
        {
            if (isCamera && (obj == null || !obj.gameObject.activeInHierarchy))
            {
                foreach (var v in Camera.allCameras)
                {
                    if (v.GetComponent<SteamVR_Camera>() != null)
                        obj = v.gameObject.transform;
                }
            }
            if (obj != null && (moveLimit < 0.01f || Vector3.Distance(obj.position, lastPos) > moveLimit))
            {
                if (!keepInitialOffset)
                    transform.position = obj.position;
                else
                    transform.position = obj.position + offset;
                lastPos = transform.position;
            }
        }
    }
}
