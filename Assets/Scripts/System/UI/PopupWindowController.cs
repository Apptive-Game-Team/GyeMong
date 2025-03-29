using System.Collections;
using TMPro;
using UnityEngine;

namespace System.UI
{
    public class PopupWindowController : SingletonObject<PopupWindowController>
    {
        [SerializeField] private GameObject _popupWindow;
        [SerializeField] private TMP_Text _titleText;
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

        public IEnumerator OpenPopupWindow(String title = "", String content = "")
        {
            _titleText.text = title;
            _contentText.text = content;
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
        
    }
}
