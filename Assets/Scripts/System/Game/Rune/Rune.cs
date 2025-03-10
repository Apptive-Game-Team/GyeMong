using System.Collections;
using Creature.Player.Component;

namespace System.Game.Rune
{
    public interface IRune
    {
        public void OnActivate(RuneContext context);
        public void OnDeactivate(RuneContext context);
    }
    
    //Parameter : Stat, Value
    public class ModifyStatRune : IRune
    {

        public void OnActivate(RuneContext context)
        {
            if (context is ModifyStatContext changeValueContext)
            {
                //How to Find Specific Stats in StatComp?
                //Implement in StatComp, And I call the method.
                //statComp.ChangeValueMethod();
                changeValueContext.StatComp.SetStatValue(changeValueContext.StatType, changeValueContext.ValueType, changeValueContext.Value);
            }
        }

        public void OnDeactivate(RuneContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    //Parameter : Coroutine(IEnumerator)
    public class RunCoroutineRune : IRune
    {
        public void OnActivate(RuneContext context)
        {
            if (context is RunCoroutineContext runCoroutineContext)
            {
                RuneConditionChecker.Instance.AddRuneCondition(runCoroutineContext.runeCondition); 
            }
        }

        public void OnDeactivate(RuneContext context)
        {
            throw new NotImplementedException();
        }
    }

    //Parameter : BuffObject, BuffComp
    public class AddBuffRune : IRune
    {
        public void OnActivate(RuneContext context)
        {
            if (context is AddBuffContext addBuffContext)
            {
                addBuffContext.BuffComp.AddBuff(addBuffContext.BuffData);
            }
        }

        public void OnDeactivate(RuneContext context)
        {
            throw new NotImplementedException();
        }
    }

    //Parameter : PLayerInput
    public class ChangePlayerInputRune
    {
        
    }
    
}
