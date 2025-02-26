using System.Input.Interface;
using playerCharacter;
using UI.mouse_input;
using UnityEngine;
using UnityEngine.UI;

namespace System.UI
{
    public class UIWindowToggler<T> : SingletonObject<T>, IInputListener where T : MonoBehaviour
    {
        protected ActionCode toggleKeyActionCode;
        private bool _isOptionOpened = false;

        protected void OpenOrCloseOption()
        {
            _isOptionOpened = !_isOptionOpened;
            gameObject.SetActive(_isOptionOpened);
            PlayerCharacter.Instance.SetPlayerMove(!_isOptionOpened);
        }
    
        protected override void Awake()
        {
            base.Awake();
            InputManager.Instance.SetInputListener(this);
        }

        private void OnEnable()
        {
            gameObject.SetActive(_isOptionOpened);
            MouseInputManager.Instance.SetRaycaster(GetComponentInChildren<GraphicRaycaster>());
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
