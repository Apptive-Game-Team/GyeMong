using System.Collections.Generic;
using System.Game.Rune.RuneUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace System.Input
{
    [Serializable]
    public class UIHoverDetector
    {
        private GraphicRaycaster _raycaster;
        private EventSystem _eventSystem;
        private MonoBehaviour _context;

        public UIHoverDetector(GraphicRaycaster raycaster)
        {
            _raycaster = raycaster;
            _eventSystem = EventSystem.current;
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