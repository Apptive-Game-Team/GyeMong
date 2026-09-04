using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Creature.Player.Interface.Listener;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class PlayerShieldController : GaugeController, IShieldChangeListener
    {
        private PlayerChangeListenerCaller _changeListenerCaller;

        private float _shield;
        private float _maxShield;
        private void Start()
        {
            _changeListenerCaller = SceneContext.Character.changeListenerCaller;
            _changeListenerCaller.AddShieldChangeListener(this);
            _maxShield = SceneContext.Character.stat.HealthMax;
        }

        protected override float GetCurrentGauge()
        {
            return _shield;
        }

        protected override float GetMaxGauge()
        {
            return _maxShield;
        }

        public void OnChanged(float data)
        {
            _shield = data;
            UpdateSkillGauge();
        }
    
        protected override void Update() { } // Do not call base.Update()
        private void OnDestroy()
        {
            _changeListenerCaller?.RemoveShieldChangeListener(this);
        }

    }
}
