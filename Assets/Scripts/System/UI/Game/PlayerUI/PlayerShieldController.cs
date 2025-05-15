using Creature.Player.Component;
using Creature.Player.Interface.Listener;
using playerCharacter;

namespace System.UI.Game.PlayerUI
{
    public class PlayerShieldController : GaugeController, IShieldChangeListener
    {
        private float _shield;
        private float _maxShield;
        private void Start()
        {
            PlayerCharacter.Instance.changeListenerCaller.AddShieldChangeListener(this);
            _maxShield = PlayerCharacter.Instance.stat.HealthMax;
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
