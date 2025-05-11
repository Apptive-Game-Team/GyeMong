using System;
using Gyemong.GameSystem.Creature.Player.Interface.Listener;
using Util.ChangeListener;

namespace Gyemong.GameSystem.Creature.Player.Component
{
    public class PlayerChangeListenerCaller
    {
        private class HpCaller : ChangeListenerCaller<IHpChangeListener, float>{}
        private class ShieldCaller : ChangeListenerCaller<IShieldChangeListener, float>{}
        private class SkillGaugeCaller : ChangeListenerCaller<ISkillGaugeChangeListener, float>{}
        public static event Action OnPlayerSpawned;
        public static event Action OnPlayerDied;

        private HpCaller _hpCaller = new();
        private ShieldCaller _shieldCaller = new();
        private SkillGaugeCaller _skillGaugeCaller = new();
        
        public void AddHpChangeListener(IHpChangeListener listener)
        {
            _hpCaller.AddListener(listener);
        }
        public void AddShieldChangeListener(IShieldChangeListener listener)
        {
            _shieldCaller.AddListener(listener);
        }
        public void AddSkillGaugeChangeListener(ISkillGaugeChangeListener listener)
        {
            _skillGaugeCaller.AddListener(listener);
        }
        
        public void CallHpChangeListeners(float hp)
        {
            _hpCaller.Call(hp);
        }
        public void CallShieldChangeListeners(float amount)
        {
            _shieldCaller.Call(amount);
        }
        
        public void CallSkillGaugeChangeListeners(float skillGauge)
        {
            _skillGaugeCaller.Call(skillGauge);
        }
        
        public void CallPlayerDied()
        {
            OnPlayerDied?.Invoke();
        }
        
        public void CallPlayerSpawned()
        {
            OnPlayerSpawned?.Invoke();
        }
    }
}