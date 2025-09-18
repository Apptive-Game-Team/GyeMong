using System;
using System.Collections;
using GyeMong.InputSystem;
using UnityEngine;

namespace GyeMong.EventSystem.Event.Input
{
    public abstract class ControlEvent : Event { }

    [Serializable]
    public class SetKeyInputEvent : ControlEvent
    {
        [SerializeField] public bool _isEnable;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            InputManager.Instance.SetActionState(_isEnable);
            SceneContext.Character.isControlled = !_isEnable;
            SceneContext.Character.StopPlayer(_isEnable);
            yield return null;
        }
    }

    [Serializable]
    public class SetActiveEvent : ControlEvent
    {
        enum ActiveState
        {
            ONCE_ACTIVE = 1,
            INFINITE_ACTIVE = -1,
            INACTIVE = 0
        }
        [SerializeField] private EventObject eventObject;
        [SerializeField] private ActiveState _activeState;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            // EventObject _object = (EventObject)eventObject;
            this.eventObject.SetTriggerLimitCounter((int) _activeState);
            return null;
        }
    }
}