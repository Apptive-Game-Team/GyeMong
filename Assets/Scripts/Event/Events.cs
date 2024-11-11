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

    private enum PlayOrStop
    {
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

[Serializable]
public class NestedEventEvent : Event
{
    [SerializeReference]
    private List<Event> events;

    public override IEnumerator execute()
    {
        foreach (Event eventObject in events)
        {
            yield return eventObject.execute();
        }
    }
}

[Serializable]
public class ConditionalEvent : Event
{
    [SerializeReference]
    private Condition condition;

    [SerializeReference]
    private Event eventInTrue;

    [SerializeReference]
    private Event eventInFalse;

    public override IEnumerator execute()
    {
        if (condition.Check())
        {
            return eventInTrue.execute();
        } else
        {
            return eventInFalse.execute();
        }
    }
}