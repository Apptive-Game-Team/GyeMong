using DG.Tweening;
using Gyemong.InputSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Gyemong.GameSystem.Inventory
{
    public class ScrollViewHandler : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private int divisionCount;
        [SerializeField] private float tweenDuration = 0.3f;

        private float stepSize => 1f / (divisionCount - 1);
        private Tween currentTween;

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
            if (_scrollRect.horizontalNormalizedPosition < 1f)
            {
                float target = Mathf.Min(_scrollRect.horizontalNormalizedPosition + stepSize, 1f);
                TweenToPosition(target);
            }
        }

        private void ScrollLeft()
        {
            if (_scrollRect.horizontalNormalizedPosition > 0f)
            {
                float target = Mathf.Max(_scrollRect.horizontalNormalizedPosition - stepSize, 0f);
                TweenToPosition(target);
            }
        }

        private void TweenToPosition(float target)
        {
            currentTween?.Kill();
            currentTween = DOTween.To(
                () => _scrollRect.horizontalNormalizedPosition,
                value => _scrollRect.horizontalNormalizedPosition = value,
                target,
                tweenDuration
            ).SetEase(Ease.OutCubic);
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