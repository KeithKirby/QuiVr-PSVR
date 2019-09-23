using Microsoft.Win32;
using UnityEngine;

namespace Vive.Plugin.SR
{
    public class ViveSR_DualCameraCalibrationTool : MonoBehaviour
    {
        public static bool IsCalibrating;
        public static CalibrationType CurrentCalibrationType;

        private Vector3 RelativeAngle;
        private Vector3 AbsoluteAngle;

        private string rootPaht = "HKEY_CURRENT_USER\\Vive SRWorks";
        private string keyNameRelativeAngle = "RelativeAngle";
        private string keyNameAbsoluteAngle = "AbsoluteAngle";

        public void SetCalibrationMode(bool active, CalibrationType calibrationType = CalibrationType.ABSOLUTE)
        {
            CurrentCalibrationType = calibrationType;
            IsCalibrating = active;
            if (IsCalibrating)
            {
                ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.gameObject.SetActive(calibrationType == CalibrationType.RELATIVE);
            }
            else
            {
                ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.gameObject.SetActive(false);
            }
        }

        public void Calibration(CalibrationAxis axis, float angle)
        {
            Vector3 vectorAxis = Vector3.zero;
            switch (axis)
            {
                case CalibrationAxis.X:
                    vectorAxis = Vector3.right;
                    break;
                case CalibrationAxis.Y:
                    vectorAxis = Vector3.up;
                    break;
                case CalibrationAxis.Z:
                    vectorAxis = Vector3.forward;
                    break;
            }
            if (CurrentCalibrationType == CalibrationType.RELATIVE)
            {
                ViveSR_DualCameraRig.Instance.TrackedCameraLeft.Anchor.transform.localEulerAngles += vectorAxis * angle;
                RelativeAngle += vectorAxis * angle;
            }
            if (CurrentCalibrationType == CalibrationType.ABSOLUTE)
            {
                ViveSR_DualCameraRig.Instance.TrackedCameraLeft.Anchor.transform.localEulerAngles += vectorAxis * angle;
                ViveSR_DualCameraRig.Instance.TrackedCameraRight.Anchor.transform.localEulerAngles += vectorAxis * angle;
                AbsoluteAngle += vectorAxis * angle;
            }
        }

        public void ResetCalibration()
        {
            CurrentCalibrationType = CalibrationType.RELATIVE;
            Calibration(CalibrationAxis.X, -RelativeAngle.x);
            Calibration(CalibrationAxis.Y, -RelativeAngle.y);
            Calibration(CalibrationAxis.Z, -RelativeAngle.z);

            CurrentCalibrationType = CalibrationType.ABSOLUTE;
            Calibration(CalibrationAxis.X, -AbsoluteAngle.x);
            Calibration(CalibrationAxis.Y, -AbsoluteAngle.y);
            Calibration(CalibrationAxis.Z, -AbsoluteAngle.z);
        }

