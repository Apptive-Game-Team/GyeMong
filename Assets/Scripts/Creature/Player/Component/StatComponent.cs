using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


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
    }
