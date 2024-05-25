using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace Qydha.API.Validators;

public class ValidatorResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
    {
        var bodyError = new InvalidBodyInputError();
        foreach (var errors in validationProblemDetails!.Errors)
        {
            bodyError.ValidationErrors.Add(errors.Key, errors.Value.Select(e =>
                    {
                        if (e.StartsWith("Error converting value"))
                            return $"Invalid {errors.Key} , the provided data can't be casted to the target data type.";
                        return e;
                    }).ToList());
        };
        return bodyError.Handle(context.HttpContext.TraceIdentifier);
    }
}
