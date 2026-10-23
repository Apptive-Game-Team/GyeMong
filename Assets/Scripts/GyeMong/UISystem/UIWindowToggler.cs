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
        protected virtual bool LocksPlayerMovement => true;
        protected virtual void OnWindowStateChanged(bool opened) { }

        protected void OpenOrCloseOption()
        {
            if (SceneTransitionCurtain.IsCovered) return;
            _isOptionOpened = !_isOptionOpened;
            _window.SetActive(_isOptionOpened);
            OnWindowStateChanged(_isOptionOpened);
            if (LocksPlayerMovement && SceneContext.Character != null)
            {
                SceneContext.Character.SetPlayerMove(!_isOptionOpened);
            }
        }
    
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            InputManager.OnKeyEvent += OnKey;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            if (_isOptionOpened) CloseWindow();
            InputManager.OnKeyEvent -= OnKey;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void CloseWindow()
        {
            if (!_isOptionOpened) { if (_window != null) _window.SetActive(false); return; }
            if (LocksPlayerMovement && SceneContext.Character != null) SceneContext.Character.SetPlayerMove(true);
            _isOptionOpened = false;
            if (_window != null) _window.SetActive(false);
            OnWindowStateChanged(false);
        }

        private void OnDisable() { CloseWindow(); }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) { CloseWindow(); }

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
