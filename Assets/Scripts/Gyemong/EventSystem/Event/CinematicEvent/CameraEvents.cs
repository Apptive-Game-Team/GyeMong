using System.Collections;
using Gyemong.GameSystem.Creature.Player;
using UnityEngine;
using Visual.Camera;

namespace Gyemong.EventSystem.Event.CinematicEvent
{
    public abstract class CameraEvent : Event { }

    public class CameraMove : CameraEvent
    {
        [SerializeField] private Vector3 destination;
        [SerializeField] private float speed;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return CameraManager.Instance.CameraMove(destination, speed);
        }
    }

    public class CameraFollow : CameraEvent
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            CameraManager.Instance.CameraFollow(PlayerCharacter.Instance.gameObject.transform);
            return null;
        }
    }

    public class CameraZoomInOut : CameraEvent
    {
        [SerializeField] private float size;
        [SerializeField] private float duration;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return CameraManager.Instance.CameraZoomInOut(size, duration);
        }
    }

    public class CameraZoomReset : CameraEvent
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            CameraManager.Instance.CameraZoomReset();
            return null;
        }
    }

    public class CameraShake : CameraEvent
    {
        [SerializeField] private float force;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            CameraManager.Instance.CameraShake(force);
            return null;
        }
    }
}