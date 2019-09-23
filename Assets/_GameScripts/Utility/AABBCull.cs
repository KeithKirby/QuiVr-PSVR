using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBCull : MonoBehaviour {

    Renderer r;
    Vector3[] points = new Vector3[8];

    public static int FramesForDisable = 80;
    public static LayerMask Mask = 1 | (1 << 26);

    public int LastFrame;
    public bool hidden;

    static int MAX_FRAMEDIFF = 6;
    static int id;
    int myID;

    /*
    void Start()
    {
        r = GetComponent<Renderer>();
        if (r == null)
            Destroy(this);

        points[0] = r.bounds.min;
        points[1] = r.bounds.max;
        points[2] = new Vector3(points[0].x, points[0].y, points[1].z);
        points[3] = new Vector3(points[0].x, points[1].y, points[0].z);
        points[4] = new Vector3(points[1].x, points[0].y, points[0].z);
        points[5] = new Vector3(points[0].x, points[1].y, points[1].z);
        points[6] = new Vector3(points[1].x, points[0].y, points[1].z);
        points[7] = new Vector3(points[1].x, points[1].y, points[0].z);

        id++;
        if (id > MAX_FRAMEDIFF)
            id = 0;
        myID = id;
    }

	void Update()
    {
        if (r.isVisible && PlayerHead.instance != null && Time.frameCount % MAX_FRAMEDIFF == myID)
        {
            Vector3 StPos = PlayerHead.instance.transform.position;
            Vector3 closest = r.bounds.ClosestPoint(StPos);
            if (!Physics.Linecast(closest, Vector3.MoveTowards(StPos, closest, 1), Mask))
            {
                Show();
                return;
            }

            if (!Physics.Linecast(r.bounds.center, Vector3.MoveTowards(StPos, r.bounds.center, 1), Mask))
            {
                Show();
                return;
            }

            foreach (var v in points)
            {
                if (!Physics.Linecast(v, Vector3.MoveTowards(StPos, v, 1), Mask))
                {
                    Show();
                    return;
                }
            }

            int Randoms = 3;
            for (int i = 0; i < Randoms; i++)
            {
                Vector3 rp = new Vector3(Random.Range(points[0].x, points[1].x), Random.Range(points[0].y, points[1].y), Random.Range(points[0].z, points[1].z));
                if (!Physics.Linecast(rp, Vector3.MoveTowards(StPos, rp, 1), Mask))
                {
                    Debug.DrawLine(rp, StPos);
                    Show();
                    return;
                }
            }

            if (!hidden && Time.frameCount > LastFrame)
                Hide();
        }
        else if (!r.isVisible)
            Hide();
        else if (PlayerHead.instance == null)
            Show();

    }
    */

    void Show()
    {
        LastFrame = Time.frameCount + FramesForDisable;
        if(hidden)
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        hidden = false;
    }

    void Hide()
    {
        hidden = true;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }
}
