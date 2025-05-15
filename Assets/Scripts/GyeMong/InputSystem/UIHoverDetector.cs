using System;
using System.Collections.Generic;
using GyeMong.InputSystem.Interface;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GyeMong.InputSystem
{
    [Serializable]
    public class UIHoverDetector
    {
        private GraphicRaycaster _raycaster;
        private UnityEngine.EventSystems.EventSystem _eventSystem;
        private MonoBehaviour _context;

        public UIHoverDetector(GraphicRaycaster raycaster)
        {
            _raycaster = raycaster;
            _eventSystem = UnityEngine.EventSystems.EventSystem.current;
        }

        public ISelectableUI GetHoveredUI()
        {
            PointerEventData pointerData = new PointerEventData(_eventSystem)
            {
                position = UnityEngine.Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            _raycaster.Raycast(pointerData, results);

            foreach (var result in results)
            {
                ISelectableUI selectable = result.gameObject.GetComponent<ISelectableUI>();
                if (selectable != null)
                    return selectable;
            }

            return null;
        }
    }
}