using System;
using System.Collections.Generic;
using UnityEngine;

namespace Visual.Effect
{
    [Serializable]
    public class EffectData
    {
        public string name;
        public int id;
        public GameObject effectPrefab;
    }

    [CreateAssetMenu(fileName = "EffectDataList", menuName = "ScriptableObject/EffectDataList")]
    public class EffectDataList : ScriptableObject
    {
        public List<EffectData> effectDataList;

        public EffectData GetEffectData(int id)
        {
            EffectData effectData = effectDataList.Find(x => x.id == id);
            if (effectData == null)
            {
                Debug.LogError("There's No Effect that has this id!");
                return null;
            }

            return effectData;
        }
    }
}