using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.PackageManager;


[Serializable]
public abstract class Event
{
    public abstract IEnumerator execute(EventObject eventObject = null);
    public virtual List<ToggeableCondition> FindToggleableConditions()
    {
        return null;
    }
}

[Serializable]
public class HurtEffectEvent : Event
{
    public float amount;
    public override IEnumerator execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.HurtEffect(amount);
    }
}

[Serializable]
public class ShakeCameraEvent : Event
{
    public float time;
    public override IEnumerator execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.ShakeCamera(time);
    }
}

[Serializable]
public class WarpPortalEvent : Event
{
    public PortalID portalID;
    public override IEnumerator execute(EventObject eventObject = null)
    {
        return PortalManager.Instance.TransitScene(portalID);
    }
}

[Serializable]
public class DelayEvent : Event
{
    public float delayTime;
    public override IEnumerator execute(EventObject eventObject = null)
    {
        yield return new WaitForSeconds(delayTime);
    }
}

[Serializable]
public class FadeInEvent : Event
{
    public override IEnumerator execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.FadeIn();
    }
}

[Serializable]
public class FadeOutEvent : Event
{
    public override IEnumerator execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.FadeOut();
    }
}

[Serializable]
public class SoundEvent : Event
{
    [SerializeField]
    private SoundObject soundObject;

    private enum PlayOrStop
    {
        PLAY,
        STOP
    }

    [SerializeField]
    private PlayOrStop playOrStop = PlayOrStop.PLAY;

    [SerializeField]
    private string soundName = null;

    public override IEnumerator execute(EventObject eventObject = null)
    {
        if (playOrStop == PlayOrStop.STOP)
        {
            soundObject.Stop();
            return null;
        }
        else
        {
            if (soundName != null)
                soundObject.SetSoundSourceByName(soundName);
            return soundObject.Play();
        }
    }
}

[Serializable]
public class BGMEvent : Event
{
    [SerializeField]
    private string bgmName;
    public override IEnumerator execute(EventObject eventObject = null)
    {
        SoundObject soundObject = SoundManager.Instance.GetBgmObject();
        soundObject.SetSoundSourceByName(bgmName);
        return soundObject.Play();
    }
}

[Serializable]
public class StopEvent : Event
{
    [SerializeField]
    private EventObject targetObject;
    public override IEnumerator execute(EventObject eventObject = null)
    {
        targetObject.KillEvent();
        yield return null;
    }
}

[Serializable]
public class NestedEventEvent : Event
{
    [SerializeReference]
    private List<Event> events;

    public override IEnumerator execute(EventObject eventObject = null)
    {
        foreach (Event @event in events)
        {
            yield return @event.execute();
        }
    }
    public override List<ToggeableCondition> FindToggleableConditions()
    {
        List<ToggeableCondition> result = new List<ToggeableCondition>();
        foreach (Event @event in events)
        {
            List<ToggeableCondition> temp = @event.FindToggleableConditions();
            if (temp != null)
            {
                result.AddRange(temp);
            }
        }
        return result;
    }
}

[Serializable]
public class ConditionalBranchEvent : Event
{
    [SerializeReference]
    protected Condition condition;

    [SerializeReference]
    private Event eventInTrue;

    [SerializeReference]
    private Event eventInFalse;

    public override IEnumerator execute(EventObject eventObject = null)
    {
        if (condition.Check())
        {
            return eventInTrue.execute();
        } else
        {
            return eventInFalse.execute();
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
public class ConditionalLoopEvent : Event
{
    [SerializeReference]
    protected Condition condition;

    [SerializeReference]
    private Event loopBodyevent;

    public override IEnumerator execute(EventObject eventObject = null)
    {
        while (condition.Check())
        {
            yield return loopBodyevent.execute();
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
public class ToggleConditionEvent : Event
{
    [SerializeReference]
    private string tag;
    [SerializeReference]
    private bool condition;

    public override IEnumerator execute(EventObject eventObject = null)
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

[Serializable]
public class MovePositionEvent : Event
{
    [SerializeField]
    private GameObject @gameObject;
    [SerializeField]
    private Vector3 targetPosition;

    public override IEnumerator execute(EventObject eventObject = null)
    {
        @gameObject.transform.position = targetPosition;
        yield return null;
    }
}