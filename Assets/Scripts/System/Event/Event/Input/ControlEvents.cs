
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using playerCharacter;

public abstract class ControlEvent : Event { }

[Serializable]
public class SetKeyInputEvent : ControlEvent
{
    [SerializeField] private bool _isEnable;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        InputManager.Instance.SetActionState(_isEnable);
        PlayerCharacter.Instance.isControlled = !_isEnable;
        PlayerCharacter.Instance.StopPlayer(_isEnable);
        yield return null;
    }
}

[Serializable]
public class SetActiveEvent : ControlEvent
{
    enum ActiveState
    {
        ONCE_ACTIVE = 1,
        INFINITE_ACTIVE = -1,
        INACTIVE = 0
    }
    [SerializeField] private EventObject eventObject;
    [SerializeField] private ActiveState _activeState;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        // EventObject _object = (EventObject)eventObject;
        this.eventObject.SetTriggerLimitCounter((int) _activeState);
        return null;
    }
}

[Serializable]
public class NestedEventEvent : ControlEvent
{
    [SerializeReference]
    private List<Event> events;

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        foreach (Event @event in events)
        {
            yield return @event?.Execute();
        }
    }
    
    public override Event[] GetChildren()
    {
        return events.ToArray();
    }
    
    public override List<ToggeableCondition> FindToggleableConditions()
    {
        List<ToggeableCondition> result = new List<ToggeableCondition>();
        foreach (Event @event in events)
        {
            List<ToggeableCondition> temp = @event?.FindToggleableConditions();
            if (temp != null)
            {
                result.AddRange(temp);
            }
        }
        return result;
    }
}