using Creature.Player.Interface.Listener;
using Util.ChangeListener;

namespace Creature.Player.Component
{
    public class PlayerChangeListenerCaller
    {
        private class HpCaller : ChangeListenerCaller<IHpChangeListener, float>{}
        private class SkillGaugeCaller : ChangeListenerCaller<ISkillGaugeChangeListener, float>{}

        private HpCaller _hpCaller = new();
        private SkillGaugeCaller _skillGaugeCaller = new();
        
        public void AddHpChangeListener(IHpChangeListener listener)
        {
            _hpCaller.AddListener(listener);
        }
        public void AddSkillGaugeChangeListener(ISkillGaugeChangeListener listener)
        {
            _skillGaugeCaller.AddListener(listener);
        }
        
        public void CallHpChangeListeners(float hp)
        {
            _hpCaller.Call(hp);
        }
        
        public void CallSkillGaugeChangeListeners(float skillGauge)
        {
            _skillGaugeCaller.Call(skillGauge);
        }
    }
}