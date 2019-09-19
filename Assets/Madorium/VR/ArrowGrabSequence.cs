using UnityEngine;


public static class TransformEx
{
    public static void Copy(Transform from, Transform to)
    {
        to.position = from.position;
        to.rotation = from.rotation;
    }
}

public class ArrowGrabSequence : MonoBehaviour
{
    enum Stage
    {
        Idle,
        GrabWait,
        Releasing
    }

    public float DrawTimeMax = 1;
    public float ReleaseTime = 1;
    Stage _stage = Stage.Idle;
    PadControl _padControl;
    float _timer = 0;

    void Start()
    {
        _padControl = GetComponent<PadControl>();
    }

    public void OnMoved()
    {
        if(_stage == Stage.Idle)
        {
            _padControl.LeftController.transform.position = _padControl.LeftIdleLocator.transform.position;
            _padControl.LeftController.transform.rotation = _padControl.LeftIdleLocator.transform.rotation;
            _padControl.LeftController.SetActive(true);

            _padControl.RightController.transform.position = _padControl.RightIdleLocator.transform.position;
            _padControl.RightController.transform.rotation = _padControl.RightIdleLocator.transform.rotation;
            _padControl.RightController.SetActive(true);
        }
    }

    void Update()
    {
        var grabRight = PS4InputEx.GrabRight();
        switch (_stage)
        {
            case Stage.Idle:
                if (grabRight > 0)
                {
                    _stage = Stage.GrabWait;
                    _timer = 0.5f;
                    TransformEx.Copy(_padControl.QuiverLocator.transform, _padControl.RightController.transform);
                    _padControl.GrabKey.Pressed = true;
                }
                break;
            case Stage.GrabWait:
                _timer += Time.deltaTime;
                if (_timer > DrawTimeMax)
                    _timer = DrawTimeMax;
                {
                    float t = _timer / DrawTimeMax;

                    TransformEx.Copy(_padControl.BowDraw.transform, _padControl.LeftController.transform);
                    _padControl.RightController.transform.position = Vector3.Lerp(_padControl.DrawStartLocator.transform.position, _padControl.DrawEndLocator.transform.position, t);
                    _padControl.RightController.transform.rotation = Quaternion.Lerp(_padControl.DrawStartLocator.transform.rotation, _padControl.DrawEndLocator.transform.rotation, t);                    
                }
                if (grabRight == 0)
                {
                    _stage = Stage.Releasing;
                    _padControl.GrabKey.Pressed = false;
                    _timer = ReleaseTime;
                }
                break;
            case Stage.Releasing:
                _timer += Time.deltaTime;
                if(_timer>ReleaseTime)
                {
                    _timer = 0;
                    _stage = Stage.Idle;
                }
                break;
        }
    }
}