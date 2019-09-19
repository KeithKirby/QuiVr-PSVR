using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "SkyDB", menuName = "SkyColors", order = 1)]
public class SkyValues : ScriptableObject
{
    public Material SkyMat;
    public List<SkyColors> Skies;
    public int ApplyIndex;

    [AdvancedInspector.Inspect]
    public void Apply()
    {
        if(ApplyIndex >= 0 && ApplyIndex < Skies.Count)
        {
            RenderSettings.fogColor = Skies[ApplyIndex].FogColor;
            RenderSettings.skybox.SetColor("_TopColor", Skies[ApplyIndex].TopColor);
            RenderSettings.skybox.SetColor("_MiddleColor", Skies[ApplyIndex].FogColor);
            RenderSettings.sun.color = Skies[ApplyIndex].SunColor;
            RenderSettings.ambientSkyColor = Skies[ApplyIndex].AmbientColor;
        }
    }

    public List<SkyColors> GetColors(EnvironmentType type)
    {
        List<SkyColors> colors = new List<SkyColors>();
        foreach(var v in Skies)
        {
            if (v.Environment == type)
                colors.Add(v);
        }
        return colors;
    }

    [AdvancedInspector.Inspect]
    public void Save()
    {
        SkyColors c = new SkyColors();
        c.Name = "New Color";
        c.FogColor = RenderSettings.fogColor;
        c.TopColor = RenderSettings.skybox.GetColor("_TopColor");
        c.BottomColor = RenderSettings.skybox.GetColor("_MiddleColor");
        c.SunColor = RenderSettings.sun.color;
        c.AmbientColor = RenderSettings.ambientSkyColor;
        Skies.Add(c);
        //Save
    }

}

[System.Serializable]
public class SkyColors
{
    public string Name;
    public Color TopColor;
    public Color BottomColor;
    public Color FogColor;
    public Color AmbientColor;
    public Color SunColor;

    public EnvironmentType Environment;

    public override string ToString()
    {
        return Name;
    }

    [AdvancedInspector.Inspect]
    public void OverwriteColorsWithCurrent()
    {
        FogColor = RenderSettings.fogColor;
        TopColor = RenderSettings.skybox.GetColor("_TopColor");
        BottomColor = RenderSettings.skybox.GetColor("_MiddleColor");
        AmbientColor = RenderSettings.ambientSkyColor;
        SunColor = RenderSettings.sun.color;
    }
}