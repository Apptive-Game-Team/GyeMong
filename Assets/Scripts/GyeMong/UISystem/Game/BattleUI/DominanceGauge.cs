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
            StartCoroutine(Init());
        }
        private IEnumerator Init()
        {
            yield return null;

            _playerMaxHp = SceneContext.Character.stat.HealthMax;
            _bossMaxHp = boss.MaxHp;
            _totalDominanceRange = _playerMaxHp + _bossMaxHp;
            _dominanceValue = _playerMaxHp;

            UpdateGaugeVisual();
        }
        public void ApplyDamageToPlayer(float damage)
        {
            _dominanceValue = Mathf.Clamp(_dominanceValue - damage, 0f, _totalDominanceRange);
            UpdateGaugeVisual();
        }
        public void ApplyDamageToBoss(float damage)
        {
            _dominanceValue = Mathf.Clamp(_dominanceValue + damage*10, 0f, _totalDominanceRange);
            UpdateGaugeVisual();
        }
        private void UpdateGaugeVisual()
        {
            _gaugeSlider.value = _dominanceValue / _totalDominanceRange;
        }
    }
}
