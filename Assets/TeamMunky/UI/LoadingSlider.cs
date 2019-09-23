using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSlider : MonoBehaviour {

    Slider _slider;
    AppModel _appModel;
    //LoadingController _loadingController;

	// Use this for initialization
	void Start ()
    {
        _slider = GetComponent<Slider>();
        _appModel = FindObjectOfType<AppModel>();
        //_loadingController = FindObjectOfType<LoadingController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //_slider.value = _loadingController.LoadProgress;
    }
}
