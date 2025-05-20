using GyeMong.EventSystem.Controller;
using GyeMong.GameSystem.Creature;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.BattleUI
{
    public class DominanceGauge : MonoBehaviour
    {
        [SerializeField] private Creature boss;
        private Slider _gaugeSlider;

        private float _playerMaxHp;
        private float _bossMaxHp;
        private float _totalDominanceRange;

        private float _dominanceValue;

        private void Awake()
        {
            _gaugeSlider = GetComponent<Slider>();
        }

        private void Start()
        {
            _playerMaxHp = SceneContext.Character.stat.HealthMax;
            _bossMaxHp = boss.MaxHp;
            _totalDominanceRange = _playerMaxHp + _bossMaxHp;
            _dominanceValue = 0f;
            UpdateGaugeVisual();
        }

        public void ApplyDamageToPlayer(float damage)
        {
            float delta = (damage / _totalDominanceRange) * 2f;
            _dominanceValue = Mathf.Clamp(_dominanceValue - delta, -1f, 1f);
            UpdateGaugeVisual();
        }

        public void ApplyDamageToBoss(float damage)
        {
            float delta = (damage / _totalDominanceRange) * 2f;
            _dominanceValue = Mathf.Clamp(_dominanceValue + delta, -1f, 1f);
            UpdateGaugeVisual();
        }

        private void UpdateGaugeVisual()
        {
            _gaugeSlider.value = _dominanceValue;
        }
    }

}
