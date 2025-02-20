using System.Collections.Generic;
using UnityEngine;

namespace Util.ObjectCreator
{
    public class ObjectPool<T> where T : Component
    {
        private List<T> _pool = new();
        private GameObject _prefab;
        public ObjectPool(int numOfObjects, GameObject prefab)
        {
            _prefab = prefab;
            if (_prefab.GetComponent<T>() == null)
            {
                _prefab.AddComponent<T>();
            }
            CreateObjects(numOfObjects);
        }

        public T GetObject()
        {
            foreach (T obj in _pool)
            {
                if (!obj.gameObject.activeInHierarchy)
                {
                    return obj;
                }
            }

            GameObject newGameObject = Object.Instantiate(_prefab);
            T newObj = newGameObject.GetComponent<T>();
            
            _pool.Add(newObj);
            return newObj;
        }
        
        public void ReturnObject(T obj)
        {
            obj.gameObject.SetActive(false);
        }
        
        private void CreateObjects(int numOfObjects)
        {
            for (int i = 0; i < numOfObjects; i++)
            {
                GameObject @gameObject = Object.Instantiate(_prefab);
                T obj = gameObject.GetComponent<T>();
                @gameObject.SetActive(false);
                _pool.Add(obj);
            }
        }
    }
}
