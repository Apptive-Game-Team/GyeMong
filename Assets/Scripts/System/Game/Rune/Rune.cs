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
            }
        }

        public void OnDeactivate(RuneContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    //Parameter : Coroutine(IEnumerator)
    public class RunCoroutineRune
    {
        
    }

    //Parameter : BuffObject, BuffComp
    public class AddBuffRune
    {
        
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
        public StatComponent StatComp { get; }
        public float Amount { get; }

        public ModifyStatContext(StatComponent stat, float amount)
        {
            StatComp = stat;
            Amount = amount;
        }
    }
}
