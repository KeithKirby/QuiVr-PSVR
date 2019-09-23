using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatText : MonoBehaviour {

    public static CombatText instance;
    public DisplayText[] Displays;
    int curIndex;
    public float TextDuration;
    public AnimationCurve MoveCurve;
    public AnimationCurve FadeCurve;

    void Awake()
    {
        instance = this;
        curIndex = 0;
        foreach(var v in Displays)
        {
            v.Setup();
        }
    }

    public static void ShowText(string val, Vector3 pos, Color c, float upPerc = 0f, float sidePerc=0f)
    {
        if(instance && Settings.GetBool("CombatText"))
        {
            instance.ShowTxt(val, pos, c, upPerc, sidePerc);
        }     
    }

    void ShowTxt(string val, Vector3 pos, Color c, float upPerc, float sidePerc)
    {
        if(curIndex < Displays.Length)
        {
            Displays[curIndex].SetText(val);
            Displays[curIndex].SetPos(pos, upPerc, sidePerc);
            Displays[curIndex].SetColor(c);
            /*
            Displays[curIndex].Motion.SetStateToOpen();
            Displays[curIndex].Motion.Close();
            */
            StartCoroutine("DoDisplay", curIndex);
        }
        NextIndex();
    }

    void NextIndex()
    {
        curIndex++;
        if (curIndex >= Displays.Length)
            curIndex = 0;
    }

    IEnumerator DoDisplay(int idx)
    {
        yield return true;
        Displays[idx].look.visible = true;
        Transform t = Displays[idx].obj;
        CanvasGroup cg = Displays[idx].obj.GetComponent<CanvasGroup>();
        Vector3 p = t.position;
        float x = 0;
        while (x < 1)
        {
            x += Time.deltaTime/TextDuration;
            t.position = p + Vector3.up * MoveCurve.Evaluate(x);
            cg.alpha = FadeCurve.Evaluate(x);
            yield return true;
        }
        Displays[idx].look.visible = false;
    }

    [System.Serializable]
    public class DisplayText
    {
        public Transform obj;
        [HideInInspector]
        public EMOpenCloseMotion Motion;
        [HideInInspector]
        public Text EnergyText;
        [HideInInspector]
        public LookAt look;

        float startScale;
        bool isSetup;

        public void Setup()
        {
            startScale = obj.transform.localScale.x;
            Motion = obj.GetComponent<EMOpenCloseMotion>();
            EnergyText = obj.GetComponent<Text>();
            look = obj.GetComponent<LookAt>();
            isSetup = true;
        }

        public void SetPos(Vector3 pos, float upPerc, float sidePerc)
        {
            if (!isSetup)
                Setup();
            float scaleMult = Vector3.Distance(pos, PlayerHead.instance.transform.position) / 30f;
            if(EnergyText != null)
            {
                EnergyText.transform.position = pos + (Vector3.up * upPerc * scaleMult) + (Vector3.left * scaleMult * sidePerc);
                EnergyText.transform.localScale = startScale * Vector3.one * scaleMult;
            }
        }

        public void SetColor(Color c)
        {
            EnergyText.color = c;
        }

        public void SetText(string txt)
        {
            EnergyText.text = txt;
        }

        public override string ToString()
        {
            if (obj != null)
                return obj.name;
            return "None";
        }
    }
}
