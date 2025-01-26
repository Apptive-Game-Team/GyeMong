using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.mouse_input
{
    public enum MouseInputState
    {
        ENTERED,
        EXITED,
        CLICKED,
        LONG_CLICKED,
        NONE,
    }
    
    public class MouseInputManager : SingletonObject<MouseInputManager>
    {
        public static float LONG_CLICK_TIME = 0.5f;
        
        List<IMouseInputListener> _listeners = new List<IMouseInputListener>();
        
        private SelectableUI _lastHoveredUI;
        private SelectableUI _hoveredUI;
        private Coroutine _longClickCoroutine;
        
        private void Update()
        {
            _lastHoveredUI = _hoveredUI;
            _hoveredUI = GetHoveredUI();
            CheckClick();
            CheckLongClick();
            CheckEnterExit();
        }

        public void AddListener(IMouseInputListener listener)
        {
            _listeners.Add(listener);
        }
        
        private void CheckEnterExit()
        {
            if (_lastHoveredUI != _hoveredUI)
            {
                if (_lastHoveredUI != null)
                {
                    NotifyListeners(MouseInputState.EXITED, _lastHoveredUI);
                }
                if (_hoveredUI != null)
                {
                    NotifyListeners(MouseInputState.ENTERED, _hoveredUI);
                }
            }
        }
        
        private void CheckClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectableUI _clickedUI = GetClickedUI();
                if (_clickedUI != null)
                {
                    NotifyListeners(MouseInputState.CLICKED, _clickedUI);
                }
            }
        }
        
        private void CheckLongClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_hoveredUI != null)
                {
                    _longClickCoroutine = StartCoroutine(LongClickCoroutine());
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_longClickCoroutine != null)
                {
                    StopCoroutine(_longClickCoroutine);
                }
            }
        }
        
        private IEnumerator LongClickCoroutine()
        {
            yield return new WaitForSeconds(LONG_CLICK_TIME);
            if (_hoveredUI != null)
            {
                NotifyListeners(MouseInputState.LONG_CLICKED, _hoveredUI);
            }
        }
        
        private void NotifyListeners(MouseInputState state, SelectableUI ui)
        {
            foreach (var listener in _listeners)
            {
                listener.OnMouseInput(state, ui);
            }
        }
        
        private SelectableUI GetClickedUI()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    return hit.collider.GetComponent<SelectableUI>();
                }
            }
            return null;
        }
        
        
        private SelectableUI GetHoveredUI()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                return hit.collider.GetComponent<SelectableUI>();
            }
            return null;
        }
    }
}
