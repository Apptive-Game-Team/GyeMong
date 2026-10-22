using System.Collections;
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
            yield return PopupWindowController.Instance.OpenPopupWindow(Title, Message);
            
            float timer = Time.unscaledTime;
            yield return new WaitUntil(() =>
            {
                // Consume both buffered actions, including on the timeout frame.
                bool interaction = InputManager.Instance.GetKeyDown(ActionCode.Interaction);
                bool enter = InputManager.Instance.GetKeyDown(ActionCode.Enter);
                return interaction || enter || Time.unscaledTime >= timer + Duration;
            });
            
            yield return PopupWindowController.Instance.ClosePopupWindow();
        }
    }
}
