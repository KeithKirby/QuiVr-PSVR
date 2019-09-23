using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class VRUIController : MonoBehaviour
{
    public LayerMask uiLayerMask;
    private float sliderFillSpeed = 0.75f;
    private RaycastHit hit;
    private Slider currentSlider;
    private AudioSource audioSrc;

    void Start()
    {
        // Audio source for UI interaction feedback
        audioSrc = GetComponent<AudioSource>();
    }

	void Update ()
    {
        // Continuously check what's in front of the camera
        if (Physics.Raycast(transform.position, transform.forward, out hit, Camera.main.farClipPlane, uiLayerMask))
        {
            // Check to make sure we haven't already got the current object
            if (EventSystem.current.currentSelectedGameObject != hit.collider.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(hit.collider.gameObject);

                if (EventSystem.current.currentSelectedGameObject.GetComponent<Slider>())
                    currentSlider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
                else
                    currentSlider = null;
            }
        }
        else
        {
            // For when there's no UI elements to look at
            EventSystem.current.SetSelectedGameObject(null);
            hit.distance = 0;

            // If we were just looking at a slider, make sure we reset it now we're looking away
            if (currentSlider && currentSlider.value != 1)
            {
                currentSlider.value = 0;
                currentSlider = null;
            }
        }

        // Handles filling in sliders
        if(currentSlider)
        {
            // WORKAROUND: Right now there's a problem where the Move controller isn't maintaining the 'down' state of buttons
            // when queried via GetButton. For now this workaround should allow normal functionality
#region PSMoveWorkaround
            // Loop through all 4 possible players, and both of their slots, to see if we have *any* move controller Cross buttons pressed
            bool hasMoveButton = false;
            for (int slot = 0; slot < 4; slot++)
            {
                for (int controller = 0; controller < 2; controller++)
                {
#if UNITY_PS4
                    if (PS4Input.MoveIsConnected(slot, controller))
                    {
                        if (PS4Input.MoveGetButtons(slot, controller) == 64)
                            hasMoveButton = true;
                    }
#endif
                }
            }
#endregion

            if ((Input.GetButton("Fire1") || hasMoveButton) && currentSlider && currentSlider.value != 1 && currentSlider.interactable)
            {
                currentSlider.value += Time.deltaTime * sliderFillSpeed;

                if (currentSlider.value == 1)
                {
                    audioSrc.Play();

                    if (currentSlider.gameObject.GetComponent<VRUIComplete>())
                        currentSlider.gameObject.GetComponent<VRUIComplete>().Complete();
                }
            }
            else if (Input.GetButtonUp("Fire1") && currentSlider.value != 1)
            {
                currentSlider.value = 0;
            }
        }
	}
}
