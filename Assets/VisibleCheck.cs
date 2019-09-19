//#define TEST_LINES
//#define TEST_SPHERES

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VisibleCheck : MonoBehaviour {

    public Color VisibleColor;
    public Color HiddenColor;
    public BoxCollider VisibleObject;
    public float SafeLeft = -27;
    public float SafeRight = 27;
    public float SafeUp = 17;
    public float SafeDown = -22;

    Vector3[] _corners = new Vector3[4];
    bool[] _valid = new bool[4];

    public UnityEvent OnLook;
    public UnityEvent OnNotLook;

    bool _wasLooking = false;

#if TEST_SPHERES
    GameObject[] _spheres = new GameObject[4];
    Material[] _sphereMat = new Material[4];
#endif

#if TEST_LINES
    GameObject[] _lines = new GameObject[4];
#endif

    Camera _mainCam;

    // Use this for initialization
    IEnumerator Start()
    {
        _corners[0] = VisibleObject.bounds.min;
        _corners[1] = new Vector3(VisibleObject.bounds.min.x, VisibleObject.bounds.max.y, VisibleObject.bounds.min.z);
        _corners[2] = new Vector3(VisibleObject.bounds.max.x, VisibleObject.bounds.min.y, VisibleObject.bounds.min.z);
        _corners[3] = VisibleObject.bounds.max;

        for (int i = 0; i < 4; ++i)
        {
            _valid[i] = false;
        }

#if TEST_LINES
        // Create test lines
        for (int i = 0; i < 4; ++i)
        {
            var line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            line.transform.localScale = new Vector3(0.001f, 3.0f, 0.001f);
            line.layer = 10;

            var lineHolder = new GameObject();
            line.transform.parent = lineHolder.transform;
            line.transform.localPosition = new Vector3(0, 0.0f, 3.05f);
            line.transform.localRotation = Quaternion.Euler(90, 0, 0);
            _lines[i] = lineHolder;
        }
#endif

#if TEST_SPHERES
        // Create test spheres
        for (int i = 0; i < 4; ++i)
        {
            _spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _spheres[i].transform.position = _corners[i];
            _spheres[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);            
            _spheres[i].layer = 10;
            var mr = _spheres[i].GetComponent<MeshRenderer>();
            _sphereMat[i] = new Material(Shader.Find("Diffuse"));
            mr.material = _sphereMat[i];
        }
#endif

        while (null == _mainCam)
        {
            var psvrRig = GameObject.FindObjectOfType<CameraRigPSVR>();
            if (null != psvrRig)
                _mainCam = psvrRig.RigCamera;
            else
                yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    public bool IsLooking
    {
        get
        {
            bool allValid = true;
            for (int i = 0; i < 4; ++i)
            {
                if (!_valid[i])
                    allValid = false;
            }
            return allValid;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (null == _mainCam)
            return;

        var camFwd = _mainCam.transform.forward;
        var camRight = _mainCam.transform.right;
        var camUp = _mainCam.transform.up;
        var camPos = _mainCam.transform.position;

        for (int i = 0; i < 4; ++i)
        {
            var c = _corners[i];

            var toCorner = c - camPos;
            Vector3 fwdXY = Vector3.ProjectOnPlane(toCorner, camRight);
            Vector3 fwdXZ = Vector3.ProjectOnPlane(toCorner, camUp);

            float upDown = Vector3.SignedAngle(camFwd, fwdXY, -camRight);
            float leftRight = Vector3.SignedAngle(camFwd, fwdXZ, camUp);

            bool ok = true;
            if (leftRight < SafeLeft)
            {
                ok = false;
            }
            else if (leftRight > SafeRight)
            {
                ok = false;
            }
            else if (upDown > SafeUp)
            {
                ok = false;
            }
            else if (upDown < SafeDown)
            {
                ok = false;
            }
            else
            {
            }
            _valid[i] = ok;
        }

        if (IsLooking != _wasLooking)
        {
            _wasLooking = IsLooking;
            if (_wasLooking)
            {
                OnLook.Invoke();
            }
            else
            {
                OnNotLook.Invoke();
            }
        }

#if TEST_LINES
        // Lines
        for (int i = 0; i < 4; ++i)
        {
            var l = _lines[i];
            l.transform.rotation = Quaternion.identity;

            switch (i)
            {
                case 0:
                    l.transform.Rotate(new Vector3(0, SafeLeft, 0));
                    break;
                case 1:
                    l.transform.Rotate(new Vector3(0, SafeRight, 0));
                    break;
                case 2:
                    l.transform.Rotate(new Vector3(-SafeUp, 0, 0));
                    break;
                case 3:
                    l.transform.Rotate(new Vector3(-SafeDown, 0, 0));
                    break;
            }
            
            l.transform.position = Camera.main.transform.position;
        }
#endif

#if TEST_SPHERES
        // Draw a yellow sphere at the transform's position        
        for (int i = 0; i < 4; ++i)
        {
            if (_valid[i])
                _sphereMat[i].SetColor("_Color", new Color(0, 1, 0));
            else
                _sphereMat[i].SetColor("_Color", new Color(1, 0, 0));
        }
#endif
    }
}
