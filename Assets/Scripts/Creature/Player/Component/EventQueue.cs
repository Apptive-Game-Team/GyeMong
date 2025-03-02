using System.Collections;
using System.Collections.Generic;

namespace Creature.Player.Component
{
    public class EventQueue : SingletonObject<EventQueue>
    {
        private LinkedList<global::Event> _eventQueue = new();
        private bool _isProcessing = false;

        public void AddEvent(global::Event e)
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
                global::Event e = _eventQueue.First.Value;
                _eventQueue.RemoveFirst();
                yield return StartCoroutine(e.Execute());
            }
            _isProcessing = false;
        }
    }
}
