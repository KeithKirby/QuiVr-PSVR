using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Toggle))]
public class ToggleSetting : MonoBehaviour {

    Toggle input;
    public string valueKey;
    public bool defaultValue;

	void Start () {

        bool correctVal = defaultValue;
        if(Settings.HasKey(valueKey))
        {
            correctVal = Settings.GetBool(valueKey);
        }
        else
        {
            SetValue(defaultValue);
        }   
        input = GetComponent<Toggle>();
        input.isOn = correctVal;
        input.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(SetValue));
	}
	
	public void SetValue(bool val)
    {
        Settings.Set(valueKey, val);
    }
}
