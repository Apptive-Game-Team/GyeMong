using System.Collections;
using UnityEngine;

namespace GyeMong.EventSystem.Event.CinematicEvent
{
    public abstract class CameraEvent : Event { }

    public class CameraMove : CameraEvent
    {
        [SerializeField] private Vector3 destination;
        [SerializeField] private float speed;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return SceneContext.CameraManager.CameraMove(destination, speed);
        }
    }

    public class CameraFollowPlayer : CameraEvent
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            SceneContext.CameraManager.CameraFollow(SceneContext.Character.transform);
            yield return null;
        }
    }
    
    public class CameraZoomInOut : CameraEvent
    {
        [SerializeField] private float size;
        [SerializeField] private float duration;
        public void SetSize(float value)
        {
            size = value;
        }

        public void SetDuration(float value)
        {
            duration = value;
        }
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return SceneContext.CameraManager.CameraZoomInOut(size, duration);
        }
    }

    public class CameraZoomReset : CameraEvent
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            SceneContext.CameraManager.CameraZoomReset();
            return null;
        }
    }

    public class CameraShake : CameraEvent
    {
        [SerializeField] private float force;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            SceneContext.CameraManager.CameraShake(force);
            return null;
        }
    }
}