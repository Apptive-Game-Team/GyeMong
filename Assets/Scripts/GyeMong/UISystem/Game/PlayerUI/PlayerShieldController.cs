using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Creature.Player.Interface.Listener;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class PlayerShieldController : GaugeController, IShieldChangeListener
    {
        private float _shield;
        private float _maxShield;
        private PlayerChangeListenerCaller _caller;
        private void OnDestroy() => _caller?.RemoveShieldChangeListener(this);

        private void Start()
        {
            _caller = SceneContext.Character.changeListenerCaller;
            _caller.AddShieldChangeListener(this);
            _maxShield = SceneContext.Character.stat.HealthMax;
            OnChanged(SceneContext.Character.curShield);
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
    }
}
