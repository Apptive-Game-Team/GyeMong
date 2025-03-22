using System.Collections.Generic;
using System.Data;
using System.Game.Portal;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace System.Game.Object.Persisted
{
    [Serializable]
    public class PersistedGameObjectDatas
    {
        [SerializeField] private List<PersistedGameObjectData> _datas = new();

        public PersistedGameObjectDatas(Dictionary<string, PersistedGameObjectData> datas)
        {
            _datas = datas.Values.ToList();
        }

        public PersistedGameObjectDatas()
        {
            throw new NotImplementedException();
        }

        public static implicit operator Dictionary<string, PersistedGameObjectData>(PersistedGameObjectDatas datas)
        {
            return datas._datas.ToDictionary(kv => kv.uniqueId, kv => kv);
        }
    }
    
    public class PersistedGameObjectManager : SingletonObject<PersistedGameObjectManager>
    {
        private Dictionary<string, PersistedGameObjectData> _persistedGameObjects = new();
        public static string PERSISTED_DATA_FILE = "persistedGameObjects";
        public void Save()
        {
            DataManager.Instance.SaveSection(new PersistedGameObjectDatas(_persistedGameObjects), PERSISTED_DATA_FILE);
        }
        public void SetPersistedGameObjects(PersistedGameObjectDatas persistedGameObjects)
        {
            _persistedGameObjects = persistedGameObjects;
            ScanPersistedGameObjects();
            PlacePersistedGameObjects();
        }
        
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            ScanPersistedGameObjects();
            PlacePersistedGameObjects();
        }

        private void ScanPersistedGameObjects()
        {
            PersistedGameObject[] objects = FindObjectsByType<PersistedGameObject>(FindObjectsSortMode.None);
            foreach (PersistedGameObject persistedGameObject in objects)
            {
                if (_persistedGameObjects.ContainsKey(persistedGameObject.UniqueId))
                { // this GameObject's Data is already saved
                    print("Applying Data to PersistedGameObject: " + persistedGameObject.UniqueId);
                    ApplyDataToPersistedGameObject(persistedGameObject, SceneManager.GetActiveScene());
                }
            }
        }
        private void PlacePersistedGameObjects()
        {
            HashSet<string> existingObjectIds = new HashSet<string>();
            foreach (var obj in FindObjectsByType<PersistedGameObject>(FindObjectsSortMode.None))
            {
                existingObjectIds.Add(obj.UniqueId);
            }

            foreach (var data in _persistedGameObjects.Values)
            {
                if (SceneManager.GetActiveScene().name.Equals(data.sceneName) && !existingObjectIds.Contains(data.uniqueId))
                {
                    GameObject go = Instantiate(data.prefab, data.position, Quaternion.identity);
                    PersistedGameObject persistedGameObject = go.GetComponent<PersistedGameObject>();
                    print("Placing PersistedGameObject: " + persistedGameObject.UniqueId);
                    persistedGameObject.Load(data);
                }
            }
        }
        
        private void Start()
        {
            ScanPersistedGameObjects();
            PlacePersistedGameObjects();
            SceneManager.sceneLoaded += OnSceneLoaded;
            PortalManager.sceneUnloading += OnSceneUnloading;
        }

        private void OnSceneUnloading(Scene arg0)
        {
            // Save all PersistedGameObjects
            foreach (var obj in FindObjectsByType<PersistedGameObject>(FindObjectsSortMode.None))
            {
                print("Saving PersistedGameObject: " + obj.UniqueId);
                SavePersistedGameObject(obj);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            PortalManager.sceneUnloading -= OnSceneUnloading;
        }

        private void ApplyDataToPersistedGameObject(PersistedGameObject persistedGameObject, Scene scene)
        {
            var data = _persistedGameObjects[persistedGameObject.UniqueId];
            if (scene.name.Equals(data.sceneName))
            {
                persistedGameObject.Load(data);
            }
            else
            {
                Destroy(persistedGameObject.gameObject);
            }
        }
        
        private void SavePersistedGameObject(PersistedGameObject persistedGameObject)
        {
            var datas = persistedGameObject.Save();
            _persistedGameObjects[persistedGameObject.UniqueId] = datas;
        }
       
    }
}
