using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace System.Game.Object.Persisted
{
    public class PersistedGameObjectManager : SingletonObject<PersistedGameObjectManager>
    {
        private Dictionary<string, PersistedGameObjectData> _persistedGameObjects = new();
        
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
                    ApplyDataToPersistedGameObject(persistedGameObject, SceneManager.GetActiveScene());
                }
                else
                { // this GameObject is not saved yet
                    SavePersistedGameObject(persistedGameObject);
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
                if (SceneManager.GetActiveScene().name.Equals(data.sceneName))
                {
                    if (existingObjectIds.Contains(data.uniqueId))
                    {
                        continue;
                    }

                    GameObject go = Instantiate(data.prefab, data.position, Quaternion.identity);
                    PersistedGameObject persistedGameObject = go.GetComponent<PersistedGameObject>();
                    persistedGameObject.Load(data);
                }
            }
        }
        
        private void Start()
        {
            ScanPersistedGameObjects();
            PlacePersistedGameObjects();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            _persistedGameObjects[datas.uniqueId] = datas;
        }
       
    }
}
