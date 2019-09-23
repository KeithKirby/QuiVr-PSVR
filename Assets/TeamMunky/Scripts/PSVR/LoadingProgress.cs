using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgress : MonoBehaviour
{
    public static float Progress = 0;

    Slider _slider;

    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    void Update()
    {
        _slider.value = Progress;
    }
}
