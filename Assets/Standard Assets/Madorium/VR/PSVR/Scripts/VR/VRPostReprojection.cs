using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4.VR;
#endif

public class VRPostReprojection : MonoBehaviour
{
    public Camera PostReprojectionCam;

#if UNITY_PS4
    private int m_CurrentEye = 0;
    private RenderTexture m_PostReprojectionTexture;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
#if UNITY_EDITOR
        // In editor this renders in front of game cam, on PS4 this is a texture so is cleared with a solid color
        cam.clearFlags = CameraClearFlags.Nothing;
        cam.depth = 0;
#endif
    }

    void Update()
    {
        // Reset which eye we're adjusting at the start of every frame
        m_CurrentEye = 0;
	}

    void OnPostRender()
    {
        if (UnityEngine.XR.XRSettings.loadedDeviceName == VRDeviceNames.PlayStationVR)
        {
            if (PlayStationVRSettings.postReprojectionType == PlayStationVRPostReprojectionType.None)
            {
                DisablePostReprojection();
            }
            else
            {
                m_PostReprojectionTexture = PlayStationVR.GetCurrentFramePostReprojectionEyeTexture(m_CurrentEye == 0 ? UnityEngine.XR.XRNode.LeftEye : UnityEngine.XR.XRNode.RightEye);

                if (RenderTexture.active.antiAliasing > 1)
                {
                    RenderTexture.active.ResolveAntiAliasedSurface(m_PostReprojectionTexture);
                }
                else
                {
                    Graphics.Blit(RenderTexture.active, m_PostReprojectionTexture);
                }

                m_CurrentEye++;
            }
        }
    }

    void DisablePostReprojection()
    {
        // If post-reprojection isn't supported (either because it wasn't turned on, or else we're in
        // Deferred) then disable this script and re-parent the reticle to the main camera instead
        Debug.LogError("You're trying to use Post Reprojection, but it is not enabled in your PlayStationVRSettings!");

        if (transform.childCount > 0)
        {
            var reticle = transform.GetChild(0);
            reticle.gameObject.layer = 0;
            reticle.parent = Camera.main.transform;
        }

        gameObject.SetActive(false);
    }
#endif
    }
