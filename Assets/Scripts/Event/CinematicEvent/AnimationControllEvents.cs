using System.Collections;
using UnityEngine;

public abstract class AnimationControllEvent : CinematicEvent
{
    [SerializeField] protected Animator _animator;
}

public class StopAnimatiorEvent : AnimationControllEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        Debug.Log(_animator);
        _animator.enabled = false;
        return null;
    }
}

public class StartAnimatiorEvent : AnimationControllEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        _animator.enabled = true;
        return null;
    }
}