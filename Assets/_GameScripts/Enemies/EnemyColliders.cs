using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyColliders : MonoBehaviour {

    List<EnemyCollider> Cols;

    void Awake()
    {
        Cols = new List<EnemyCollider>();
        foreach(var v in GetComponentsInChildren<Collider>())
        {
            Cols.Add(new EnemyCollider(v));
        }
    }

    public void SetColliderScale(EnemyState s)
    {
        if (Cols == null)
            return;
        if (s == EnemyState.attacking)
            ChangeScale(1.25f);
        else
            ResetScale();
    }

    public void ChangeScale(float val)
    {
        foreach(var v in Cols)
        {
            v.ChangeScale(val);
        }
    }

    public void ResetScale()
    {
        foreach(var v in Cols)
        {
            v.ResetScale();
        }
    }
}

[System.Serializable]
public class EnemyCollider
{
    public Collider c;
    public Vector3 BoxSize;
    public Vector2 CapsuleVals;
    public float SphereRadius;

    public EnemyCollider (Collider col)
    {
        c = col;
        if (c.GetType() == typeof(BoxCollider))
            BoxSize = ((BoxCollider)c).size;
        else if (c.GetType() == typeof(CapsuleCollider))
            CapsuleVals = new Vector2(((CapsuleCollider)c).radius, ((CapsuleCollider)c).height);
        else if (c.GetType() == typeof(SphereCollider))
            SphereRadius = ((SphereCollider)c).radius;
    }

    public void ChangeScale(float val)
    {
        if (c == null)
            return;
        if (c.GetType() == typeof(BoxCollider))
            ((BoxCollider)c).size = BoxSize*val;
        else if (c.GetType() == typeof(CapsuleCollider))
        {
            ((CapsuleCollider)c).radius = CapsuleVals.x * val;
            ((CapsuleCollider)c).height = CapsuleVals.y * val;
        }
        else if (c.GetType() == typeof(SphereCollider))
            ((SphereCollider)c).radius = SphereRadius*val;
    }

    public void ResetScale()
    {
        if (c == null)
            return;
        if (c.GetType() == typeof(BoxCollider))
            ((BoxCollider)c).size = BoxSize;
        else if (c.GetType() == typeof(CapsuleCollider))
        {
            ((CapsuleCollider)c).radius = CapsuleVals.x;
            ((CapsuleCollider)c).height = CapsuleVals.y;
        }
        else if (c.GetType() == typeof(SphereCollider))
            ((SphereCollider)c).radius = SphereRadius;
    }
}
