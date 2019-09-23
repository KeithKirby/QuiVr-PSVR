using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ThrowBomb : MonoBehaviour, IPointerDownHandler
{
    public float Cooldown;
    public Camera c;
    float cd;
    MobilePlayerSync ps;

    //Projectile Equation
    public float baseForce = 8;
    float force;
    Vector3 target;
    int iterations = 0;

    void Awake()
    {
        ps = GetComponentInParent<MobilePlayerSync>();
    }

    void Update()
    {
        if (cd >= 0)
            cd -= Time.deltaTime;
    }

    RaycastHit hit;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (cd <= 0)
        {
            cd = Cooldown;
            Vector2 pos = eventData.position;
            Ray r = c.ScreenPointToRay(pos);
            Vector3 worldPos = c.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 1f));
            Vector3 vel = Vector3.zero;
            Debug.DrawRay(worldPos, c.transform.forward*100, Color.blue, 1);
            if(Physics.Raycast(r, out hit, 100))
            {
                target = hit.point;
                float yRotation = GetYRotation(c.transform);
                force = baseForce;
                float barrelAngle = CalculateProjectileFiringSolution(worldPos, force, (target.y - worldPos.y));
                Vector3 fVector = new Vector3(force, 0, 0); // fixed vector (image a 2d trajectory graph)      
                Vector3 rVector = Quaternion.Euler(0, yRotation, barrelAngle) * fVector; // transformed to 3d to point at target
                vel = rVector;
                iterations = 0;
            }
            ps.ThrowOrb(worldPos, vel);
        }
    }

    float CalculateProjectileFiringSolution(Vector3 StartPos, float vel, float alt)
    {
        float g = Mathf.Abs(Physics.gravity.y);

        Vector2 a = new Vector2(StartPos.x, StartPos.z);
        Vector2 b = new Vector2(target.x, target.z);
        float dis = Vector2.Distance(a, b);

        float dis2 = dis * dis;
        float vel2 = vel * vel;
        float vel4 = vel * vel * vel * vel;
        float calc = (vel4 - g * ((g * dis2) + (2 * alt * vel2)));
        if (calc <= 0 && iterations < 100)
        {
            force *= 1.1f;
            iterations++;
            return CalculateProjectileFiringSolution(StartPos, force, alt);
        }
        float num = vel2 + Mathf.Sqrt(calc);
        float dom = g * dis;
        float angle = Mathf.Atan(num / dom);

        return angle * Mathf.Rad2Deg;
    }

    float GetYRotation(Transform t)
    {
        Vector3 relativePos = t.InverseTransformPoint(target);
        float x = (relativePos.x);
        float z = (relativePos.z);

        float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;

        return angle + c.transform.eulerAngles.y - 90;
    }
}
