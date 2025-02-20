using System.Collections.Generic;
using UnityEngine;
using Util.ObjectCreator;

namespace Visual.Effect
{
    public class EffectCreator : SingletonObject<EffectCreator>
    {
        [SerializeField] EffectDataList effectDataList;

        private Dictionary<int, ObjectPool<EffectObject>> effectPools = new Dictionary<int, ObjectPool<EffectObject>>();
        
        [SerializeField] int poolSize = 10;

        public void InitialObjectPool(int id)
        {
            effectPools.Add(id, new ObjectPool<EffectObject>(1, effectDataList.GetEffectData(id).effectPrefab));
        }
        
        public void CreateEffect(int id, Vector3 pos)
        {
            if (!effectPools.ContainsKey(id)) 
                InitialObjectPool(id); // late Initialize

            effectPools[id].GetObject(pos).gameObject.SetActive(true);
        }

        public void CreateEffect(int id, Transform tr)
        {
            if (!effectPools.ContainsKey(id)) 
                InitialObjectPool(id); // late Initialize

            effectPools[id].GetObject(tr).gameObject.SetActive(true);
            // Instantiate(effectDataList.GetEffectData(id).effectPrefab, tr);
        }
    }
}
