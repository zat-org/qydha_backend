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

            res = new InvalidBodyInputError(balootEventException.Message).ToObjectResult(context.HttpContext.TraceIdentifier);

        }
        else if (exception is DbException dbException)
        {
            _logger.LogError(dbException, "Error caught by Exception Filter DbException Message :: {msg} ; trace id : {TraceId}", dbException.Message, context.HttpContext.TraceIdentifier); //logger 
            res = new InternalServerError().ToObjectResult(context.HttpContext.TraceIdentifier);

        }
        else
        {
            _logger.LogError(exception, "Error caught by Exception Handler exception Message :: {msg} ; trace id : {TraceId}", exception.Message, context.HttpContext.TraceIdentifier); //logger 
            res = new InternalServerError().ToObjectResult(context.HttpContext.TraceIdentifier);
        }
        context.Result = res;
    }
}
public sealed class InternalServerError() : ResultError(
    "Internal Server Error , please try again later",
    ErrorType.InternalServerError,
    StatusCodes.Status500InternalServerError)
{ }