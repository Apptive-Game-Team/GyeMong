using System.Collections;
using UnityEngine;

public abstract class AnimationControllEvent : CinematicEvent
{
    [SerializeField] protected Animator _animator;
}

public class StopAnimatorEvent : AnimationControllEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        _animator.enabled = false;
        return null;
    }
}

public class StartAnimatorEvent : AnimationControllEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        _animator.enabled = true;
        return null;
    }
}

public class SetAnimatorParameter : AnimationControllEvent
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private int _value;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        _animator.SetFloat(_name, _value);
        return null;
    }
}