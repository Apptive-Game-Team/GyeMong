using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.BattleUI
{
    public class DominanceGauge : MonoBehaviour
    {
        [SerializeField] private Boss boss;
        private Slider _gaugeSlider;

        private float _playerMaxHp;
        private float _bossMaxHp;
        private float _totalDominanceRange;

        private float _dominanceValue;

        public float PlayerDamageMultiplier { get; private set; } = 1f;
        public float BossDamageMultiplier { get; private set; } = 1f;

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
            float adjustDamage = damage * PlayerDamageMultiplier;
            _dominanceValue = Mathf.Clamp(_dominanceValue - adjustDamage/10, 0f, _totalDominanceRange);
            UpdateGaugeVisual();
        }
        public void ApplyDamageToBoss(float damage)
        {
            float adjustDamage = damage * BossDamageMultiplier;
            _dominanceValue = Mathf.Clamp(_dominanceValue + adjustDamage, 0f, _totalDominanceRange);
            UpdateGaugeVisual();
        }
        private void UpdateGaugeVisual()
        {
            float ratio = _dominanceValue / _totalDominanceRange;
            _gaugeSlider.value = ratio;

            UpdateDamageMultipliers(ratio);
        }
        private void UpdateDamageMultipliers(float ratio)
        {
            PlayerDamageMultiplier = 1f;
            BossDamageMultiplier = 1f;

            if (ratio <= 0.25f)
            {
                BossDamageMultiplier = 1.5f;
            }
            else if (ratio >= 0.75f)
            {
                PlayerDamageMultiplier = 1.5f;
            }
            else if(ratio >= 1f)
            {
                boss.Die();
            }
        }
    }
}
