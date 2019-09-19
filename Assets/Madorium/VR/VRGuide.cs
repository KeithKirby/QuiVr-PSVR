using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRGuide : MonoBehaviour {

    public Text Txt;
    public MeshRenderer[] Meshes;
    public float ChangeRate = 1;
    bool _show = false;

    public bool Show
    {
        set
        {
            _show = value;
        }
        get
        {
            return _show;
        }
    }

    public void Update()
    {
        if(_show)
        {
            if (_alpha < 0.7)
            {
                var newAlpha = Mathf.Clamp01(_alpha + Time.deltaTime * ChangeRate);
                Alpha = newAlpha;
                
            }
        }
        else
        {
            if (_alpha > 0)
            {
                var newAlpha = Mathf.Clamp01(_alpha - Time.deltaTime * ChangeRate);
                Alpha = newAlpha;
            }
        }
    }

    float Alpha
    {
        set
        {
            if(_alpha != value)
            {
                _alpha = value;
                if (null != Txt)
                {
                    var col = Txt.color;
                    col.a = Mathf.Sqrt(value);
                    Txt.color = col;
                }
                foreach(var m in Meshes)
                {
                    var meshCol = m.materials[0].color;
                    meshCol.a = _alpha;
                    m.materials[0].color = meshCol;
                }
            }
        }
    }
    float _alpha = 0.001f;
}