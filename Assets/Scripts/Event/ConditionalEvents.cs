using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ConditionalEvent : Event
{
    [SerializeReference]
    protected Condition _condition;
    public override List<ToggeableCondition> FindToggleableConditions()
    {
        List<ToggeableCondition> result = new List<ToggeableCondition>();
        Event[] children = GetChildren();
        if (children != null)
        {
            foreach (Event @event in children)
            {
                List<ToggeableCondition> temp;
                temp = @event.FindToggleableConditions();
                if (temp != null)
                {
                    result.AddRange(temp);
                }
            }
        }
        
        if (_condition is ToggeableCondition)
        {
            result.Add((ToggeableCondition) _condition);
        }
        return result;
    }
}

[Serializable]
public class ConditionalBranchEvent : ConditionalEvent
{
    [SerializeReference]
    private Event eventInTrue;

    [SerializeReference]
    private Event eventInFalse;

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        if (_condition.Check())
        {
            return eventInTrue.Execute();
        } else
        {
            return eventInFalse.Execute();
        }
    }

    public override Event[] GetChildren()
    {
        return new Event[] { eventInTrue, eventInFalse };
    }
}

[Serializable]
public class ConditionalLoopEvent : ConditionalEvent
{
    [SerializeReference]
    private Event loopBodyevent;

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        while (_condition.Check())
        {
            yield return loopBodyevent.Execute();
        }
    }

    public override Event[] GetChildren()
    {
        return new Event[] { loopBodyevent };
    }
}

[Serializable]
public class ToggleConditionEvent : Event
{
    [SerializeReference]
    private string tag;
    [SerializeReference]
    private bool condition;

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        List<ToggeableCondition> conditions = EventObject.toggleableConditions[tag];
        if (conditions == null)
        {
            Debug.Log("Toggleable Conditions is not found by " + tag);
            yield return null;
        }
        foreach (ToggeableCondition condition in conditions)
        {
            try
            {
                condition.SetCondition(this.condition);
            } catch
            {
                EventObject.toggleableConditions[tag].Remove(condition);
            }
        }
    }
}