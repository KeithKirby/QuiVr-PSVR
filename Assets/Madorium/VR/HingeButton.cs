using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class HingeButton : MonoBehaviour {

    public enum ActivateLimitType
    {
        Min,
        Max
    }

    public ActivateLimitType ActivateLimit;
    public UnityEvent ActivationEvent;

    public float Threshold = 3.0f;

    public AudioClip GrabSound;
    public AudioClip CloseSound;

    AudioSource _source;
    HingeJoint _hinge;
    bool _pressed = false;

	// Use this for initialization
	void Start () {
        _hinge = GetComponent<HingeJoint>();
        _source = GetComponent<AudioSource>();
        var interactable = GetComponent<VRTK_InteractableObject>();
        interactable.InteractableObjectGrabbed += Interactable_InteractableObjectGrabbed;
    }

    private void Interactable_InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (GrabSound != null)
        {
            _source.clip = GrabSound;
            _source.Play();
        }
    }

    // Update is called once per frame
    void Update () { 
        float limitEnd = ActivateLimit == ActivateLimitType.Min ? _hinge.limits.min : _hinge.limits.max;
        bool isPressed = Mathf.Abs(_hinge.angle - limitEnd) < Threshold;
        if(isPressed != _pressed)
        {
            _pressed = isPressed;
            if (_pressed)
            {
                if (CloseSound != null)
                {
                    _source.clip = CloseSound;
                    _source.Play();
                }
                ActivationEvent.Invoke();
            }
        }
    }
}
