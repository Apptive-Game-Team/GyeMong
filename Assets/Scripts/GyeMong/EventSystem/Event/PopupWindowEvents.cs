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
            
            float timer = Time.time;
            yield return new WaitUntil(() => (timer + Duration < Time.time) || InputManager.Instance.GetKeyDown(ActionCode.Interaction));
            
            yield return PopupWindowController.Instance.ClosePopupWindow();
        }
    }
}