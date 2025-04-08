using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util.ObjectCreator
{
    public class ObjectPool<T> where T : Component
    {
        private List<T> _pool = new();
        private GameObject _prefab;

        private void OnSceneUnloading(Scene obj)
        {
            _pool.Clear();
        }
        ~ObjectPool()
        {
            PortalManager.sceneUnloading -= OnSceneUnloading;
        }

        public ObjectPool(int numOfObjects, GameObject prefab, Transform parent = null)
        {
            _prefab = prefab;
            if (_prefab.GetComponent<T>() == null)
            {
                _prefab.AddComponent<T>();
            }
            CreateObjects(numOfObjects, parent);
            PortalManager.sceneUnloading += OnSceneUnloading;
        }

        public T GetObject()
        {
            T result = GetInactiveObject();
            if (result != null)
                return result;

            GameObject newGameObject = Object.Instantiate(_prefab);
            result = newGameObject.GetComponent<T>();
            
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
            foreach (T obj in _pool)
            {
                if (!obj.gameObject.activeInHierarchy)
                {
                    return obj;
                }
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
                T obj = gameObject.GetComponent<T>();
                @gameObject.SetActive(false);
                _pool.Add(obj);
            }
        }
    }
}
