using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Subtitles : MonoBehaviour {

    private static List<Subtitles> instances = new List<Subtitles>();

    public bool ActiveOnStart = true;

    Text display;
    [TextArea]
    public string TestString;
    
    public bool SubtitlesActive { get { return display.enabled; } }

    void RegisterMe()
    {
        instances.Add(this);
    }

    void Awake()
    {
        RegisterMe();
        display = GetComponent<Text>();

        if (!ActiveOnStart)
            TurnOff();
    }

    public static void Show(string text, float duration=5f)
    {
        foreach (var instance in instances)
        {
            if (instance != null && instance.SubtitlesActive)
                instance.ShowText(text, duration);
        }
    }

    [AdvancedInspector.Inspect]
    public void TestShow()
    {
        ShowText(TestString, 5f);
    }

    public void TurnOff()
    {
        display.enabled = false;
    }

    public void TurnOn()
    {
        display.enabled = true;
    }

    float CurDur;
    public void ShowText(string text, float duration)
    {
        StopAllCoroutines();
        CurDur = duration;
        StartCoroutine("TextSequence", text);
    }

    IEnumerator TextSequence(string text)
    {
        display.text = text;
        Color c = display.color;
        while(c.a < 1)
        {
            c.a += Time.deltaTime * 2f;
            display.color = c;
            yield return true;
        }
        yield return new WaitForSeconds(CurDur);
        while (c.a > 0)
        {
            c.a -= Time.deltaTime * 2f;
            display.color = c;
            yield return true;
        }
        c.a = 0;
        display.color = c;
    }
}
