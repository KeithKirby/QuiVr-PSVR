namespace VRTK
{
    using UnityEngine;
#if UNITY_PS4
    using UnityEngine.PS4;
#endif

    public class VRTK_SDK_Bridge
    {
        private static SDK_Base activeSDK = null;

        public static string GetControllerElementPath(SDK_Base.ControllerElelements element, VRTK_DeviceFinder.ControllerHand hand = VRTK_DeviceFinder.ControllerHand.Right)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetControllerElementPath(element, hand);
#endif
            return "";
        }

        public static GameObject GetTrackedObject(GameObject obj, out uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetTrackedObject(obj, out index);
#endif
            index = 0;
            return null;
        }

        public static GameObject GetTrackedObjectByIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetTrackedObjectByIndex(index);
#endif
            return null;
        }

        public static uint GetIndexOfTrackedObject(GameObject trackedObject)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetIndexOfTrackedObject(trackedObject);
#endif
            return 0;
        }

        public static Transform GetTrackedObjectOrigin(GameObject obj)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetTrackedObjectOrigin(obj);
#endif
            return null;
        }

        public static bool TrackedIndexIsController(uint index)
        {

#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().TrackedIndexIsController(index);
#endif
            return false;
        }

        public static GameObject GetControllerLeftHand()
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetControllerLeftHand();
#endif
            return null;
        }

        public static GameObject GetControllerRightHand()
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetControllerRightHand();
#endif
            return null;
        }

        public static bool IsControllerLeftHand(GameObject controller)
        {
#if UNITY_PS4
            var ce = controller.GetComponent<VRTK_ControllerEvents>();
            if (null != ce)
            {
                return ce.Hand == MotionControllerInput.Handed.Left;
            }
#elif UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsControllerLeftHand(controller);
#endif

            return false;
        }

        public static bool IsControllerRightHand(GameObject controller)
        {
#if UNITY_PS4
            var ce = controller.GetComponent<VRTK_ControllerEvents>();
            if (null != ce)
            {
                return ce.Hand == MotionControllerInput.Handed.Right;
            }
#elif UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsControllerRightHand(controller);
#endif
            return false;
        }

        public static Transform GetHeadset()
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetHeadset();
#endif
            return null;
        }

        public static Transform GetHeadsetCamera()
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetHeadsetCamera();
#endif
            return null;
        }

        public static GameObject GetHeadsetCamera(GameObject obj)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetHeadsetCamera(obj);
#endif
            return null;
        }

        public static Transform GetPlayArea()
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetPlayArea();
#endif
            return null;
        }

        public static Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetPlayAreaVertices(playArea);
#endif
            return null;
        }

        public static float GetPlayAreaBorderThickness(GameObject playArea)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetPlayAreaBorderThickness(playArea);
#endif
            return 0;
        }

        public static bool IsPlayAreaSizeCalibrated(GameObject playArea)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsPlayAreaSizeCalibrated(playArea);
#endif
            return false;
        }

        public static bool IsDisplayOnDesktop()
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsDisplayOnDesktop();
#endif
            return false;
        }

        public static bool ShouldAppRenderWithLowResources()
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().ShouldAppRenderWithLowResources();
#endif
            return false;
        }

        public static void ForceInterleavedReprojectionOn(bool force)
        {
#if UNITY_STANDALONE || UNITY_WSA
            GetActiveSDK().ForceInterleavedReprojectionOn(force);
#endif
        }

        public static GameObject GetControllerRenderModel(GameObject controller)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetControllerRenderModel(controller);
#endif
            return null;
        }

        public static void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
#if UNITY_STANDALONE || UNITY_WSA
            GetActiveSDK().SetControllerRenderModelWheel(renderModel, state);
#endif
        }

        public static void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
#if UNITY_STANDALONE || UNITY_WSA
            GetActiveSDK().HeadsetFade(color, duration, fadeOverlay);
#endif
        }

        public static bool HasHeadsetFade(GameObject obj)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().HasHeadsetFade(obj);
#endif
            return false;
        }

        public static void AddHeadsetFade(Transform camera)
        {
#if UNITY_STANDALONE || UNITY_WSA
            GetActiveSDK().AddHeadsetFade(camera);
#endif
        }

        public static void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500)
        {
#if UNITY_STANDALONE || UNITY_WSA
            GetActiveSDK().HapticPulseOnIndex(index, durationMicroSec);
#endif
        }

        public static Vector3 GetVelocityOnIndex(uint index)
        {
#if UNITY_PS4
            var moveState = PS4InputEx.GetMove((int)index);
            return moveState.Velocity;
#elif UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetVelocityOnIndex(index);
#endif
            return Vector3.zero;
        }

        public static Vector3 GetAngularVelocityOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetAngularVelocityOnIndex(index);
