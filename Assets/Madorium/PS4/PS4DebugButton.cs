using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PS4DebugButton : MonoBehaviour {

    public Color Normal;
    public Color Highlighted;
    public UnityEvent Press;
    public int Sort = 0;
    Text _text;

    public bool Selected
    {
        set
        {
            _selected = value;
            _text.color = _selected ? Highlighted : Normal;
        }
    }

    bool _selected = false;

	// Use this for initialization
	void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void DoPress()
    {
        Press.Invoke();
    }
}
