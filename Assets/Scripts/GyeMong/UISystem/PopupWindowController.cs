using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Util;

namespace GyeMong.UISystem
{
    public class PopupWindowController : SingletonObject<PopupWindowController>
    {
        public enum PopupImageType
        {
            None = 0,
            Wasd = 1,
            Shift = 2,
            MouseLeft = 3,
            MouseRight = 4,
            Heal = 5,
        }
        
        [SerializeField] private GameObject _popupWindow;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private GameObject[] popupImages;
        [SerializeField] private TMP_Text _contentText;
        private RectTransform _rectTransform;
        
        private const float MOVE_SPEED = 1.0f;
        private const float TARGET_X = 300.0f;
        
        protected override void Awake()
        {
            base.Awake();
            _rectTransform = _popupWindow.GetComponent<RectTransform>();
            _popupWindow.SetActive(false);
        }

        public IEnumerator OpenPopupWindow(String title = "", String content = "", PopupImageType type = PopupImageType.None)
        {
            _titleText.text = title;
            _contentText.text = content;
            SetImage(type);
            _popupWindow.SetActive(true);
            Time.timeScale = 0.0f;
            return null;
        }
        public IEnumerator ClosePopupWindow()
        {
            _popupWindow.SetActive(false);
            Time.timeScale = 1f;
            return null;
        }

        private IEnumerator MoveWindow(float targetX)
        {
            while (Mathf.Abs(_rectTransform.anchoredPosition.x - targetX) > 0.1f)
            {
                _rectTransform.anchoredPosition = Vector2.MoveTowards(_rectTransform.anchoredPosition, new Vector2(targetX, _rectTransform.anchoredPosition.y), MOVE_SPEED);
                yield return null;
            }
        }

        private void SetImage(PopupImageType type)
        {
            foreach (var image in popupImages)
            {
                image.SetActive(false);
            }

            if (type != PopupImageType.None)
            {
                popupImages[(int)type - 1].SetActive(true);
            }
        }
    }
}
