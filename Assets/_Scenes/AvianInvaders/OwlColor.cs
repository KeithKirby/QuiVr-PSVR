using UnityEngine;
using System.Collections;

public class OwlColor : MonoBehaviour {

    public SkinnedMeshRenderer OwlRenderer;
    public Color color = Color.white;

    void Start()
    {
        foreach(var v in OwlRenderer.materials)
        {
            v.color = color;
        }
    }
}
