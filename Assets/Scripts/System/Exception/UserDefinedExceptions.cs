using ExceptionBase = System.Exception;

namespace Systems.Exceptions
{
    public class NotFoundException : ExceptionBase
    {
        public NotFoundException(string message) : base(message) { }
    }
}
