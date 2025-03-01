using System.Collections;
using System.UI;
using UnityEngine;

namespace System.Event.Event
{
    public abstract class PopupWindowEvent : global::Event
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
}