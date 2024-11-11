using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Event
{
    public abstract IEnumerator execute();
}

[Serializable]
public class HurtEffectEvent : Event
{
    public float amount;
    public override IEnumerator execute()
    {
        return EffectManager.Instance.HurtEffect(amount);
    }
}

[Serializable]
public class ShakeCameraEvent : Event
{
    public float time;
    public override IEnumerator execute()
    {
        return EffectManager.Instance.ShakeCamera(time);
    }
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

[Serializable]
public class SoundEvent : Event
{
    [SerializeField]
    private SoundObject soundObject;

    private enum PlayOrStop{
        PLAY,
        STOP
    }

    [SerializeField]
    private PlayOrStop playOrStop = PlayOrStop.PLAY;

    [SerializeField]
    private string soundName = null;

    public override IEnumerator execute()
    {
        if (playOrStop == PlayOrStop.STOP)
        {
            soundObject.Stop();
            return null;
        } else {
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
    public override IEnumerator execute()
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
    public override IEnumerator execute()
    {
        targetObject.KillEvent();
        yield return null;
    }
}

public class EventObject : InteractableObject
{
    private enum EventTrigger
    {
        OnCollisionEnter = 0,
        OnInteraction = 1,
        OnAwake = 2,
    }

    [SerializeField]
    private bool isLoop = false;

    [SerializeField]
    private EventTrigger trigger;

    [SerializeField]
    private int triggerLimitCounter = -1;

    [SerializeField]
    [SerializeReference]
    public List<Event> eventSequence = new List<Event>();

    private Coroutine eventLoop = null;

    private void Start()
    {
        if (trigger == EventTrigger.OnAwake && triggerLimitCounter != 0)
        {
            TriggerEvent();
            triggerLimitCounter -= 1;
        }
    }

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
        if (eventLoop != null)
        {
            KillEvent();
        }
        eventLoop = StartCoroutine(EventLoop());
    }

    public void KillEvent()
    {
        if (eventLoop != null)
            StopCoroutine(eventLoop);
    }

    protected override void OnInteraction(Collider2D collision)
    {
        if (trigger == EventTrigger.OnInteraction && triggerLimitCounter != 0)
        {
            TriggerEvent();
            triggerLimitCounter -= 1;
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (trigger == EventTrigger.OnCollisionEnter && triggerLimitCounter != 0)
        {
            TriggerEvent();
            triggerLimitCounter -= 1;
        }
    }
}
