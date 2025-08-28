using GyeMong.GameSystem.Creature.Player.Interface.Listener;
using UnityEngine;
using Util.ChangeListener;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class PlayerDashCooltimeController : GaugeController, IDashListener
    {
        private float _cooldownTime;
        private float _currentTime;

        private void Start()
        {
            SceneContext.Character.changeListenerCaller.AddDashListener(this);

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

        public void OnDashUsed(float cooldown)
        {
            _cooldownTime = cooldown;
            _currentTime = _cooldownTime;
            UpdateSkillGauge();
        }

        public void OnChanged(float changedValue)
        {
            _cooldownTime = changedValue;
            UpdateSkillGauge();
        }
    }
}