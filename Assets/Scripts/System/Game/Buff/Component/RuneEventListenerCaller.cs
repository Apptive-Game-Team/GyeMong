using Util.ChangeListener;
using Util.Interface;

public interface IRuneEventListener : IChangeListener<BuffComponent>
{
    
}

namespace System.Game.Buff.Component
{
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