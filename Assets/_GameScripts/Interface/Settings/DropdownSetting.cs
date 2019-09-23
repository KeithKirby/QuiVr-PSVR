using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class DropdownSetting : MonoBehaviour
{

    Dropdown input;
    public string valueKey;
    public int defaultValue;

    void Start()
    {

        int correctVal = defaultValue;
        if (Settings.HasKey(valueKey))
        {
            correctVal = Settings.GetInt(valueKey);
        }
        else
        {
            SetValue(defaultValue);
        }
        input = GetComponent<Dropdown>();
        input.value = correctVal;
        input.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(SetValue));
    }

    public void SetValue(int val)
    {
        Settings.Set(valueKey, val);
    }
}
