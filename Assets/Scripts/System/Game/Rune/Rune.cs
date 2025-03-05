using System.Collections;

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

    public enum StatValueType
    {
        DEFAULT_VALUE,
        FLAT_VALUE,
        PERCENT_VALUE,
        FINAL_PERCENT_VALUE,
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
                changeValueContext.StatComp.ModifyStat(changeValueContext.StatName, changeValueContext.ValueName, changeValueContext.Amount);
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
            if (context is RunCoroutineContext runCoroutineContext)
            {
                
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
        public string StatName { get; } //TO DO. Change into Enum.
        public string ValueName { get; } //TO DO. Change into Enum.
        public float Amount { get; }

        //Too Many Parameters. Struct. Need.
        public ModifyStatContext(StatComponent statComp, string statName, string valueName ,float amount)
        {
            StatComp = statComp;
            StatName = statName;
            ValueName = valueName;
            Amount = amount;
        }
    }

    public class RunCoroutineContext : RuneContext
    {
        private IEnumerator _coroutine;
    }    
    public class AddBuffContext : RuneContext
    {
        public BuffComponent BuffComp { get;}
        public BuffObject BuffObject { get;}

        public AddBuffContext(BuffComponent buffComp, BuffObject buffObject)
        {
            BuffComp = buffComp;
            BuffObject = buffObject;
        }
    }    
    //How to Make PlayerInput class?
    //Members : KeyAction, PLayerAction,
    //I Want to Change PlayerAction or KeyAction to RuneActivated-Things.
    public class ChangePlayerInputContext : RuneContext
    {
        
    }
}
