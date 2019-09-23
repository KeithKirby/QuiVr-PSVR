using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CooldownDisplay : MonoBehaviour {

    bool completed;
    Image img;

    public Color displayColor;

    public bool lockPosition;

    void Awake()
    {
        img = GetComponent<Image>();
        img.color = displayColor;
    }

    void Update()
    {
        if(SteamVR_ControllerManager.freeHand != null && !lockPosition)
        {
            CDRoot root = SteamVR_ControllerManager.freeHand.GetComponentInChildren<CDRoot>();
            if(root != null)
            {
                transform.position = root.transform.position;
                transform.rotation = root.transform.rotation;
            }
        }
    }

	public void SetValue(float v)
    {
        if(v < 1)
        {
            StopCoroutine("PulseSequence");
            img.color = displayColor;
            completed = false;
        }  
        if(v >= 1 && !completed)
        {
            completed = true;
            pulse();
        }
        img.fillAmount = v;
    }

    void pulse()
    {
        StopCoroutine("PulseSequence");
        StartCoroutine("PulseSequence");
    }

    IEnumerator PulseSequence()
    {
        img.color = displayColor;
        float t = 0;
        while(img.color.a < 1)
        {
            Color c = img.color;
            c.a = Mathf.Lerp(displayColor.a, 1f, t);
            img.color = c;
            t += 2 * Time.deltaTime;
            yield return true;
        }
        t = 0;
        while (img.color.a > displayColor.a)
        {
            Color c = img.color;
            c.a = Mathf.Max(Mathf.Lerp(1f, displayColor.a, t), displayColor.a);
            img.color = c;
            t += 2 * Time.deltaTime;
            yield return true;
        }
        img.color = displayColor;
        StopCoroutine("PulseSequence");
    }
}
