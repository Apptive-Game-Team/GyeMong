using System.Collections.Generic;

namespace System.Event.Interpreter.Expression
{
    public abstract class Expression
    {
        public abstract List<global::Event> Interpret();
    }
}