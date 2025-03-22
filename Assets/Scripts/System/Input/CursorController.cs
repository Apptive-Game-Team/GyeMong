using System.Game.Rune.RuneUI;
using System.Input.Interface;
using UnityEngine;

namespace System.Input
{
    public class CursorController : MonoBehaviour, IMouseInputListener
    {
        private void Start()
        {
            MouseInputManager.Instance.AddListener(this);
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
