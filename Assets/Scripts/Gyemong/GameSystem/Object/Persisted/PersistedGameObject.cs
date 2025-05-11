using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gyemong.GameSystem.Object.Persisted
{
    [Serializable]
    public struct PersistedGameObjectData
    {
        public string uniqueId;
        public GameObject prefab;
        public bool isActive;
        public string sceneName;
        public Vector3 position;
        public List<ComponentData> componentDatas;

        public PersistedGameObjectData(string uniqueId, GameObject prefab, bool isActive, string sceneName, Vector3 position, List<ComponentData> componentDatas)
        {
            this.uniqueId = uniqueId; this.prefab = prefab; this.isActive = isActive; this.sceneName = sceneName; this.position = position; this.componentDatas = componentDatas;
        }
    }
    
    public class PersistedGameObject : MonoBehaviour
    {
        [SerializeField] [Tooltip("Reference It's Own Prefab")] private GameObject _prefab;
        [SerializeField] private string _uniqueId;
        public string UniqueId => _uniqueId;
        
        private List<ComponentData> _componentDatas = new();
        
        public PersistedGameObjectData Save()
        {
            SaveComponents();
            return new PersistedGameObjectData(
                _uniqueId, 
                _prefab, 
                gameObject.activeSelf, 
                SceneManager.GetActiveScene().name, 
                transform.position, 
                _componentDatas
                );
        }
        
        public void Load(PersistedGameObjectData data)
        {
            _uniqueId = data.uniqueId;
            _prefab = data.prefab;
            transform.position = data.position;
            _componentDatas = data.componentDatas;
            LoadComponents();
            gameObject.SetActive(data.isActive);
        }
        
        private void SaveComponents()
        {
            _componentDatas.Clear();
            var components = GetComponentsInChildren<IPersistedComponent>();
            foreach (IPersistedComponent component in components)
            {
                _componentDatas.Add(component.Save());
            }
        }
        
        private void LoadComponents()
        {
            int i = 0;
            var components = GetComponentsInChildren<IPersistedComponent>();
            foreach (var component in components)
            {
                component.Load(_componentDatas[i]);
                i++;
            }
        }
    }
}
