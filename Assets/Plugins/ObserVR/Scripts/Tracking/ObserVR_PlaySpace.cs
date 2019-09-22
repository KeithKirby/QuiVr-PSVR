using System.Collections;
using UnityEngine;
using ObserVR;

[DisallowMultipleComponent]
public class ObserVR_PlaySpace : MonoBehaviour {

#if OBSERVR_SVR
    private static Valve.VR.HmdQuad_t rect;
#endif

    IEnumerator Start() {
        yield return new WaitUntil(() => ObserVR_User.Instance != null && !string.IsNullOrEmpty(ObserVR_Session.Instance.sessionID));
#if OBSERVR_SVR && OBSERVR_OVR
        if (FindObjectOfType<SteamVR_PlayArea>() != null) {
            SteamVR_PlayArea.GetBounds(FindObjectOfType<SteamVR_PlayArea>().size, ref rect);
            ObserVR_Events.CustomEvent("PLAYSPACE_SIZE")
                .AddParameter("length", Mathf.Abs(rect.vCorners0.v0 - rect.vCorners2.v0))
                .AddParameter("width", Mathf.Abs(rect.vCorners0.v2 - rect.vCorners2.v2))
                .EndParameters();
        }
        if (OVRManager.instance != null) {
            ObserVR_Events.CustomEvent("PLAYSPACE_SIZE")
                .AddParameter("length", OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea).x)
                .AddParameter("width", OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea).z)
                .EndParameters();
        }
#elif OBSERVR_SVR
        if (FindObjectOfType<SteamVR_PlayArea>() != null) {
            SteamVR_PlayArea.GetBounds(FindObjectOfType<SteamVR_PlayArea>().size, ref rect);
            ObserVR_Events.CustomEvent("PLAYSPACE_SIZE")
                .AddParameter("length", Mathf.Abs(rect.vCorners0.v0 - rect.vCorners2.v0))
                .AddParameter("width", Mathf.Abs(rect.vCorners0.v2 - rect.vCorners2.v2))
                .EndParameters();
        }
#elif OBSERVR_OVR
        ObserVR_Events.CustomEvent("PLAYSPACE_SIZE")
            .AddParameter("length", OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea).x)
            .AddParameter("width", OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea).z)
            .EndParameters();
#endif
    }
}
