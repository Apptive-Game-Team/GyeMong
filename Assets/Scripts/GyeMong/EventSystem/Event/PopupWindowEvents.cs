using System.Collections;
using System.Collections.Generic;
using GyeMong.InputSystem;
using GyeMong.UISystem;
using UnityEngine;

namespace GyeMong.EventSystem.Event
{
    public abstract class PopupWindowEvent : Event
    {
    }

    public class OpenPopupWindowEvent : PopupWindowEvent
    {
        [SerializeField] private string _title;
        [SerializeField] private string _message;
        
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return PopupWindowController.Instance.OpenPopupWindow(_title, _message);
        }
    }

    public class ClosePopupWindowEvent : PopupWindowEvent
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return PopupWindowController.Instance.ClosePopupWindow();
        }
    }
    
    public class SkippablePopupWindowEvent : PopupWindowEvent
    {
        public string Title;
        public string Message;
        public float Duration;
    
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            var input = InputManager.Instance;
            var previousStates = new Dictionary<ActionCode, bool>();
            foreach (ActionCode action in input.GetKeyActions().Keys)
            {
                previousStates[action] = input.GetKeyActive(action);
                input.SetKeyActive(action, false);
            }
            input.ClearBufferedInput();
            int openedFrame = Time.frameCount;
            try
            {
                yield return PopupWindowController.Instance.OpenPopupWindow(Title, Message);

                float timer = Time.unscaledTime;
                yield return new WaitUntil(() => input.ConsumeAdvanceInput(openedFrame) ||
                                                Time.unscaledTime >= timer + Duration);
            }
            finally
            {
                PopupWindowController.Instance.ClosePopupWindow();
                input.ClearBufferedInput();
                foreach (var state in previousStates)
                    input.SetKeyActive(state.Key, state.Value);
            }
        }
    }
}
