#if DEBUG
//#define LOG_CONSOLE_VRGraphicRaycaster
#endif
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;

namespace U3D.SteamVR.UI
{
    //https://bitbucket.org/Unity-Technologies/ui/src/0155c39e05ca5d7dcc97d9974256ef83bc122586/UnityEngine.UI/UI/Core/GraphicRaycaster.cs?at=5.2&fileviewer=file-view-default
    [RequireComponent(typeof(Canvas))]
    public class SteamVRGraphicRaycaster : BaseRaycaster
    {
        #region log stuff
        [System.Diagnostics.Conditional("LOG_CONSOLE_VRGraphicRaycaster")]
        void Log(string format, params object[] args)
        {
            Debug.LogFormat(this.GetType().Name + "\t" + format, args);
        }

#if LOG_CONSOLE_VRGraphicRaycaster
        public float gizmoRadius = 0.01f;
        public void OnDrawGizmos()
        {
            for (int i = 0; i < m_RaycastResults.Count; ++i)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(m_RaycastResults[i].position, gizmoRadius);
            }
        }
#endif
#endregion

        Canvas __canvas;
        Canvas m_canvas { get { return __canvas ?? (__canvas = GetComponent<Canvas>()); } }

        Camera __eventCamera;
        public override Camera eventCamera
        {
            get
            {
                return Camera.main;//__eventCamera ?? (__eventCamera = (m_canvas.worldCamera != null ? m_canvas.worldCamera : Camera.main));
            }
        }

        [SerializeField]
        protected LayerMask m_BlockingMask = -1;

        [NonSerialized]
        readonly List<TGraphicRaycastResult> m_RaycastResults = new List<TGraphicRaycastResult>();
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (eventCamera == null) // Was throwing errors?
                return;
            float hitDistance = eventCamera.farClipPlane;

#if LOG_CONSOLE_VRGraphicRaycaster
            Debug.DrawRay(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerCurrentRaycast.worldNormal, Color.red);
#endif
            Ray ray = new Ray(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerCurrentRaycast.worldNormal);
            // Phisics hits (to block the ray)
            {
                // 3D hits
                {
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, eventData.pointerCurrentRaycast.depth, m_BlockingMask))
                    {
                        hitDistance = hit.distance;
                    }
                }
                // 2D hits
                {
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, eventData.pointerCurrentRaycast.depth, m_BlockingMask);

                    if (hit.collider != null)
                    {
                        float currentHit = hit.fraction * eventData.pointerCurrentRaycast.depth;
                        if (currentHit < hitDistance)
                        {
                            hitDistance = currentHit;
                        }
                    }
                }
            }

            m_RaycastResults.Clear();

            Raycast(m_canvas, ray, hitDistance, eventCamera, m_RaycastResults);
#if LOG_CONSOLE_VRGraphicRaycaster
            //StringBuilder cast = new StringBuilder();
            //for (int i = 0; i < m_RaycastResults.Count; ++i)
            //{
            //    cast.Append(m_RaycastResults[i]);
            //    cast.Append("\n");
            //}
            //if (cast.Length > 0)
            //    Log(cast.ToString());
#endif

            RaycastResult? closestResult = null;
            eventData.hovered.Clear();
            for (var index = 0; index < m_RaycastResults.Count; index++)
            {
                var go = m_RaycastResults[index].graphic.gameObject;

                RaycastResult castResult = new RaycastResult
                {
                    gameObject = go,
                    module = this,
                    distance = m_RaycastResults[index].distance,
                    screenPosition = m_RaycastResults[index].pointerPosition,
                    worldPosition = m_RaycastResults[index].position,
                    index = resultAppendList.Count,
                    depth = m_RaycastResults[index].graphic.depth,
                    sortingLayer = m_canvas.sortingLayerID,
                    sortingOrder = m_canvas.sortingOrder
                };
                resultAppendList.Add(castResult);
                if(!closestResult.HasValue || castResult.distance < closestResult.Value.distance)
                {
                    closestResult = castResult;
                }
                eventData.hovered.Add(castResult.gameObject);
            }
            if (closestResult.HasValue)
            {
                eventData.pointerCurrentRaycast = closestResult.Value;
                eventData.position = closestResult.Value.screenPosition;
                eventData.delta = eventData.position - m_lastPosition;
                m_lastPosition = eventData.position;
            }
        }
        Vector2 m_lastPosition = Vector2.zero;

        struct TGraphicRaycastResult
        {
            public Graphic graphic;
            public float distance;
            public Vector3 position;
            public Vector2 pointerPosition;
            public override string ToString()
            {
                return string.Format("[{0} - {1}, {2}]", graphic.gameObject.name, distance, pointerPosition);
            }
        }
        [NonSerialized]
        static readonly List<TGraphicRaycastResult> s_SortedGraphics = new List<TGraphicRaycastResult>();
        private static void Raycast(Canvas canvas, Ray ray, float hitDistance, Camera eventCamera, List<TGraphicRaycastResult> results)
        {
            // Debug.Log("ttt" + pointerPoision + ":::" + camera);
            // Necessary for the event system
            IList<Graphic> foundGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            for (int i = 0; i < foundGraphics.Count; ++i)
            {
                Graphic graphic = foundGraphics[i];
                // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
                if (graphic.depth == -1 || !graphic.raycastTarget)
                    continue;

                Transform trans = graphic.transform.transform;
                Vector3 transForward = trans.forward;
                // http://geomalgorithms.com/a06-_intersect-2.html
                float distance = (Vector3.Dot(transForward, trans.position - ray.origin) / Vector3.Dot(transForward, ray.direction));

                // Check to see if the go is behind the camera.
                if (distance < 0)
                    continue;

                // is behind some blocking 2D or 3D
                if (distance >= hitDistance)
                    continue;

                Vector3 position = ray.GetPoint(distance);
                Vector2 pointerPosition = eventCamera.WorldToScreenPoint(position);

                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
                    continue;

                if (graphic.Raycast(pointerPosition, eventCamera))
                {
                    s_SortedGraphics.Add(new TGraphicRaycastResult()
                    {
                        graphic = graphic,
                        distance = distance,
                        pointerPosition = pointerPosition,
                        position = position
                    });
                }
            }

            s_SortedGraphics.Sort((g1, g2) => g2.graphic.depth.CompareTo(g1.graphic.depth));
            for (int i = 0; i < s_SortedGraphics.Count; ++i)
            {
                results.Add(s_SortedGraphics[i]);
            }

            s_SortedGraphics.Clear();
        }
    }

}