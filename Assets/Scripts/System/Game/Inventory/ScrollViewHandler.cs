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
        [SerializeField] private int divisionCount;
        private float stepSize => 1f / (divisionCount - 1);
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
                _scrollRect.horizontalNormalizedPosition += stepSize;
                _scrollRect.horizontalNormalizedPosition = Mathf.Min(_scrollRect.horizontalNormalizedPosition, 1f);
            }
            else
                _scrollRect.horizontalNormalizedPosition = 1f;
        }
        private void ScrollLeft()
        {
            if (_scrollRect.horizontalNormalizedPosition > 0f)
            {
                _scrollRect.horizontalNormalizedPosition -= stepSize;
                _scrollRect.horizontalNormalizedPosition = Mathf.Max(_scrollRect.horizontalNormalizedPosition, 0f);
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

