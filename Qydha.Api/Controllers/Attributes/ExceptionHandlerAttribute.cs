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

        var Exception = context.Exception;
        _logger.LogError(Exception, "Error caught by Exception Filter "); //logger 
        while (Exception.InnerException != null)
        {
            Exception = Exception.InnerException;
        }
        context.Result = new BadRequestObjectResult(new Error()
        {
            Message = Exception.Message,
            Code = ErrorCodes.UnknownError
        });
    }
}
