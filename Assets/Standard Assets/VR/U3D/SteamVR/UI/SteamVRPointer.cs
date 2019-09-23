#if DEBUG
#define LOG_CONSOLE_SteamVRPointer
#endif
using System;
using System.Collections.Generic;
using System.Text;
using U3D.SteamVR.UI.Watchers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace U3D.SteamVR.UI
{
    [RequireComponent(typeof(EventSystem))]
    public class SteamVRPointer : MonoBehaviourBase
    {
        #region log stuff
        [System.Diagnostics.Conditional("LOG_CONSOLE_SteamVRPointer")]
        void Log(string format, params object[] args)
        {
            Debug.LogFormat(this.GetType().Name + "\t" + format, args);
        }
        #endregion

        #region Laser and Hit
        public SteamVR_TrackedObject trackedObject;
        public GameObject PointerObject;
        public LineRenderer laserPrefab;
        LineRenderer m_laser;
        public Transform hitPrefab;
        Transform m_hit;

        public bool menuUp;

        public static List<SteamVRPointer> ptrs;

        void Awake()
        {
            if (ptrs == null)
                ptrs = new List<SteamVRPointer>();
            ptrs.Add(this);
        }

        void OnDestroy()
        {
            ptrs.Remove(this);
        }

        public static SteamVRPointer[] AllPointers()
        {
            if (ptrs != null)
                return ptrs.ToArray();
            return new SteamVRPointer[] { };
        }

        void CreateLaserAndHit()
        {
            if (m_laser == null && laserPrefab != null)
            {
                m_laser = Instantiate(laserPrefab);
                m_laser.name = trackedObject.name + " - laser";
                m_laser.gameObject.SetActive(false);
                m_laser.transform.SetParent(m_trackedObjectTransform, false);
            }
            if (m_hit == null && hitPrefab != null)
            {
                m_hit = Instantiate(hitPrefab);
                m_hit.name = trackedObject.name + " - hit";
                m_hit.gameObject.SetActive(false);
                m_hit.transform.SetParent(this.transform, false);
            }
        }

        public void SetEnabled(bool val)
        {
            menuUp = val;
            if (!menuUp && m_laser != null)
            {
                m_laser.gameObject.SetActive(false);
                m_pointerData = null;
            }
            else if(menuUp)
            {
                Process();
            }
        }

        void DrawLaserAndHit()
        {
            if (m_pointerData != null && m_pointerData.pointerCurrentRaycast.gameObject != null)
            {
                if (m_laser != null)
                {
                    m_laser.gameObject.SetActive(true);
                    //m_laser.transform.position = m_trackedObjectTransform.position;
                    //m_laser.transform.LookAt(m_trackedObjectTransform.forward + m_trackedObjectTransform.position);
                    m_laser.SetPositions(new Vector3[] { Vector3.zero, Vector3.forward * m_pointerData.pointerCurrentRaycast.distance });
                    //m_laser.SetPositions(new Vector3[] { Vector3.zero, m_laser.transform.InverseTransformVector(m_pointerData.pointerCurrentRaycast.worldPosition) });
                }
                if (m_hit != null)
                {
                    m_hit.gameObject.SetActive(true);
                    m_hit.position = m_pointerData.pointerCurrentRaycast.worldPosition;
                }
            }
            else
            {
                if (m_laser != null)
                {
                    m_laser.gameObject.SetActive(false);
                }
                if (m_hit != null)
                {
                    m_hit.gameObject.SetActive(false);
                }
            }
        }

        void DestroyLaserAndHit()
        {
            if (m_laser != null)
            {
                m_laser.gameObject.SetActive(false);
                Destroy(m_laser.gameObject);
                m_laser = null;
            }
            if (m_hit != null)
            {
                m_hit.gameObject.SetActive(false);
                Destroy(m_hit.gameObject);
                m_hit = null;
            }
        }
        #endregion

        System.Collections.IEnumerator Start()
        {
            while (!trackedObject.gameObject.activeSelf)
            {
                yield return true;
            }
            m_trackedObjectTransform = PointerObject.transform;
            CreateLaserAndHit();
        }

        EventSystem __eventSystem;
        EventSystem m_eventSystem { get { return __eventSystem ?? (__eventSystem = GetComponent<EventSystem>()); } }
        Transform m_trackedObjectTransform;
        SteamVR_Controller.Device m_device
        {
            get
            {
                if (trackedObject == null)
                    return null;
                if (trackedObject.index == SteamVR_TrackedObject.EIndex.None)
                    return null;
                if (!trackedObject.isValid)
                    return null;
                return SteamVR_Controller.Input((int)trackedObject.index);
            }
        }

        PointerEventData m_pointerData = null;
        public interface IWatcher
        {
            void Process(PointerEventData pointerData, List<RaycastResult> raycastResults, SteamVR_Controller.Device device);
        }
        IWatcher[] __watchers = null;
        IWatcher[] m_watchers
        {
            get
            {
                if(__watchers==null)
                {
                    __watchers = new IWatcher[] { new EnterExit(), new DownUpClick(), new Drag(), new SelectDeselect(GetSelectedObject, NewObjectSelected), new Scroll(ShowWheel) };
                }
                return __watchers;
            }
        }

        public string state
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (m_pointerData != null)
                {
                    sb.AppendFormat("\nentered: {0}", m_pointerData.pointerEnter == null ? "none" : m_pointerData.pointerEnter.name);
                    sb.AppendFormat("\npressed: {0}", m_pointerData.pointerPress == null ? "none" : m_pointerData.pointerPress.name);
                    sb.AppendFormat("\ndragged: {0}", m_pointerData.pointerDrag == null ? "none" : m_pointerData.pointerDrag.name);
                    sb.AppendFormat("\nselected: {0}", m_eventSystem.currentSelectedGameObject == null ? "none" : m_eventSystem.currentSelectedGameObject.name);
                }
                return sb.ToString();
            }
        }


        //SteamVR_RenderModel __renderModel;
        //SteamVR_RenderModel m_renderModel { get { return __renderModel ?? (__renderModel = m_trackedObjectTransform.GetComponentInChildren<SteamVR_RenderModel>()); } }

        public float scrollMultiplier = 1;
        public void Process()
        {
            if (m_device == null)
            {
                this.enabled = false;
                return;
            }
            this.enabled = true;

            if (!menuUp)
                return;

            //Valve.VR.VRControllerState_t s = m_device.GetState();
            //Debug.LogFormat("{0},{1} - {2},{3} - {4},{5} - {6},{7} - {8},{9}", s.rAxis0.x, s.rAxis0.y, s.rAxis1.x, s.rAxis1.y, s.rAxis2.x, s.rAxis2.y, s.rAxis3.x, s.rAxis3.y, s.rAxis4.x, s.rAxis4.y);
            //Debug.LogFormat("{0}", m_device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger));

            if (m_pointerData == null)
            {
                m_pointerData = new PointerEventData(m_eventSystem);
                m_pointerData.pointerId = (int)m_device.index + 220;
            }
            Vector2 scroll = m_device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) * scrollMultiplier;
            scroll.x *= -1; // x is inverted WTF
            m_pointerData.scrollDelta = scroll;

            List<RaycastResult> raycastResults = RaycastAll(ref m_pointerData);
            foreach (IWatcher i in m_watchers)
            {
                i.Process(m_pointerData, raycastResults, m_device);
            }
            DrawLaserAndHit();
        }

        private void ShowWheel(bool obj)
        {
            //m_renderModel.controllerModeState.bScrollWheelVisible = obj;
        }

        private GameObject GetSelectedObject()
        {
            return m_eventSystem.currentSelectedGameObject;
        }

        private void NewObjectSelected(GameObject obj)
        {
            m_eventSystem.SetSelectedGameObject(obj);
        }

        List<RaycastResult> RaycastAll(ref PointerEventData pointerData)
        {
            List<RaycastResult> raycastResult = new List<RaycastResult>();
            if (m_trackedObjectTransform != null)
            {
                pointerData.pointerCurrentRaycast = new RaycastResult()
                {
                    worldPosition = m_trackedObjectTransform.position,
                    worldNormal = m_trackedObjectTransform.forward
                };
                m_eventSystem.RaycastAll(pointerData, raycastResult);
            }
            return raycastResult;
        }
    }
}