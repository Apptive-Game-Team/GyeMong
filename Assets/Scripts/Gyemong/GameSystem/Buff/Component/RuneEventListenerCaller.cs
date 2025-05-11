using Util.ChangeListener;

namespace Gyemong.GameSystem.Buff.Component
{
    public interface IRuneEventListener : IChangeListener<BuffComponent>
    {
    
    }

    public class RuneEventListenerCaller
    {
        private class RuneEventCaller : ChangeListenerCaller<IRuneEventListener, BuffComponent>{}

        private RuneEventCaller _runeEventCaller = new();
        
        public void AddRuneEventListener(IRuneEventListener listener)
        {
            _runeEventCaller.AddListener(listener);
        }
        
        public void CallRuneEventListeners(BuffComponent buffComp)
        {
            _runeEventCaller.Call(buffComp);
        }
        
    }
}