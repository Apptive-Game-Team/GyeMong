using System;
using UnityEngine;

namespace Creature.Player.Component
{
    [Serializable]
    public class Stat
    {
        [SerializeField] private float defaultValue;
        [SerializeField] private float flatValue;
        [SerializeField] private float percentValue;
        [SerializeField] private float finalPercentValue;

        public float GetValue()
        {
            return (defaultValue * (1 + (percentValue)) + flatValue) * (1 + finalPercentValue);
        }
    }

    [Serializable]
    public class StatComponent
    {
        public Stat healthMax;
        public Stat skillUsageGauge;
        public Stat attackPower;
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
    }
}