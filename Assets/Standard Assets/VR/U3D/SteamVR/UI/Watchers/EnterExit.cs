using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3D.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace U3D.SteamVR.UI.Watchers
{
    class EnterExit : SteamVRPointer.IWatcher
    {
        public void Process(PointerEventData pointerData, List<RaycastResult> raycastResults, SteamVR_Controller.Device device)
        {
            if(pointerData.pointerEnter == null)
            {
                foreach (RaycastResult raycast in raycastResults)
                {
                    GameObject targetGO = ExecuteEvents.ExecuteHierarchy(raycast.gameObject, pointerData, ExecuteEvents.pointerEnterHandler);
                    if (targetGO != null)
                    {
                        pointerData.pointerCurrentRaycast = raycast;
                        pointerData.pointerEnter = targetGO;
                        break;
                    }
                }
            }
            else
            {
                // Needs to be first result on the list, a new object can come on front and thats the one which is overed now (it happens in the dropdown)
                if (raycastResults.Count == 0 || !raycastResults[0].gameObject.transform.BelongsToHierarchy(pointerData.pointerEnter.transform))
                {
                    ExecuteEvents.ExecuteHierarchy(pointerData.pointerEnter, pointerData, ExecuteEvents.pointerExitHandler);
                    pointerData.pointerEnter = null;
                }
            }
        }
    }
}
