using GyeMong.InputSystem;
using GyeMong.InputSystem.Interface;
using UnityEngine;

namespace System.Input
{
    public class CursorController : MonoBehaviour, IMouseInputListener
    {
        private void Start()
        {
            MouseInputManager.Instance.AddListener(this);
        }
    
        private void OnDestroy()
        {
            //MouseInputManager.Instance.RemoveListener(this);
        }

        public void OnMouseInput(MouseInputState state, ISelectableUI ui)
        {
            if (state == MouseInputState.ENTERED)
            {
                MonoBehaviour mono = (MonoBehaviour) ui;
                transform.position = mono.transform.position;
            }
        }
    }
}
