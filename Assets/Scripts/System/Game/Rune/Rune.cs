using System.Collections;
using Creature.Player.Component;

namespace System.Game.Rune
{
    public enum RuneType
    {
        DEFAULT,
        MODIFY_STAT,
        RUN_COROUTINE,
        ADD_BUFF,
        CHANGE_PLAYER_INPUT,
    }
    
    
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
    
    //Goal. I Want to Abstract these Parameters into a single General Parameter...
    //Otherwise, I would have to use if-else...
    public abstract class RuneContext
    {
        
    }
    
    public class ModifyStatContext : RuneContext
    {
        public StatComponent StatComp { get;}
        public StatType StatType { get; } 
        public StatValueType ValueType { get; } 
        public float Value { get; }

        //Too Many Parameters. Struct. Need.
        public ModifyStatContext(StatComponent statComp, StatType statType, StatValueType statValueType , float value)
        {
            StatComp = statComp;
            StatType = statType;
            ValueType = statValueType;
            Value = value;
        }
    }

    public class RunCoroutineContext : RuneContext
    {
        public RuneCondition runeCondition;
    }    
    public class AddBuffContext : RuneContext
    {
        public BuffComponent BuffComp { get;}
        public BuffData BuffData { get;}

        public AddBuffContext(BuffComponent buffComp, BuffData buffData)
        {
            BuffComp = buffComp;
            BuffData = buffData;
        }
    }    
    
    //How to Make PlayerInput class?
    //Members : KeyAction, PLayerAction,
    //I Want to Change PlayerAction or KeyAction to RuneActivated-Things.
    public class ChangePlayerInputContext : RuneContext
    {
        
    }
    
    
}
