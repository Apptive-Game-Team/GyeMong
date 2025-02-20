using System.Collections.Generic;
using UnityEngine;

namespace Util.ObjectCreator
{
    public class ObjectPool
    {
        private List<GameObject> _pool = new();
        private GameObject _prefab;
        private int _numOfObjects;
        public ObjectPool(int numOfObjects, GameObject prefab)
        {
            _numOfObjects = numOfObjects;
            _prefab = prefab;
            CreateObjects(numOfObjects);
        }

        public GameObject GetObject()
        {
            foreach (GameObject obj in _pool)
            {
                if (!obj.activeSelf)
                {
                    return obj;
                }
            }

            GameObject newObj = Object.Instantiate(_prefab);
            _pool.Add(newObj);
            return newObj;
        }
        
        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
        }
        
        private void CreateObjects(int numOfObjects)
        {
            for (int i = 0; i < numOfObjects; i++)
            {
                GameObject obj = Object.Instantiate(_prefab);
                obj.SetActive(false);
                _pool.Add(obj);
            }
        }
    }
}
