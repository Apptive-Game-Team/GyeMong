using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GyeMong.UISystem
{
    public class PopupWindowController : MonoBehaviour
    {
        private static PopupWindowController _instance;
        public static PopupWindowController Instance =>
            _instance != null ? _instance : (_instance = FindObjectOfType<PopupWindowController>(true));

        public bool IsOpen { get; private set; }
        private float _previousTimeScale;
        [SerializeField] private GameObject _popupWindow;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _contentText;
        private RectTransform _rectTransform;
        
        private const float MOVE_SPEED = 1.0f;
        private const float TARGET_X = 300.0f;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _rectTransform = _popupWindow.GetComponent<RectTransform>();
            _popupWindow.SetActive(false);
        }

        public IEnumerator OpenPopupWindow(String title = "", String content = "")
        {
            if (!IsOpen)
                _previousTimeScale = Time.timeScale;
            IsOpen = true;
            _titleText.text = title;
            _contentText.text = content;
            _popupWindow.SetActive(true);
            Time.timeScale = 0.0f;
            return null;
        }
        public IEnumerator ClosePopupWindow()
        {
            if (!IsOpen) return null;
            IsOpen = false;
            _popupWindow.SetActive(false);
            Time.timeScale = _previousTimeScale;
            return null;
        }

        private void OnDisable()
        {
            ClosePopupWindow();
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        private IEnumerator MoveWindow(float targetX)
        {
            while (Mathf.Abs(_rectTransform.anchoredPosition.x - targetX) > 0.1f)
            {
                _rectTransform.anchoredPosition = Vector2.MoveTowards(_rectTransform.anchoredPosition, new Vector2(targetX, _rectTransform.anchoredPosition.y), MOVE_SPEED);
                yield return null;
            }
        }
        
    }
}
