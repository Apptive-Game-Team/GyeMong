using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class PlayerDashCooltimeController : GaugeController
    {
        private float _cooldownTime;
        private float _currentTime;

        private void Start()
        {
            _cooldownTime = SceneContext.Character.stat.DashCooldown;
            _currentTime = 0f;
        }

        protected override float GetCurrentGauge()
        {
            return _currentTime;
        }

        protected override float GetMaxGauge()
        {
            return _cooldownTime;
        }

        private new void Update()
        {
            if (_currentTime > 0f)
            {
                _currentTime -= Time.deltaTime;
                if (_currentTime < 0f) _currentTime = 0f;

                UpdateSkillGauge();
            }
        }

        public void StartCooldown()
        {
            _cooldownTime = SceneContext.Character.stat.DashCooldown;
            _currentTime = _cooldownTime;
            UpdateSkillGauge();
        }
    }
}
