using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeautifulDissolves;

public class TutorialScroll : MonoBehaviour {

    public static TutorialScroll instance;

    Animation anim;
    public SkinnedMeshRenderer scroll;
    public float AnimSpeed = 1f;
    public AnimationCurve IllumPulse;

    Dissolve[] Dissolves;

    Material scrollMat;
    Texture2D curTexture;
    Texture2D curIllum;
    Color illumColor;

    [HideInInspector]
    public bool showing;

    void Awake()
    {
        anim = GetComponent<Animation>();
        instance = this;
        Dissolves = GetComponentsInChildren<Dissolve>();
        if (scroll != null)
        {
            scrollMat = scroll.material;
            illumColor = scrollMat.GetColor("_EmissionColor");
        }
    }

    public static void ShowScroll(Texture2D t=null)
    {
        if (instance != null)
            instance.Show();
    }

    public static void HideScroll()
    {
        if (instance != null)
            instance.Hide();
    }

    public static void ChangeImage(Texture2D t, Texture2D illum=null)
    {
        if (instance != null)
            instance.ChangeTexture(t, illum);
    }

    [AdvancedInspector.Inspect]
    public void Show()
    {
        Show(null, null);
    }

    public void Show(TutorialManager.ScrollDisplay.ScrollImage img)
    {
        Show(img.image, img.illum);
    }

    public void Show(Texture2D t, Texture2D illum)
    {
        showing = true;
        ChangeTexture(t, illum);
        StopAllCoroutines();
        StartCoroutine("DisplayOn");
    }

    float curT;

    IEnumerator DisplayOn()
    {
        foreach (var v in Dissolves)
        {
            v.TriggerReverseDissolve();
        }
        yield return new WaitForSeconds(1f);
        if (anim != null)
        {
            anim["Roll"].speed = AnimSpeed;
            anim["Roll"].time = curT;
            anim.Play();
            while (anim.isPlaying)
            {
                curT = anim["Roll"].time;
                yield return true;
            }
            curT = anim["Roll"].length;
        }
    }

    [AdvancedInspector.Inspect]
    public void Hide()
    {
        showing = false;
        StopAllCoroutines();
        StartCoroutine("DisplayOff");
    }

    IEnumerator DisplayOff()
    {
        if (anim != null)
        {
            anim["Roll"].speed = -AnimSpeed;
            anim["Roll"].time = curT;
            anim.Play();
            while (anim.isPlaying)
            {
                curT = anim["Roll"].time;
                yield return true;
            }
            curT = 0;
        }
        yield return new WaitForSeconds(0.25f);
        foreach (var v in Dissolves)
        {
            v.TriggerDissolve();
        }
    }

    public float t = 0;
    void Update()
    {
        if(showing && scrollMat != null && curIllum != null)
        {
            t += Time.unscaledDeltaTime/2;
            if (t >= 1)
                t = 0;
            scrollMat.SetColor("_EmissionColor", illumColor * IllumPulse.Evaluate(t));
        }
    }

    public void ChangeTexture(Texture2D t, Texture2D illum=null)
    {
        if(t != null && curTexture != t && scrollMat != null)
        {
            scrollMat.mainTexture = t;
            curTexture = t;
        }
        if(illum != null && curIllum != illum && scrollMat != null && scrollMat.HasProperty("_EmissionMap"))
        {
            curIllum = illum;
            scrollMat.SetTexture("_EmissionMap", illum);
        }
        else if(illum == null && scrollMat != null && scrollMat.HasProperty("_EmissionMap"))
        {
            curIllum = null;
            scrollMat.SetTexture("_EmissionMap", null);
            scrollMat.SetColor("_EmissionColor", Color.black);
        }
    }
}
