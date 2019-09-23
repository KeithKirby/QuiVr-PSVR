using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent (typeof(Slider))]
public class SliderSetting : MonoBehaviour {

    Slider s;
    public string valueKey;
    public float defaultValue;

    [System.Serializable]
    public class floatEvent : UnityEvent<float> { }

    [SerializeField]
    public floatEvent OnChange;

    // Use this for initialization
    IEnumerator Start () {
        s = GetComponent<Slider>();
        float correctVal = defaultValue;
        if (Settings.HasKey(valueKey))
        {
            correctVal = Settings.GetFloat(valueKey);
        }
        yield return true;
        s.onValueChanged.AddListener(new UnityAction<float>(setValue));
        s.value = correctVal;
    }

    public float getValue()
    {
        return s.value;
    }

    public void setValue(float val)
    {
        float actualVal = val;
        OnChange.Invoke(actualVal);
        Settings.Set(valueKey, actualVal);
    }
}
