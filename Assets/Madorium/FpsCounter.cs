using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    [System.Serializable]
    public struct FPSInfo
    {
        public float MinFps;
        public Color Col;
    }

    public float UpdateInverval = 0.25f;

    public FPSInfo NormalFPS;
    public FPSInfo MediumFPS;
    public FPSInfo BadFPS;

    public GameObject FpsCounterHolder;

    Text fpsCounterText;
    float deltaTime = 0.0f;
    float t = 0;

    FPSInfo[] fpsInfoHolders;

#if ENABLE_CHEATS
    bool fpsCounterActive = true;
#else
    bool fpsCounterActive = false;
#endif

    private void Awake()
    {
        fpsInfoHolders = new FPSInfo[3]
        {
            NormalFPS,
            MediumFPS,
            BadFPS
        };

        t = UpdateInverval;
    }

    private void Start()
    {
        FpsCounterHolder.gameObject.SetActive(true);
        fpsCounterText = FpsCounterHolder.GetComponentInChildren<Text>();
        FpsCounterHolder.SetActive(true == fpsCounterActive);
    }

#if ENABLE_CHEATS
    private Color GetFPSColor(float fps)
    {
        int holderIndex = 0;
        for (int i = 0; i < fpsInfoHolders.Length; ++i)
            if (fps < fpsInfoHolders[i].MinFps)
                holderIndex = i;

        return fpsInfoHolders[holderIndex].Col;
    }

    void DrawFPSStats()
    {
        float dt = deltaTime;
        float fps = 1.0f / dt;

        string text = string.Format("{0:0.} fps", (int)fps);
        fpsCounterText.text = text;
        fpsCounterText.color = GetFPSColor(fps);
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        t -= Time.unscaledDeltaTime;
        if (t <= 0)
        {
            t = UpdateInverval;
            DrawFPSStats();
        }
    }
#endif
}