        /// <summary>
        /// Load the custom calibration parameters from  DualCameraParameters.xml.
        /// </summary>
        public void LoadDeviceParameter()
        {
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.DistortedImageWidth = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.DistortedImageWidth = ViveSR_DualCameraImageCapature.DistortedImageWidth;
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.DistortedImageHeight = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.DistortedImageHeight = ViveSR_DualCameraImageCapature.DistortedImageHeight;
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.UndistortedImageWidth = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.UndistortedImageWidth = ViveSR_DualCameraImageCapature.UndistortedImageWidth;
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.UndistortedImageHeight = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.UndistortedImageHeight = ViveSR_DualCameraImageCapature.UndistortedImageHeight;
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.DistortedCx = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.DistortedCx = ViveSR_DualCameraImageCapature.DistortedCx_L;
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.DistortedCy = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.DistortedCy = ViveSR_DualCameraImageCapature.DistortedCy_L;
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.UndistortedCx = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.UndistortedCx = ViveSR_DualCameraImageCapature.UndistortedCx_L;
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.UndistortedCy = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.UndistortedCy = ViveSR_DualCameraImageCapature.UndistortedCy_L;
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.FocalLength = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.FocalLength = ViveSR_DualCameraImageCapature.FocalLength_L;
            ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlane.UndistortionMap = ViveSR_DualCameraRig.Instance.TrackedCameraLeft.ImagePlaneCalibration.UndistortionMap = ViveSR_DualCameraImageCapature.UndistortionMap_L;
            Vector3 relativeAngle = new Vector3(float.Parse((string)Registry.GetValue(rootPaht, keyNameRelativeAngle + "_x", 0)),
                                                float.Parse((string)Registry.GetValue(rootPaht, keyNameRelativeAngle + "_y", 0)),
                                                float.Parse((string)Registry.GetValue(rootPaht, keyNameRelativeAngle + "_z", 0)));

            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.DistortedImageWidth = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.DistortedImageWidth = ViveSR_DualCameraImageCapature.DistortedImageWidth;
            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.DistortedImageHeight = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.DistortedImageHeight = ViveSR_DualCameraImageCapature.DistortedImageHeight;
            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.UndistortedImageWidth = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.UndistortedImageWidth = ViveSR_DualCameraImageCapature.UndistortedImageWidth;
            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.UndistortedImageHeight = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.UndistortedImageHeight = ViveSR_DualCameraImageCapature.UndistortedImageHeight;
            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.DistortedCx = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.DistortedCx = ViveSR_DualCameraImageCapature.DistortedCx_R;
            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.DistortedCy = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.DistortedCy = ViveSR_DualCameraImageCapature.DistortedCy_R;
            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.UndistortedCx = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.UndistortedCx = ViveSR_DualCameraImageCapature.UndistortedCx_R;
            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.UndistortedCy = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.UndistortedCy = ViveSR_DualCameraImageCapature.UndistortedCy_R;
            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.FocalLength = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.FocalLength = ViveSR_DualCameraImageCapature.FocalLength_R;
            ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlane.UndistortionMap = ViveSR_DualCameraRig.Instance.TrackedCameraRight.ImagePlaneCalibration.UndistortionMap = ViveSR_DualCameraImageCapature.UndistortionMap_R;
            Vector3 absoluteAngle = new Vector3(float.Parse((string)Registry.GetValue(rootPaht, keyNameAbsoluteAngle + "_x", 0)),
                                                float.Parse((string)Registry.GetValue(rootPaht, keyNameAbsoluteAngle + "_y", 0)),
                                                float.Parse((string)Registry.GetValue(rootPaht, keyNameAbsoluteAngle + "_z", 0)));

            CurrentCalibrationType = CalibrationType.RELATIVE;
            Calibration(CalibrationAxis.X, relativeAngle.x);
            Calibration(CalibrationAxis.Y, relativeAngle.y);
            Calibration(CalibrationAxis.Z, relativeAngle.z);

            CurrentCalibrationType = CalibrationType.ABSOLUTE;
            Calibration(CalibrationAxis.X, absoluteAngle.x);
            Calibration(CalibrationAxis.Y, absoluteAngle.y);
            Calibration(CalibrationAxis.Z, absoluteAngle.z);
        }

        /// <summary>
        /// Save the custom calibration parameters. 
        /// </summary>
        public void SaveDeviceParameter()
        {
            Registry.SetValue(rootPaht, keyNameRelativeAngle + "_x", RelativeAngle.x.ToString());
            Registry.SetValue(rootPaht, keyNameRelativeAngle + "_y", RelativeAngle.y.ToString());
            Registry.SetValue(rootPaht, keyNameRelativeAngle + "_z", RelativeAngle.z.ToString());

            Registry.SetValue(rootPaht, keyNameAbsoluteAngle + "_x", AbsoluteAngle.x.ToString());
            Registry.SetValue(rootPaht, keyNameAbsoluteAngle + "_y", AbsoluteAngle.y.ToString());
            Registry.SetValue(rootPaht, keyNameAbsoluteAngle + "_z", AbsoluteAngle.z.ToString());
        }
    }
}