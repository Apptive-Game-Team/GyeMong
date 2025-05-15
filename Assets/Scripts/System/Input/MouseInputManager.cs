using System;
using System.Collections;
using System.Collections.Generic;
using System.Input;
using UnityEngine;
using UnityEngine.UI;

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

        private UIHoverDetector _hoverDetector;
        
        private ISelectableUI _lastHoveredUI;
        private ISelectableUI _hoveredUI;
        private Coroutine _longClickCoroutine;
        
        public void SetRaycaster(GraphicRaycaster raycaster)
        {
            _hoverDetector = new UIHoverDetector(raycaster);
        }
        
        private void Update()
        {
            if (_hoverDetector != null)
            {
                _lastHoveredUI = _hoveredUI;
                _hoveredUI = _hoverDetector.GetHoveredUI();
                CheckClick();
                CheckLongClick();
                CheckEnterExit();
            }
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
                ISelectableUI _clickedUI = GetClickedUI();
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
        
        private void NotifyListeners(MouseInputState state, ISelectableUI ui)
        {
            //Fix Error : Rune UI Interact Triple;
            if (state == MouseInputState.CLICKED)
            {
                ui.OnInteract();
            } else if (state == MouseInputState.LONG_CLICKED)
            {
                ui.OnLongInteract();
            }
            
            foreach (var listener in _listeners)
            {
                listener.OnMouseInput(state, ui);
            }
        }
        
        private ISelectableUI GetClickedUI()
        {
            if (Input.GetMouseButtonDown(0))
            {
                return _hoverDetector.GetHoveredUI();
            }
            return null;
        }
    }
}
