using System.Collections;
using System.Collections.Generic;
using System.Input;
using UnityEngine;
using UnityEngine.UI;

namespace System.Game.Inventory
{
    public class ScrollViewHandler : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _rectTransform;
        private void OnEnable()
        {
            _scrollRect.horizontalNormalizedPosition = 0f;
        }
        private void Update()
        {
            ScrollCalc();
        }
        private void ScrollRight()
        {
            if(_scrollRect.horizontalNormalizedPosition < 1f)
            {
                _scrollRect.horizontalNormalizedPosition += 0.01f;
            }
            else
                _scrollRect.horizontalNormalizedPosition = 1f;
        }
        private void ScrollLeft()
        {
            if (_scrollRect.horizontalNormalizedPosition > 0f)
            {
                _scrollRect.horizontalNormalizedPosition -= 0.01f;
            }
            else
                _scrollRect.horizontalNormalizedPosition = 0f;
        }
        private void ScrollCalc()
        {
            if (InputManager.Instance.GetKeyDown(ActionCode.MenuRight))
                ScrollRight();
            else if (InputManager.Instance.GetKeyDown(ActionCode.MenuLeft))
                ScrollLeft();
        }
    }
}

