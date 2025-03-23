using System.Collections.Generic;

namespace System.Event.Interpreter
{
    public interface IEventInterpreter
    {
        List<global::Event> Interpret(string path);
    }
}