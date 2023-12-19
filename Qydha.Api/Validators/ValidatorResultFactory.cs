using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace Qydha.API.Validators;

public class ValidatorResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
    {
        var errors = validationProblemDetails?.Errors.SelectMany(errors =>
        {
            return errors.Value.Select(e =>
            {
                if (e.StartsWith("Error converting value"))
                    return $"Invalid {errors.Key} , the provided data can't be casted to the target data type.";
                return e;
            });
        });

        return new BadRequestObjectResult(new Error()
        {
            Code = ErrorType.InvalidBodyInput,
            Message = string.Join(" ;", errors!)
        });
    }
}
