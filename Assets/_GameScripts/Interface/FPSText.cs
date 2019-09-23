using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class FPSText : MonoBehaviour {

    Text txt;
    public ToggleMenu menu;

    static FPSText instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        txt = GetComponent<Text>();
    }

    float cd = 0f;
    float cooldown = 0.1f;
    void Update() {
        cd += Time.unscaledDeltaTime;
        val += Time.unscaledDeltaTime;
        num++;
        if ((menu == null || menu.isOpen()) && txt != null && cd > cooldown)
        {
            cd = 0;
            float val = (int)(1f / (Time.deltaTime/Time.timeScale));
            txt.text = val + " fps";
            Color c = Color.white;
            if (val < 80)
                c = Color.yellow;
            if (val < 35)
                c = Color.red;

            if (txt.color != c)
                txt.color = c;
        }
	}

    [AdvancedInspector.Inspect]
    void LogFPS()
    {
        LogAvgFPS();
    }

    int num;
    float val;
    public static void LogAvgFPS()
    {
        if(instance != null)
        {
            int n = instance.num;
            float v = instance.val;
            v = v / n;
            v = 1f / v;
            float t = n / v;
            DebugTimed.Log("Average FPS (" + (int)t + " seconds): " + System.Math.Round(v + 0.5f, 2));
            instance.val = 0;
            instance.num = 1;
        }
    }
}
