// Physics - Swing Bone Component
// Ray@raymix.net

using UnityEngine;
using System.Collections.Generic;


[ExecuteInEditMode]
[AddComponentMenu("Physics/Swing Bone")]
public class SwingBone: MonoBehaviour
{
    private bool IsRoot = true;
    private Transform baseTransform;

    private Vector3 m_LastPos = Vector3.zero;
    private Quaternion m_LastRot = Quaternion.identity;

    private List<SwingBone> m_Bones = new List<SwingBone>();


    [Range(0.01f, 100.0f)]
    public float drag = 50;

    [Range(0.01f, 100.0f)]
    public float angelDrag = 50;

    [Range(0.01f, 100f)]
    public float lDrag = 5f;

    public bool affectChild = true;


    // Use this for initialization
    [ExecuteInEditMode]
    void Start()
    {
        baseTransform = transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            SwingBone bone = child.GetComponent<SwingBone>();
            if (bone == null)
            {
                bone = child.gameObject.AddComponent<SwingBone>();
                bone.drag = drag;
                bone.angelDrag = angelDrag;
            }
            bone.IsRoot = false;
            m_Bones.Add(bone);
        }
    }

    [ExecuteInEditMode]
    void LateUpdate()
    {
        if (IsRoot)
            UpdateBone();
    }

    void UpdateBone()
    {
        if (baseTransform == null)
            return;
        if (m_LastRot != Quaternion.identity)
        {
            baseTransform.rotation = Quaternion.Lerp(m_LastRot, baseTransform.rotation, Mathf.Clamp01(angelDrag * Time.unscaledDeltaTime));
        }
        Quaternion rot = baseTransform.rotation;
        if (m_LastRot != rot && !IsRoot)
        {
            Vector3 pos = baseTransform.position;
            Vector3 ppos = transform.parent.position;
            if (m_LastPos != Vector3.zero && m_LastPos != pos)
            {
                Vector3 a = pos - ppos;
                Vector3 b = m_LastPos - ppos;
                Vector3 v = a + (b - a) / 2;
                m_LastRot = Quaternion.Lerp(m_LastRot, Quaternion.FromToRotation(a, b) * rot, Mathf.Clamp01(lDrag * Time.unscaledDeltaTime));
                baseTransform.rotation = Quaternion.Lerp(m_LastRot, rot, Mathf.Clamp01(drag * Time.unscaledDeltaTime));
            }
            else
                m_LastRot = baseTransform.rotation;
            m_LastPos = pos;
        }

        for (int i = 0; i < m_Bones.Count; i++)
        {
            if (affectChild)
            {
                m_Bones[i].affectChild = affectChild;
                m_Bones[i].drag = drag;
                m_Bones[i].angelDrag = angelDrag;
                m_Bones[i].lDrag = lDrag;
            }
            m_Bones[i].UpdateBone();
        }
    }
}
