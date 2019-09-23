using UnityEngine;

namespace Vive.Plugin.SR
{
    public class ViveSR : MonoBehaviour
    {
        public static FrameworkStatus FrameworkStatus { get; private set; }
        public bool EnableAutomatically;
        [SerializeField] private ViveSR_DualCameraRig DualCameraRig;
        [SerializeField] private ViveSR_RigidReconstructionRenderer RigidReconstruction;

        private ViveSR() { }
        private static ViveSR Mgr = null;
        public static ViveSR Instance
        {
            get
            {
                if (Mgr == null)
                {
                    Mgr = FindObjectOfType<ViveSR>();
                }
                if (Mgr == null)
                {
                    Debug.LogError("ViveSR does not be attached on GameObject");
                }
                return Mgr;
            }
        }

        // Use this for initialization
        void Start()
        {
            FrameworkStatus = FrameworkStatus.STOP;
            if (EnableAutomatically)
            {
                StartFramework();
            }
        }


        // Update is called once per frame
        void OnDestroy()
        {
            StopFramework();
        }

        private void StartFramework()
        {
            if (FrameworkStatus == FrameworkStatus.WORKING) return;
            // Before initialize framework
            int result = -1;
            do
            {
                if (DualCameraRig != null)
                {
                    if (ViveSR_DualCameraImageCapature.CheckNecessayFile()) result = 0;
                    else break;
                }
            } while (false);

            if (result == (int)Error.WORK) result = ViveSR_Framework.InitialFramework();
            if (RigidReconstruction != null) RigidReconstruction.InitRigidReconstructionParam();
            Debug.Log("[ViveSR] Initial Framework : " + result);

            // start framework
            if (result == (int)Error.WORK) result = ViveSR_Framework.StartFramework();
            FrameworkStatus = result == (int)Error.WORK ? FrameworkStatus.WORKING : FrameworkStatus.ERROR;
            Debug.Log("[ViveSR] Start Framework : " + result);

            if (FrameworkStatus == FrameworkStatus.WORKING)
            {
                if (DualCameraRig != null) {
                    DualCameraRig.gameObject.SetActive(true);
                    DualCameraRig.Initial();
                }
                if (RigidReconstruction != null) RigidReconstruction.gameObject.SetActive(true);
            }
        }

        private void StopFramework()
        {
            if (DualCameraRig != null)
            {
                DualCameraRig.Release();
                DualCameraRig.gameObject.SetActive(false);
            }
            if (RigidReconstruction != null) RigidReconstruction.gameObject.SetActive(false);

            if (FrameworkStatus == FrameworkStatus.WORKING)
            {
                int result = ViveSR_Framework.StopFramework();
                FrameworkStatus = result == 0 ? FrameworkStatus.STOP : FrameworkStatus.ERROR;
                Debug.Log("[ViveSR] Stop Framework : " + result);
            }
            else
            {
                Debug.Log("[ViveSR] Stop Framework : not open");
            }
        }
    }
}