
namespace Qydha.API.Exceptions;

public class InvalidBalootGameEventException : System.Exception
{

    public InvalidBalootGameEventException() : base() { }
    public InvalidBalootGameEventException(string message) : base(message) { }
    public InvalidBalootGameEventException(string message, System.Exception inner) : base(message, inner) { }

}
