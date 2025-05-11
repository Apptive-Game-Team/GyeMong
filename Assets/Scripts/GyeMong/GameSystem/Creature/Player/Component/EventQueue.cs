using System.Collections;
using System.Collections.Generic;
using Util;

namespace GyeMong.GameSystem.Creature.Player.Component
{
    public class EventQueue : SingletonObject<EventQueue>
    {
        private LinkedList<global::GyeMong.EventSystem.Event.Event> _eventQueue = new();
        private bool _isProcessing = false;

        public void AddEvent(global::GyeMong.EventSystem.Event.Event e)
        {
            _eventQueue.AddLast(e);
            if (!_isProcessing)
            {
                StartCoroutine(ProcessEvent());
            }
        }

        private IEnumerator ProcessEvent()
        {
            _isProcessing = true;
            while (_eventQueue.Count > 0)
            {
                global::GyeMong.EventSystem.Event.Event e = _eventQueue.First.Value;
                _eventQueue.RemoveFirst();
                yield return StartCoroutine(e.Execute());
            }
            _isProcessing = false;
        }
    }
}
