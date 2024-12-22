using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CinematicEvent : Event { }

public class MoveCreatureEvent : CinematicEvent
{
    [SerializeField] private MonoBehaviour iControllable;
    [SerializeField] private Vector3 target;
    [SerializeField] private float speed;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        IControllable iControllable = (IControllable) this.iControllable;
        return iControllable.MoveTo(target, speed);
    }
}
