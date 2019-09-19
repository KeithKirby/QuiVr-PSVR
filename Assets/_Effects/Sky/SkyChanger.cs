using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyChanger : MonoBehaviour {

    SkyColors CurSky;
    public static SkyChanger instance;
    public Material SkyMat;
    Material instSky;
    public float ChangeDur;
    public SkyValues SkyDB;

    void Awake()
    {
        instance = this;
        if (RenderSettings.skybox != SkyMat)
            RenderSettings.skybox = SkyMat;
        instSky = new Material(SkyMat);
        RenderSettings.skybox = instSky;
        SkyMat = instSky;
        RenderSettings.fog = true;
    }

	public void ChangeSky(SkyColors sc)
    {
        CurSky = sc;
        StopCoroutine("Change");
        StartCoroutine("Change");
    }

    [AdvancedInspector.Inspect]
    public void RandomColor()
    {
        if (SkyDB != null)
            ChangeSky(SkyDB.Skies[Random.Range(0, SkyDB.Skies.Count)]);
    }

    public void SetColor(int val)
    {
        if (SkyDB != null && val < SkyDB.Skies.Count)
        {
            SkyDB.ApplyIndex = 0;
            SkyDB.Apply();
        }           
    }

    IEnumerator Change()
    {
        float t = 0;
        SkyColors old = new SkyColors();
        old.FogColor = RenderSettings.fogColor;
        old.TopColor = RenderSettings.skybox.GetColor("_TopColor");
        old.BottomColor = RenderSettings.skybox.GetColor("_MiddleColor");
        old.AmbientColor = RenderSettings.ambientSkyColor;
        old.SunColor = RenderSettings.sun.color;
        while (t < 1)
        {
            RenderSettings.fogColor = Color.Lerp(old.FogColor, CurSky.FogColor, t);
            RenderSettings.skybox.SetColor("_TopColor", Color.Lerp(old.TopColor, CurSky.TopColor, t));
            RenderSettings.skybox.SetColor("_MiddleColor", Color.Lerp(old.BottomColor, CurSky.BottomColor, t));
            RenderSettings.sun.color = Color.Lerp(old.SunColor, CurSky.SunColor, t);
            RenderSettings.ambientSkyColor = Color.Lerp(old.AmbientColor, CurSky.AmbientColor, t);
            t += Time.unscaledDeltaTime / ChangeDur;
            yield return true;
        }
    }
}
