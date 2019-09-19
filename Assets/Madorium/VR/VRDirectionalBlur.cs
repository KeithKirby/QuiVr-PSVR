using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

//[AddComponentMenu("Image Effects/Blur/Motion Blur (Color Accumulation)")]
[ExecuteInEditMode]    
[RequireComponent(typeof(Camera))]
public class VRDirectionalBlur : PostEffectsBase
{
    public enum Resolution
    {
        Low = 0,
        High = 1,
    }

    public enum BlurType
    {
        Standard = 0,
        Sgx = 1,
    }

    [Range(0.0f, 0.02f)]
    public float BlurAmount = 0.02f;
    
    public Shader shader = null;
    private Material material = null;

    public override bool CheckResources()
    {
        CheckSupport(false);

        material = CheckShaderAndCreateMaterial(shader, material);

        if (!isSupported)
            ReportAutoDisable();
        return isSupported;
    }

    void OnDisable()
    {
        if (material)
            DestroyImmediate(material);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CheckResources() == false)
        {
            Graphics.Blit(source, destination);
            return;
        }

        //fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, 0.0f, threshold, intensity));
        material.SetTexture("_MainTex", source);
        material.SetFloat("_BlurAmount", BlurAmount);
        Graphics.Blit(source, destination, material, 0);
    }
}