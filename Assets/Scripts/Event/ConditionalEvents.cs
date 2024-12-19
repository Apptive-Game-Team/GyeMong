using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ConditionalEvent : Event {

}

[Serializable]
public class ConditionalBranchEvent : ConditionalEvent
{
    [SerializeReference]
    protected Condition condition;

    [SerializeReference]
    private Event eventInTrue;

    [SerializeReference]
    private Event eventInFalse;

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        if (condition.Check())
        {
            return eventInTrue.Execute();
        } else
        {
            return eventInFalse.Execute();
        }
    }
    public override List<ToggeableCondition> FindToggleableConditions()
    {
        List<ToggeableCondition> result = new List<ToggeableCondition>();
        List<ToggeableCondition> temp;
        try
        {
            temp = eventInTrue.FindToggleableConditions();
        }
        catch
        {
            return null;
        }
        
        if (temp != null)
        {
            result.AddRange(temp);
        }
        temp = eventInFalse.FindToggleableConditions();
        if (temp != null)
        {
            result.AddRange(temp);
        }
        if (condition is ToggeableCondition)
        {
            result.Add((ToggeableCondition) condition);
        }
        return result;
    }
}

[Serializable]
public class ConditionalLoopEvent : ConditionalEvent
{
    [SerializeReference]
    protected Condition condition;

    [SerializeReference]
    private Event loopBodyevent;

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        while (condition.Check())
        {
            yield return loopBodyevent.Execute();
        }
    }
    public override List<ToggeableCondition> FindToggleableConditions()
    {
        List<ToggeableCondition> result = new List<ToggeableCondition>();

        List<ToggeableCondition> temp = loopBodyevent.FindToggleableConditions();
        if (temp != null)
        {
            result.AddRange(temp);
        }
        if (condition is ToggeableCondition)
        {
            result.Add((ToggeableCondition)condition);
        }
        return result;
    }
}

[Serializable]
public class ToggleConditionEvent : ConditionalEvent
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