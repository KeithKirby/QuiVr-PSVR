using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3D.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace U3D.SteamVR.UI.Watchers
{
    class Drag : SteamVRPointer.IWatcher
    {
        public void Process(PointerEventData pointerData, List<RaycastResult> raycastResults, SteamVR_Controller.Device device)
        {
            if (pointerData == null || device == null)
                return;
            pointerData.dragging = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x > DownUpClick.minPressValue) && pointerData.delta != Vector2.zero;

            if (pointerData.pointerDrag==null)
            {
                if (pointerData.dragging)
                {
                    foreach (RaycastResult raycast in raycastResults)
                    {
                        // without this the Scroll views wont work
                        pointerData.pointerPressRaycast = raycast;

                        ExecuteEvents.ExecuteHierarchy(raycast.gameObject, pointerData, ExecuteEvents.initializePotentialDrag);
                        ExecuteEvents.ExecuteHierarchy(raycast.gameObject, pointerData, ExecuteEvents.beginDragHandler);
                        GameObject targetGO = ExecuteEvents.ExecuteHierarchy(raycast.gameObject, pointerData, ExecuteEvents.dragHandler);
                        if (targetGO != null)
                        {
                            pointerData.pointerDrag = targetGO;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (pointerData.dragging)
                {
                    pointerData.dragging = false;
                    foreach (GameObject i in pointerData.hovered)
                    {
                        if (i.transform.BelongsToHierarchy(pointerData.pointerDrag.transform))
                        {
                            pointerData.dragging = true;
                        }
                    }
                }

                if (!pointerData.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy(pointerData.pointerDrag, pointerData, ExecuteEvents.endDragHandler);
                    foreach (RaycastResult raycast in raycastResults)
                    {
                        ExecuteEvents.ExecuteHierarchy(raycast.gameObject, pointerData, ExecuteEvents.dropHandler);
                    }
                    pointerData.pointerDrag = null;
                }
                else
                {
                    ExecuteEvents.ExecuteHierarchy(pointerData.pointerDrag, pointerData, ExecuteEvents.dragHandler);
                }
            }
        }
    }
}
