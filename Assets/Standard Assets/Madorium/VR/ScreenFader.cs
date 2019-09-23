using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Fades this sprite according to the current alpha of the FadeController
public class ScreenFader : MonoBehaviour
{    
    SpriteRenderer _sprite;
    RenderMode _fadeController;
    float _alpha = 0;

    float FaderScale = 1.4f;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _fadeController = RenderMode.GetInst();
        if(null == _fadeController)
            throw new System.Exception("ScreenFader Could not find fade controller!");
        Alpha = _fadeController.Alpha;

        
        transform.localScale = transform.localScale * FaderScale;

    }

    float Alpha
    {
        set
        {
            if (_alpha != value)
            {
                _alpha = value;
                _sprite.color = new Color(1, 1, 1, _alpha);
            }
        }
    }

    public void Update()
    {
        Alpha = _fadeController.Alpha;
    }
}
