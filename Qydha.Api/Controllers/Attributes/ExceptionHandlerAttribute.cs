using System.Data.Common;

namespace Qydha.API.Controllers.Attributes;

public class ExceptionHandlerAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionHandlerAttribute> _logger;
    public ExceptionHandlerAttribute(ILogger<ExceptionHandlerAttribute> logger)
    {
        _logger = logger;
    }
    public override void OnException(ExceptionContext context)
    {

        var exception = context.Exception;
        ObjectResult res;
        if (exception is InvalidBalootGameEventException balootEventException)
        {
            res = new ObjectResult(new Error()
            {
                Message = balootEventException.Message,
                Code = ErrorType.InvalidBodyInput
            })
            {
                StatusCode = 400
            };
        }
        else if (exception is DbException dbException)
        {
            _logger.LogError(dbException, "Error caught by Exception Filter DbException Message :: {msg} ", dbException.Message); //logger 
            res = new ObjectResult(new Error()
            {
                Message = dbException.Message,
                Code = ErrorType.ServerErrorOnDB
            })
            {
                StatusCode = 500
            };
        }
        else
        {
            _logger.LogError(exception, "Error caught by Exception Filter Outer exception Message :: {msg} ", exception.Message); //logger 
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            _logger.LogError(exception, "Error caught by Exception Filter Inner exception Message :: {msg} ", exception.Message); //logger 
            res = new ObjectResult(new Error()
            {
                Message = exception.Message,
                Code = ErrorType.UnknownServerError
            })
            {
                StatusCode = 500
            };
        }
        context.Result = res;
    }
}
