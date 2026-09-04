using System.Collections.Generic;
using UnityEngine;

namespace Util.ChangeListener
{
    public abstract class ChangeListenerCaller<T, TU> where T : IChangeListener<TU>
    {
        private readonly List<T> _listeners = new List<T>();

        public void AddListener(T listener)
        {
            if (listener == null || _listeners.Contains(listener)) return;
            _listeners.Add(listener);
        }

        public void RemoveListener(T listener)
        {
            _listeners.Remove(listener);
        }

        public void Call(TU data)
        {
            // 순회 중 등록/해제가 일어나도 안전하도록 뒤에서부터 돈다.
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                T listener = _listeners[i];

                if (IsDestroyed(listener))
                {
                    _listeners.RemoveAt(i);
                    continue;
                }

                try
                {
                    listener.OnChanged(data);
                }
                catch (System.Exception e)
                {
                    // 리스너 하나가 던진 예외가 나머지 리스너의 갱신을 막지 않도록 한다.
                    // 예외 자체는 삼키지 않고 콘솔에 남긴다.
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// 파괴된 MonoBehaviour 는 Unity 가 오버로드한 == 로만 판별된다.
        /// T 는 인터페이스라 일반 null 비교로는 걸러지지 않는다.
        /// </summary>
        private static bool IsDestroyed(T listener)
        {
            if (listener == null) return true;
            return listener is Object unityObject && unityObject == null;
        }
    }
}
