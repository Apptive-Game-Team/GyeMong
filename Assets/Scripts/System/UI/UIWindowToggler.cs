using System.Input.Interface;
using playerCharacter;
using UI.mouse_input;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace System.UI
{
    public class UIWindowToggler<T> : SingletonObject<T>, IInputListener where T : MonoBehaviour
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
                PlayerCharacter.Instance.SetPlayerMove(!_isOptionOpened);
            }
        }
    
        protected override void Awake()
        {
            base.Awake();
            InputManager.Instance.SetInputListener(this);
        }

        private void OnEnable()
        {
            _window.SetActive(_isOptionOpened);
            MouseInputManager.Instance.SetRaycaster(_window.GetComponentInChildren<GraphicRaycaster>());
        }

        public void OnKey(ActionCode action, InputType type)
        {
            if (action == toggleKeyActionCode && type == InputType.Down)
            {
                OpenOrCloseOption();
            }
        }
    }
}
