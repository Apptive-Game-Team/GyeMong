using System.Collections.Generic;
using UnityEngine;

namespace System.Game.Object.Persisted
{
    public class PersistedGameObjectManager : SingletonObject<PersistedGameObjectManager>
    {
        private Dictionary<string, (GameObject prefab, Vector3 position, List<ComponentData> componentDatas)> _persistedGameObjects = new();
        
        public void SavePersistedGameObject(PersistedGameObject persistedGameObject, Vector3 position)
        {
            var datas = persistedGameObject.Save();
            _persistedGameObjects[datas.uniqueId] = (datas.prefab, position, datas.componentData);
        }
        
    }
}
