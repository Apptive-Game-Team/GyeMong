using System.Collections.Generic;
using UnityEngine;

namespace System.Game.Object.Persisted
{
    public class PersistedGameObject : MonoBehaviour
    {
        [SerializeField] [Tooltip("Reference It's Own Prefab")] private GameObject _prefab;
        [SerializeField] private string _uniqueId;
        
        private List<ComponentData> _componentData = new();
        
        public (string uniqueId, GameObject prefab, Vector3 position, List<ComponentData> componentData) Save()
        {
            SaveComponents();
            return (_uniqueId, _prefab, transform.position, _componentData);
        }
        
        public void Load((string uniqueId, GameObject prefab, Vector3 position, List<ComponentData> componentData) data)
        {
            _uniqueId = data.uniqueId;
            _prefab = data.prefab;
            transform.position = data.position;
            _componentData = data.componentData;
            LoadComponents();
        }
        
        private void SaveComponents()
        {
            _componentData.Clear();
            var components = GetComponentsInChildren<IPersistedComponent>();
            foreach (IPersistedComponent component in components)
            {
                _componentData.Add(component.Save());
            }
        }
        
        private void LoadComponents()
        {
            int i = 0;
            var components = GetComponentsInChildren<IPersistedComponent>();
            foreach (var component in components)
            {
                component.Load(_componentData[i]);
                i++;
            }
        }
    }
}
