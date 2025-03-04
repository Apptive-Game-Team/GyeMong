namespace System.Game.Rune
{
    public interface IRune
    {
        public void OnActivate(RuneContext context);
        public void OnDeactivate(RuneContext context);
    }
    
    //Parameter : Stat, Value
    public class ChangeValueRune : IRune
    {

        public void OnActivate(RuneContext context)
        {
            if (context is ChangeValueContext changeValueContext)
            {
                //How to Find Specific Stats in StatComp..?
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

    public class ChangeValueContext : RuneContext
    {
        public StatComponent StatComp { get; }
        public float Amount { get; }

        public ChangeValueContext(StatComponent stat, float amount)
        {
            StatComp = stat;
            Amount = amount;
        }
    }
}
