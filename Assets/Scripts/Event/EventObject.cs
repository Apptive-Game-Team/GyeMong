using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EventTrigger
{
    
}

[Serializable]
public abstract class Event
{
    public abstract IEnumerator execute();
}

[Serializable]
public class WarpPortalEvent : Event
{
    public PortalID portalID;
    public override IEnumerator execute()
    {
        return PortalManager.Instance.TransitScene(portalID);
    }
}

[Serializable]
public class DelayEvent : Event
{
    public float delayTime;
    public override IEnumerator execute()
    {
        yield return new WaitForSeconds(delayTime);
    }
}

[Serializable]
public class FadeInEvent : Event
{
    public override IEnumerator execute()
    {
        return EffectManager.Instance.FadeIn();
    }
}

[Serializable]
public class FadeOutEvent : Event
{
    public override IEnumerator execute()
    {
        return EffectManager.Instance.FadeOut();
    }
}

//[Serializable]
//public class SoundEvent : Event
//{
//    public override IEnumerator execute()
//    {

//    }
//}

public class EventObject : MonoBehaviour
{
    [SerializeField]
    private bool isLoop = false;

    [SerializeField]
    private EventTrigger trigger;

    [SerializeField]
    [SerializeReference]
    public List<Event> eventSequence = new List<Event>();

    private Coroutine eventLoop = null;

    private IEnumerator EventLoop()
    {
        do
        {
            foreach (Event eventObject in eventSequence)
            {
                yield return eventObject.execute();
            }
        } while (isLoop);
    }

    public void TriggerEvent()
    {
        if (eventLoop == null)
        {
            KillEvent();
        }
        eventLoop = StartCoroutine(EventLoop());
    }

    public void KillEvent()
    {
        StopCoroutine(eventLoop);
    }
}
