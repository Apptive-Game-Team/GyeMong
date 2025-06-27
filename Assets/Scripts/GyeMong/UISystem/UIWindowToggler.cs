using GyeMong.InputSystem;
using GyeMong.InputSystem.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace GyeMong.UISystem
{
    public class UIWindowToggler<T> : SingletonObject<T> where T : MonoBehaviour
    {
        protected ActionCode toggleKeyActionCode;
        private bool _isOptionOpened = false;
        [SerializeField] private GameObject _window;

        protected void OpenOrCloseOption()
        {
            _isOptionOpened = !_isOptionOpened;
            _window.SetActive(_isOptionOpened);
            if (SceneManager.GetActiveScene().name != "TitleScene")
            {
                SceneContext.Character.SetPlayerMove(!_isOptionOpened);
            }
        }
    
        protected override void Awake()
        {
            base.Awake();
            InputManager.OnKeyEvent += OnKey;
        }
        
        private void OnDestroy()
        {
            InputManager.OnKeyEvent -= OnKey;
        }

        private void OnEnable()
        {
            _window.SetActive(_isOptionOpened);
            MouseInputManager.Instance.SetRaycaster(_window.GetComponentInChildren<GraphicRaycaster>());
        }

        private void OnKey(ActionCode action, InputType type)
        {
            if (action == toggleKeyActionCode && type == InputType.Down)
            {
                OpenOrCloseOption();
            }
        }
    }
}
