using System.Data.Common;

namespace Qydha.API.Attributes;

public class ExceptionHandlerAttribute(ILogger<ExceptionHandlerAttribute> logger) : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionHandlerAttribute> _logger = logger;

    public override void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        ObjectResult res;
        if (exception is InvalidBalootGameEventException balootEventException)
        {
            res = new ObjectResult(new InvalidBodyInputError(balootEventException.Message))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        else if (exception is DbException dbException)
        {
            _logger.LogError(dbException, "Error caught by Exception Filter DbException Message :: {msg} ", dbException.Message); //logger 
            res = new ObjectResult(new InternalServerError())
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        else
        {
            _logger.LogError(exception, "Error caught by Exception Filter Outer (to get the source of error in code) exception Message :: {msg} ", exception.Message); //logger 
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            _logger.LogError(exception, "Error caught by Exception Filter Inner (to get the cause of error) exception Message :: {msg} ", exception.Message); //logger 
            res = new ObjectResult(new InternalServerError())
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        context.Result = res;
    }
}
public sealed class InternalServerError() : ResultError(
    "Internal Server Error , please try again later",
    ErrorType.InternalServerError,
    StatusCodes.Status500InternalServerError)
{ }