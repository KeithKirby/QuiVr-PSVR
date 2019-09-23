//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using System.Collections;

namespace Vive.Plugin.SR
{
    [ExecuteInEditMode]
    public class ViveSR_DualCameraRig : MonoBehaviour
    {
        public Camera OriginalCamera;
        public Camera VirtualCamera;
        public Camera DualCameraLeft;
        public Camera DualCameraRight;
        public ViveSR_DualCameraImageRenderer DualCameraImageRenderer;
        public ViveSR_DualCameraCalibrationTool DualCameraCalibration;
        public ViveSR_TrackedCamera TrackedCameraLeft;
        public ViveSR_TrackedCamera TrackedCameraRight;
        public ViveSR_HMDCameraShifter HMDCameraShifter;

        public DualCameraDisplayMode Mode = DualCameraDisplayMode.MIX;
        public static DualCameraStatus DualCameraStatus { get; private set; }
        public static Matrix4x4[] EyeToHeadTransform { get; private set; }

        private ViveSR_DualCameraRig() { }
        private static ViveSR_DualCameraRig Mgr = null;
        public static ViveSR_DualCameraRig Instance
        {
            get
            {
                if (Mgr == null)
                {
                    Mgr = FindObjectOfType<ViveSR_DualCameraRig>();
                }
                if (Mgr == null)
                {
                    Debug.LogError("ViveSR_DualCameraManager does not be attached on GameObject");
                }
                return Mgr;
            }
        }

        private void Awake()
        {
            #if UNITY_EDITOR
            if (Application.isEditor) ViveSR_Settings.Update();
            #endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                SetMode(DualCameraDisplayMode.MIX);
            else if (Input.GetKeyDown(KeyCode.F2))
                SetMode(DualCameraDisplayMode.VIRTUAL);
            else if (Input.GetKeyDown(KeyCode.F3))
                SetMode(DualCameraDisplayMode.REAL);
        }

        public void Initial()
        {
            DualCameraStatus = DualCameraStatus.IDLE;
            if (ViveSR.FrameworkStatus == FrameworkStatus.WORKING)
                StartCoroutine(InitialCoroutine());
        }

        public void Release()
        {
            DualCameraStatus = DualCameraStatus.IDLE;
            if (DualCameraCalibration != null)
                DualCameraCalibration.SaveDeviceParameter();
        }

        IEnumerator InitialCoroutine()
        {
            if (VirtualCamera == null || DualCameraLeft == null || DualCameraRight == null || DualCameraImageRenderer == null)
            {
                Debug.LogError("[ViveSR] Please check references");
                yield break;
            }
            if (ViveSR_DualCameraImageCapature.Initial() != (int)Error.WORK) yield break;
            ViveSR_DualCameraDepthExtra.InitialDepthCollider(ViveSR_DualCameraImageCapature.DepthImageWidth,
                                                                     ViveSR_DualCameraImageCapature.DepthImageHeight);

            if (DualCameraCalibration != null)
            {
                DualCameraCalibration.LoadDeviceParameter();
                TrackedCameraLeft.ImagePlane.Initial();
                TrackedCameraLeft.ImagePlaneCalibration.Initial();
                TrackedCameraRight.ImagePlane.Initial();
                TrackedCameraRight.ImagePlaneCalibration.Initial();
            }
            TrackedCameraRight.ImagePlaneCalibration.gameObject.SetActive(false);
            TrackedCameraLeft.ImagePlaneCalibration.gameObject.SetActive(false);

            DualCameraStatus = DualCameraStatus.WORKING;
            SetMode(Mode);
        }

        /// <summary>
        /// Decide whether real/virtual camera render or not.
        /// </summary>
        /// <param name="mode">Virtual, Real and Mix</param>
        public void SetMode(DualCameraDisplayMode mode)
        {
            if (OriginalCamera == null)
            {
                if (Camera.main == VirtualCamera) VirtualCamera.tag = "Untagged";
                OriginalCamera = Camera.main;
                VirtualCamera.tag = "MainCamera";
            }
            switch (mode)
            {
                case DualCameraDisplayMode.VIRTUAL:
                    if(OriginalCamera != VirtualCamera && OriginalCamera != null) OriginalCamera.enabled = true;
                    EnableViveCamera(false);
                    break;
                case DualCameraDisplayMode.REAL:
                    if (OriginalCamera != VirtualCamera && OriginalCamera != null) OriginalCamera.enabled = false;
                    EnableViveCamera(true, DualCameraMode.REAL);
                    break;
                case DualCameraDisplayMode.MIX:
                    if (OriginalCamera != VirtualCamera && OriginalCamera != null) OriginalCamera.enabled = false;
                    EnableViveCamera(true, DualCameraMode.MIX);
                    break;
            }
        }

        private void EnableViveCamera(bool active, DualCameraMode mode = DualCameraMode.MIX)
        {
            DualCameraImageRenderer.enabled = active;
            VirtualCamera.gameObject.SetActive(mode == DualCameraMode.MIX ? active : false);
            DualCameraLeft.gameObject.SetActive(active);
            DualCameraRight.gameObject.SetActive(active);
            TrackedCameraLeft.gameObject.SetActive(active);
            TrackedCameraRight.gameObject.SetActive(active);
        }
    }
}