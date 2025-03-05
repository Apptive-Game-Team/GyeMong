using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
    public class Stat
    {
        private Dictionary<string, float> _valueDict = new Dictionary<string, float>();
        
        [SerializeField] private float defaultValue;
        [SerializeField] private float flatValue;
        [SerializeField] private float percentValue;
        [SerializeField] private float finalPercentValue;

        public Stat()
        {
            _valueDict.Add("default", 0);
            _valueDict.Add("flat", 0);
            _valueDict.Add("percent", 0);
            _valueDict.Add("finalPercent", 0);
        }

        public void ModifyValue(string valueName, float value)
        {
            if (_valueDict.ContainsKey(valueName))
            {
                _valueDict[valueName] = value;
            }
            else
            {
                throw new ArgumentException($"Value '{valueName}' not found!");
            }
        }
        
        public float TotalValue
        {
            get
            {
                return (_valueDict["default"] * (1 + _valueDict["percent"]) + _valueDict["flat"]) * (1 + _valueDict["finalPercent"]);
            }
        }
    }

    [Serializable]
    public class StatComponent
    {
        //Stats to Dict. To find Stat by Name.
        private Dictionary<string, Stat> _statDict = new Dictionary<string, Stat>();
        
        public Stat healthMax;
        public Stat skillUsageGauge;
        public Stat attackPower;
        public Stat skillCoef;
        public Stat grazeMax;
        public Stat grazeGainOnGraze;
        public Stat grazeGainOnAttack;
        public Stat moveSpeed;
        public Stat dashSpeed;
        public Stat skillSpeed;
        public Stat dashDuration;
        public Stat dashDistance;
        public Stat dashCooldown;
        public Stat attackDelay;
        public Stat invincibilityDuration;
        
        //I Want to Get stat By name(string) And Modify by value(float)
        public Stat GetStat(string statName)
        {
            if (_statDict.TryGetValue(statName, out Stat stat))
            {
                return stat;
            }
            throw new ArgumentException($"Stat '{statName}' not found!");
        }

        //To do. change string into enum
        public void ModifyStat(string statName, string statTypeName, float amount)
        {
            if (_statDict.TryGetValue(statName, out Stat stat))
            {
                stat.ModifyValue(statTypeName, amount);
            }
            else
            {
                throw new ArgumentException($"Stat '{statName}' not found!");
            }
        }
    }
