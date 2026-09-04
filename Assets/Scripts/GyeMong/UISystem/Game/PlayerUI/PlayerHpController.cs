using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Creature.Player.Interface.Listener;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class PlayerHpController : GaugeController, IHpChangeListener
    {
        private PlayerChangeListenerCaller _changeListenerCaller;

        private float _hp;
        private float _maxHp;
        private void Start()
        {
            _changeListenerCaller = SceneContext.Character.changeListenerCaller;
            _changeListenerCaller.AddHpChangeListener(this);
            _maxHp = SceneContext.Character.stat.HealthMax;
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
        private void OnDestroy()
        {
            _changeListenerCaller?.RemoveHpChangeListener(this);
        }

    }
}
