using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectEvent : Event
{
    
}

[Serializable]
public class HurtEffectEvent : EffectEvent
{
    public float amount;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.HurtEffect(amount);
    }
}

[Serializable]
public class ShakeCameraEvent : EffectEvent
{
    public float time;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.ShakeCamera(time);
    }
}
public class ParticleEvent : EffectEvent
{

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        ParticleSystem particleSystem = eventObject.GetComponentInChildren<ParticleSystem>();
        particleSystem.Play();
        yield return new WaitForSeconds(0.1f);
        particleSystem.Stop();
    }
}

[Serializable]
public class FadeInEvent : EffectEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.FadeIn();
    }
}

[Serializable]
public class FadeOutEvent : EffectEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.FadeOut();
    }
}
