using System.Collections;
using UnityEngine;

namespace System.Event.Event.Status
{
    public abstract class StatusEvent : Event { }

    public class BoolStatusEvent : StatusEvent
    {
        [SerializeField] private EventStatus<bool> status;
        [SerializeField] private bool value;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            status.SetStatus(value);
            yield return null;
        }
    }

    public class IntStatusEvent : StatusEvent
    {
        [SerializeField] private EventStatus<int> status;
        [SerializeField] private int value;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            status.SetStatus(value);
            yield return null;
        }
    }
}