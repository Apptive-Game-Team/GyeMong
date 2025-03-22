using System;
using System.Collections;
using UnityEngine;

public abstract class EffectEvent : Event { }

[Serializable]
public class HurtEffectEvent : EffectEvent
{
    public float amount;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.HurtEffect(amount);
    }
}
public class ParticleEvent : EffectEvent
{
    private ParticleSystem _particleSystem;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        if (_particleSystem == null)
        {
            _particleSystem = eventObject.GetComponentInChildren<ParticleSystem>();
        }
        
        _particleSystem.Play();
        yield return new WaitForSeconds(0.1f);
        _particleSystem.Stop();
    }
}

public class ParticleAToBEvent : EffectEvent
{
    
    [SerializeField] internal Vector3 _startPosition;
    [SerializeField] internal Vector3 _endPosition;
    [SerializeField] private float _speed;
    [SerializeField] private float _size;
    [SerializeField] private float _rateOverTime;
    
    private ParticleSystem _particleSystem;
    private ParticleSystem.ShapeModule _shape;
    private ParticleSystem.MainModule _main;
    private ParticleSystem.EmissionModule _emission;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        if (_particleSystem == null)
        {
            _particleSystem = eventObject.GetComponentInChildren<ParticleSystem>();
            _shape = _particleSystem.shape;
            _main = _particleSystem.main;
            _emission = _particleSystem.emission;
        }
        
        _shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
        
        float signedAngle = Mathf.Atan2(_endPosition.x - _startPosition.x, _endPosition.y - _startPosition.y) * Mathf.Rad2Deg;
        float distance = Vector3.Distance(_startPosition, _endPosition);
        
        _emission.rateOverTime = new ParticleSystem.MinMaxCurve(0, _rateOverTime);
        _main.startSpeed = _speed;
        _main.startSize = _size;
        _shape.rotation = Vector3.back * signedAngle;
        _shape.position = _startPosition - _particleSystem.transform.position;
        _main.startLifetime = distance / _main.startSpeed.constant;
        _particleSystem.Play();
        yield return new WaitForSeconds(10/_rateOverTime);
        _particleSystem.Stop();
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

public class FadeInFirstEvent : EffectEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.FadeInFirst();
    }
}

public class ChangeToBlackScreenEvent : EffectEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.ChangeToBlackScreen();
    }
}

