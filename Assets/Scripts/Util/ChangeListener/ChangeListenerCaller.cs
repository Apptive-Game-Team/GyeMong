using System.Collections.Generic;

namespace Util.ChangeListener
{
    public abstract class ChangeListenerCaller<T, TU> where T : IChangeListener<TU>
    {
        private List<T> _listeners = new List<T>();
        
        public void AddListener(T listener)
        {
            _listeners.Add(listener);
        }
        
        public void Call(TU data)
        {
            foreach (T listener in _listeners)
            {
                listener.OnChanged(data);
            }
        }
    }
}