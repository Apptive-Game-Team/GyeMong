using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Creature.Player.Component
{
    public enum StatType
    {
        DEFAULT,
        HEALTH_MAX,
        SKILL_COST,
        ATTACK_POWER,
        SKILL_COEF,
        GRAZE_MAX,
        GRAZE_GAIN_ON_GRAZE,
        GRAZE_GAIN_ON_ATTACK,
        MOVE_SPEED,
        DASH_SPEED,
        SKILL_SPEED,
        DASH_DURATION,
        DASH_DISTANCE,
        DASH_COOLDOWN,
        ATTACK_DELAY,
        INVINCIBILITY_DURATION
    }

    public enum StatValueType
    {
        DEFAULT,
        DEFAULT_VALUE,
        FLAT_VALUE,
        PERCENT_VALUE,
        FINAL_PERCENT_VALUE,
    }

    [Serializable]
    public class StatEntry
    {
        public StatType statType;
        public Stat stat;
        
        public StatEntry(StatType statType, Stat stat)
        {
            this.statType = statType;
            this.stat = stat;
        }
    }

    [Serializable]
    public class ValueEntry
    {
        public StatValueType valueType;
        public float value;

        public ValueEntry(StatValueType valueType, float value)
        {
            this.valueType = valueType;
            this.value = value;
        }
        
        public static ValueEntry operator +(ValueEntry entry, float value)
        {
            return new ValueEntry(entry.valueType, entry.value + value);
        }
        public static ValueEntry operator -(ValueEntry entry, float value)
        {
            return new ValueEntry(entry.valueType, entry.value - value);
        }
        public static ValueEntry operator *(ValueEntry entry, float value)
        {
            return new ValueEntry(entry.valueType, entry.value * value);
        }
        public static ValueEntry operator /(ValueEntry entry, float value)
        {
            if(value == 0) throw new DivideByZeroException();
            return new ValueEntry(entry.valueType, entry.value / value);
        }
    }
    
    [Serializable]
    public class Stat
    {
        [SerializeField] private List<ValueEntry> valueEntries = new List<ValueEntry>();
        
        public Stat()
        {
            valueEntries.Add(new ValueEntry(StatValueType.DEFAULT_VALUE, 0));
            valueEntries.Add(new ValueEntry(StatValueType.FLAT_VALUE, 0));
            valueEntries.Add(new ValueEntry(StatValueType.PERCENT_VALUE, 0));
            valueEntries.Add(new ValueEntry(StatValueType.FINAL_PERCENT_VALUE, 0));
        }

        public void SetValue(StatValueType valueType, float value)
        {
            ValueEntry valueEntry = valueEntries.Find(x => x.valueType == valueType);
            valueEntry.value = value;
        }

        public float GetValue(StatValueType valueType)
        {
            ValueEntry valueEntry = valueEntries.Find(x => x.valueType == valueType);
            return valueEntry.value;
        }
        
        public float TotalValue =>
            (GetValue(StatValueType.DEFAULT_VALUE) * GetValue(StatValueType.PERCENT_VALUE) + GetValue(StatValueType.FLAT_VALUE)) 
            * (1 + GetValue(StatValueType.FINAL_PERCENT_VALUE));
    }

    [Serializable]
    public class StatComponent
    {
        
        [SerializeField] private List<StatEntry> statEntries = new List<StatEntry>();

        public float HealthMax => GetStatValue(StatType.HEALTH_MAX);
        public float SkillCost => GetStatValue(StatType.SKILL_COST);
        public float AttackPower => GetStatValue(StatType.ATTACK_POWER);
        public float SkillCoef => GetStatValue(StatType.SKILL_COEF);
        public float GrazeMax => GetStatValue(StatType.GRAZE_MAX);
        public float GrazeGainOnGraze => GetStatValue(StatType.GRAZE_GAIN_ON_GRAZE);
        public float GrazeGainOnAttack => GetStatValue(StatType.GRAZE_GAIN_ON_ATTACK);
        public float MoveSpeed => GetStatValue(StatType.MOVE_SPEED);
        public float DashSpeed  => GetStatValue(StatType.DASH_SPEED);
        public float SkillSpeed  => GetStatValue(StatType.SKILL_SPEED);
        public float DashDuration => GetStatValue(StatType.DASH_DURATION);
        public float DashDistance => GetStatValue(StatType.DASH_DISTANCE);
        public float DashCooldown => GetStatValue(StatType.DASH_COOLDOWN);
        public float AttackDelay => GetStatValue(StatType.ATTACK_DELAY);
        public float InvincibilityDuration => GetStatValue(StatType.INVINCIBILITY_DURATION);
        
        public StatComponent()
        {
            statEntries.Add(new StatEntry(StatType.HEALTH_MAX, new Stat()));
            statEntries.Add(new StatEntry(StatType.SKILL_COST, new Stat()));
            statEntries.Add(new StatEntry(StatType.ATTACK_POWER, new Stat()));
            statEntries.Add(new StatEntry(StatType.SKILL_COEF, new Stat()));
            statEntries.Add(new StatEntry(StatType.GRAZE_MAX, new Stat()));
            statEntries.Add(new StatEntry(StatType.GRAZE_GAIN_ON_GRAZE, new Stat()));
            statEntries.Add(new StatEntry(StatType.GRAZE_GAIN_ON_ATTACK, new Stat()));
            statEntries.Add(new StatEntry(StatType.MOVE_SPEED, new Stat()));
            statEntries.Add(new StatEntry(StatType.DASH_SPEED, new Stat()));
            statEntries.Add(new StatEntry(StatType.SKILL_SPEED, new Stat()));
            statEntries.Add(new StatEntry(StatType.DASH_DURATION, new Stat()));
            statEntries.Add(new StatEntry(StatType.DASH_COOLDOWN, new Stat()));
            statEntries.Add(new StatEntry(StatType.ATTACK_DELAY, new Stat()));
            statEntries.Add(new StatEntry(StatType.INVINCIBILITY_DURATION, new Stat()));
        }

        private float GetStatValue(StatType statType)
        {
            StatEntry statEntry = statEntries.Find(x => x.statType == statType);
            return statEntry.stat.TotalValue;
        }

        public void SetStatValue(StatType statType, StatValueType statValueType, float value)
        {
            Stat stat = statEntries.Find(x => x.statType == statType).stat;
            stat.SetValue(statValueType, value);
        }
    }
}