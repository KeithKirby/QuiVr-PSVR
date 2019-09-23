using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3D.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace U3D.SteamVR.UI.Watchers
{
    class SelectDeselect : SteamVRPointer.IWatcher
    {
        Func<GameObject> m_onGetSelectObject;
        Action<GameObject> m_onSetSelectObject;
        public SelectDeselect(Func<GameObject> onGetSelectObject, Action<GameObject> onSetSelectObject)
        {
            m_onGetSelectObject = onGetSelectObject;
            m_onSetSelectObject = onSetSelectObject;
        }
        
        public const float minPressValue = 0.1f;
        public void Process(PointerEventData pointerData, List<RaycastResult> raycastResults, SteamVR_Controller.Device device)
        {
            if (device == null)
                return;
            float triggerValue = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
            if(triggerValue > minPressValue)
            {
                GameObject targetGO = null;
                foreach (RaycastResult raycast in raycastResults)
                {
                    targetGO = ExecuteEvents.ExecuteHierarchy(raycast.gameObject, pointerData, ExecuteEvents.selectHandler);
                    if (targetGO != null)
                    {
                        break;
                    }
                }
                if(targetGO== null)
                {
                    GameObject currentSelected = m_onGetSelectObject();
                    if(currentSelected!=null)
                    {
                        ExecuteEvents.ExecuteHierarchy(currentSelected, pointerData, ExecuteEvents.deselectHandler);
                    }
                }
                m_onSetSelectObject(targetGO);
            }
        }
    }
}
