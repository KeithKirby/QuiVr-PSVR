using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3D.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace U3D.SteamVR.UI.Watchers
{
    class DownUpClick : SteamVRPointer.IWatcher
    {
        public const float minPressValue = 0.75f;
        public void Process(PointerEventData pointerData, List<RaycastResult> raycastResults, SteamVR_Controller.Device device)
        {
            float triggerValue = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
            pointerData.eligibleForClick = triggerValue > minPressValue;

            if (pointerData.pointerPress == null)
            {
                if (pointerData.eligibleForClick)
                {
                    pointerData.rawPointerPress = raycastResults.Count == 0 ? null : raycastResults[0].gameObject;
                    foreach (RaycastResult raycast in raycastResults)
                    {
                        GameObject targetGO = ExecuteEvents.ExecuteHierarchy(raycast.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
                        if (targetGO != null)
                        {
                            pointerData.pointerPressRaycast = raycast;
                            pointerData.pointerPress = targetGO;
                            pointerData.pressPosition = pointerData.position;
                            break;
                        }
                    }
                }
            }
            else
            {
                if(!pointerData.eligibleForClick)
                {
                    ExecuteEvents.ExecuteHierarchy(pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);
                }
                else
                {
                    pointerData.eligibleForClick = false;
                    foreach (GameObject i in pointerData.hovered)
                    {
                        if(i.transform.BelongsToHierarchy(pointerData.pointerPress.transform))
                        {
                            pointerData.eligibleForClick = true;
                        }
                    }
                }

                if(!pointerData.eligibleForClick)
                {
                    ExecuteEvents.ExecuteHierarchy(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);
                    pointerData.pointerPress = null;
                    pointerData.rawPointerPress = null;
                    pointerData.rawPointerPress = null;
                }
            }
        }
    }
}
