using GyeMong.GameSystem.Creature.Player.Interface.Listener;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class PlayerShieldController : GaugeController, IShieldChangeListener
    {
        private float _shield;
        private float _maxShield;
        private void Start()
        {
            SceneContext.Character.changeListenerCaller.AddShieldChangeListener(this);
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
    }
}
