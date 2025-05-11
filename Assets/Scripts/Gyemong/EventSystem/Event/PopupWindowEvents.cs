using System.Collections;
using Gyemong.InputSystem;
using Gyemong.UISystem;
using UnityEngine;

namespace Gyemong.EventSystem.Event
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
        [SerializeField] private string _title;
        [SerializeField] private string _message;
        [SerializeField] private float _duration;
    
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            yield return PopupWindowController.Instance.OpenPopupWindow(_title, _message);
            
            float timer = Time.time;
            yield return new WaitUntil(() =>
            {
                return (timer + _duration < Time.time) || InputManager.Instance.GetKeyDown(ActionCode.Interaction);
            });
            
            yield return PopupWindowController.Instance.ClosePopupWindow();
        }
    }
}