using UnityEngine;

namespace Gyemong.EventSystem.Event.Status
{
    public class EventStatus<T> : MonoBehaviour
    {
        [SerializeField] private T status;

        public void SetStatus(T status)
        {
            this.status = status;
        }
        public T GetStatus()
        {
            return status;
        }
    }
}
