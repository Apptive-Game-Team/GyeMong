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
        private Slider _gaugeSlider;

        private float _leftHp;
        private float _rightHp;
        private float _leftMaxHp;
        private float _rightMaxHp;

        private float adjustConst;

        [SerializeField] private Creature boss;
        private void Awake()
        {
            _gaugeSlider = GetComponent<Slider>();
        }
        private void Start()
        {
            adjustConst = boss.MaxHp / SceneContext.Character.stat.HealthMax;
        }
        private void Update()
        {
            UpdateDominanceGauge(SceneContext.Character.CurrentHp * adjustConst, SceneContext.Character.stat.HealthMax * adjustConst, boss.CurrentHp, boss.MaxHp);
        }

        public void UpdateDominanceGauge(float leftHp, float leftMaxHp, float rightHp, float rightMaxHp)
        {
            _leftHp = Mathf.Max(0, leftHp);
            _rightHp = Mathf.Max(0, rightHp);
            _leftMaxHp = leftMaxHp;
            _rightMaxHp = rightMaxHp;

            UpdateGaugeVisual();
        }

        private void UpdateGaugeVisual()
        {
            float leftRatio = _leftHp / _leftMaxHp;
            float rightRatio = _rightHp / _rightMaxHp;

            if (leftRatio + rightRatio == 0f)
            {
                _gaugeSlider.value = 0f;
                return;
            }

            float dominance = (leftRatio - rightRatio) / (leftRatio + rightRatio);

            _gaugeSlider.value = Mathf.Clamp(dominance, -1f, 1f);
        }
    }
}
