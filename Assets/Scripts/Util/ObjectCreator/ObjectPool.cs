using System.Collections.Generic;
using UnityEngine;

namespace Util.ObjectCreator
{
    public class ObjectPool<T> where T : Component
    {
        private List<T> _pool = new();
        private GameObject _prefab;
        private readonly Transform _parent;

        public ObjectPool(int numOfObjects, GameObject prefab, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            CreateObjects(numOfObjects, parent);
        }

        public T GetObject()
        {
            T result = GetInactiveObject();
            if (result != null)
                return result;

            GameObject newGameObject = Object.Instantiate(_prefab, _parent);
            result = GetOrAddComponent(newGameObject);
            
            _pool.Add(result);
            return result;
        }
        
        public T GetObject(Vector3 position)
        {
            T obj = GetObject();
            obj.transform.position = position;
            return obj;
        }
        
        public T GetObject(Transform transform)
        {
            T obj = GetObject(transform.position);
            obj.transform.parent = transform;
            return obj;
        }
        
        public T GetInactiveObject()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                T obj = _pool[i];
                if (obj == null)
                {
                    _pool.RemoveAt(i);
                    i--;
                    continue;
                }
                if (!obj.gameObject.activeSelf) return obj;
            }

            return null;
        }

        
        public void ReturnObject(T obj)
        {
            obj.gameObject.SetActive(false);
        }
        
        private void CreateObjects(int numOfObjects, Transform parent = null)
        {
            for (int i = 0; i < numOfObjects; i++)
            {
                GameObject @gameObject = Object.Instantiate(_prefab, parent);
                T obj = GetOrAddComponent(@gameObject);
                @gameObject.SetActive(false);
                _pool.Add(obj);
            }
        }

        private static T GetOrAddComponent(GameObject instance)
        {
            var component = instance.GetComponent<T>();
            return component != null ? component : instance.AddComponent<T>();
        }
    }
}
