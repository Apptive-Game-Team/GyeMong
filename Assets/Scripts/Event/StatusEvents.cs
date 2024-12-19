using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolStatusEvent : Event
{
    [SerializeField] private EventStatus<bool> status;
    [SerializeField] private bool value;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        status.SetStatus(value);
        yield return null;
    }
}
