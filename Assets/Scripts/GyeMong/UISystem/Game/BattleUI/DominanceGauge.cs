using GyeMong.EventSystem.Controller;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.BattleUI
{
    public class DominanceGauge : MonoBehaviour
    {
        [SerializeField] private Slider gaugeSlider;
        [SerializeField] private float maxGaugeValue = 100f;
        [SerializeField] private float pushAmountPerHit = 10f;

        private float currentGaugeValue = 0f;

        private void Start()
        {
            gaugeSlider.minValue = -maxGaugeValue;
            gaugeSlider.maxValue = maxGaugeValue;
            gaugeSlider.value = currentGaugeValue;
        }

        public void PlayerHitBoss(float damage)
        {
            currentGaugeValue += damage * pushAmountPerHit;
            currentGaugeValue = Mathf.Clamp(currentGaugeValue, -maxGaugeValue, maxGaugeValue);
            gaugeSlider.value = currentGaugeValue;
        }

        public void BossHitPlayer(float damage)
        {
            currentGaugeValue -= damage * pushAmountPerHit;
            currentGaugeValue = Mathf.Clamp(currentGaugeValue, -maxGaugeValue, maxGaugeValue);
            gaugeSlider.value = currentGaugeValue;
        }

        public float GetGaugeValue()
        {
            return currentGaugeValue;
        }

        public bool IsPlayerDominating()
        {
            return currentGaugeValue >= maxGaugeValue;
        }

        public bool IsBossDominating()
        {
            return currentGaugeValue <= -maxGaugeValue;
        }
    }
}