#endif
            return Vector3.zero;
        }

        public static Vector2 GetTouchpadAxisOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetTouchpadAxisOnIndex(index);
#endif
            return Vector2.zero;
        }

        public static Vector2 GetTriggerAxisOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetTriggerAxisOnIndex(index);
#endif
            return Vector2.zero;
        }

        public static float GetTriggerHairlineDeltaOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().GetTriggerHairlineDeltaOnIndex(index);
#endif
            return 0;
        }

        //Trigger

        public static bool IsTriggerPressedOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTriggerPressedOnIndex(index);
#endif
            return false;
        }

        public static bool IsTriggerPressedDownOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTriggerPressedDownOnIndex(index);
#endif
            return false;
        }

        public static bool IsTriggerPressedUpOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTriggerPressedUpOnIndex(index);
#endif
            return false;
        }

        public static bool IsTriggerTouchedOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTriggerTouchedOnIndex(index);
#endif
            return false;
        }

        public static bool IsTriggerTouchedDownOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTriggerTouchedDownOnIndex(index);
#endif
            return false;
        }

        public static bool IsTriggerTouchedUpOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTriggerTouchedUpOnIndex(index);
#endif
            return false;
        }

        public static bool IsHairTriggerDownOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsHairTriggerDownOnIndex(index);
#endif
            return false;
        }

        public static bool IsHairTriggerUpOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsHairTriggerUpOnIndex(index);
#endif
            return false;
        }

        //Grip

        public static bool IsGripPressedOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsGripPressedOnIndex(index);
#endif
            return false;
        }

        public static bool IsGripPressedDownOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsGripPressedDownOnIndex(index);
#endif
            return false;
        }

        public static bool IsGripPressedUpOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsGripPressedUpOnIndex(index);
#endif
            return false;
        }

        public static bool IsGripTouchedOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsGripTouchedOnIndex(index);
#endif
            return false;
        }

        public static bool IsGripTouchedDownOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsGripTouchedDownOnIndex(index);
#endif
            return false;
        }

        public static bool IsGripTouchedUpOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsGripTouchedUpOnIndex(index);
#endif
            return false;
        }

        //Touchpad

        public static bool IsTouchpadPressedOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTouchpadPressedOnIndex(index);
#endif
            return false;
        }

        public static bool IsTouchpadPressedDownOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTouchpadPressedDownOnIndex(index);
#endif
            return false;
        }

        public static bool IsTouchpadPressedUpOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTouchpadPressedUpOnIndex(index);
#endif
            return false;
        }

        public static bool IsTouchpadTouchedOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTouchpadTouchedOnIndex(index);
#endif
            return false;
        }

        public static bool IsTouchpadTouchedDownOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTouchpadTouchedDownOnIndex(index);
#endif
            return false;
        }

        public static bool IsTouchpadTouchedUpOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsTouchpadTouchedUpOnIndex(index);
#endif
            return false;
        }

        //Application Menu

        public static bool IsApplicationMenuPressedOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsApplicationMenuPressedOnIndex(index);
#endif
            return false;
        }

        public static bool IsApplicationMenuPressedDownOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsApplicationMenuPressedDownOnIndex(index);
#endif
            return false;
        }

        public static bool IsApplicationMenuPressedUpOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsApplicationMenuPressedUpOnIndex(index);
#endif
            return false;
        }

        public static bool IsApplicationMenuTouchedOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsApplicationMenuTouchedOnIndex(index);
#endif
            return false;
        }

        public static bool IsApplicationMenuTouchedDownOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsApplicationMenuTouchedDownOnIndex(index);
#endif
            return false;
        }

        public static bool IsApplicationMenuTouchedUpOnIndex(uint index)
        {
#if UNITY_STANDALONE || UNITY_WSA
            return GetActiveSDK().IsApplicationMenuTouchedUpOnIndex(index);
#endif
            return false;
        }

        private static SDK_Base GetActiveSDK()
        {
            if (activeSDK == null)
            {
#if UNITY_PS4
                throw new System.Exception("GetActiveSDK not supported");
#else
                activeSDK = ScriptableObject.CreateInstance<SDK_SteamVR>();
#endif
            }

            return activeSDK;
        }
    }
}