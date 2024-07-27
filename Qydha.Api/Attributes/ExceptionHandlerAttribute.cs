using System.Data.Common;
using Newtonsoft.Json;

namespace Qydha.API.Attributes;

public class ExceptionHandlerAttribute(ILogger<ExceptionHandlerAttribute> logger) : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionHandlerAttribute> _logger = logger;

    public override void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        ResultError res = exception switch
        {
            InvalidBalootGameEventException => new InvalidBodyInputError(exception.Message),
            JsonReaderException => new InvalidBodyInputError("Invalid body input schema"),
            OperationCanceledException => new RequestTimeoutError(),
            DbException => new InternalServerError(),
            _ => new InternalServerError(),
        };
        _logger.LogError(exception, "Error caught by Exception Filter exception type :: {exceptionType} ; Message :: {msg} ; trace id : {TraceId}", exception.GetType(), exception.Message, context.HttpContext.TraceIdentifier); //logger 
        context.Result = res.ToObjectResult(context.HttpContext.TraceIdentifier);
    }
}
public sealed class InternalServerError() : ResultError(
    "Internal Server Error , please try again later",
    ErrorType.InternalServerError,
    StatusCodes.Status500InternalServerError)
{ }

public sealed class RequestTimeoutError() : ResultError(
    "The operation was canceled.",
    ErrorType.RequestTimeout,
    StatusCodes.Status408RequestTimeout
)
{ }