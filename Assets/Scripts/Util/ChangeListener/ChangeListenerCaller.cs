using System.Collections.Generic;

namespace Util.ChangeListener
{
    public abstract class ChangeListenerCaller<T, TU> where T : IChangeListener<TU>
    {
        private readonly List<T> _listeners = new List<T>();

        public void AddListener(T listener)
        {
            if (!IsMissing(listener) && !_listeners.Contains(listener)) _listeners.Add(listener);
        }

        public void RemoveListener(T listener) => _listeners.Remove(listener);

        private static bool IsMissing(T listener) => ReferenceEquals(listener, null) ||
            (listener is UnityEngine.Object obj && obj == null);

        public void Call(TU data)
        {
            // New listeners start next notification; listeners removed during a callback are skipped.
            foreach (T listener in _listeners.ToArray())
            {
                if (IsMissing(listener)) { _listeners.Remove(listener); continue; }
                if (_listeners.Contains(listener)) listener.OnChanged(data);
            }
        }
    }
}
