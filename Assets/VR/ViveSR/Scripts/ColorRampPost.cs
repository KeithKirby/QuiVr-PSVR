using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColorRampPost : MonoBehaviour
{

    private Material mat;
    public Texture2D ColorRamp;

    void Awake()
    {
        mat = new Material(Shader.Find("Custom/Ramp shader"));
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        mat.SetTexture("_Ramp", ColorRamp);
        Graphics.Blit(source, destination, mat);
    }
}
