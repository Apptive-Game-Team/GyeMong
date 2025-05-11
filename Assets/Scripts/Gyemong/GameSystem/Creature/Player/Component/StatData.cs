using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gyemong.GameSystem.Creature.Player.Component
{
    [Serializable]
    public class StatDataEntry
    {
        public StatType statType;
        public float value;
    }

    [CreateAssetMenu(fileName = "StatData",menuName ="ScriptableObject/StatData")]
    public class StatData : ScriptableObject
    {
        [SerializeField] private List<StatDataEntry> statDataList;

        public StatComponent GetStatComp()
        {
            StatComponent statComponent = new StatComponent();
        
            foreach (var statData in statDataList)
            {
                statComponent.SetStatValue(statData.statType, StatValueType.DEFAULT_VALUE, statData.value);
            }
            
            return statComponent;
        }
    }
}