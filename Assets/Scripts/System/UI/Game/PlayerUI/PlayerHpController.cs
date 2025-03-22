using Creature.Player.Interface.Listener;
using playerCharacter;

namespace System.UI.Game.PlayerUI
{
    public class PlayerHpController : GaugeController, IHpChangeListener
    {
        private float _hp;
        private float _maxHp;
        private void Start()
        {
            PlayerCharacter.Instance.changeListenerCaller.AddHpChangeListener(this);
            _maxHp = PlayerCharacter.Instance.stat.healthMax.GetValue();
        }

        protected override float GetCurrentGauge()
        {
            return _hp;
        }

        protected override float GetMaxGauge()
        {
            return _maxHp;
        }

        public void OnChanged(float data)
        {
            _hp = data;
            UpdateSkillGauge();
        }
    
        protected override void Update() { } // Do not call base.Update()
    }
}
