using UnityEngine;

public class PSVRWarningScreen : MonoBehaviour
{
    public VisibleCheck VisCheck;
    public GameObject ContinueButton;
    public GameObject LookHere;

    bool _isLooking = false;

    private void Start()
    {
        ContinueButton.SetActive(false);
        LookHere.SetActive(true);
    }

    private void Update()
    {
        if(VisCheck.IsLooking != _isLooking)
        {
            _isLooking = VisCheck.IsLooking;
            ContinueButton.SetActive(_isLooking);
            LookHere.SetActive(!_isLooking);
        }
    }
}