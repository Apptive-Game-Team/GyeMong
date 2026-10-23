using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Creature.Player.Interface.Listener;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class PlayerHpController : GaugeController, IHpChangeListener
    {
        private float _hp;
        private float _maxHp;
        private PlayerChangeListenerCaller _caller;
        private void OnDestroy() => _caller?.RemoveHpChangeListener(this);

        private void Start()
        {
            _caller = SceneContext.Character.changeListenerCaller;
            _caller.AddHpChangeListener(this);
            _maxHp = SceneContext.Character.stat.HealthMax;
            OnChanged(SceneContext.Character.CurrentHp);
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
