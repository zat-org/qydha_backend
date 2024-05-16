namespace Qydha.API.Extensions;

public static class AbstractValidatorExtensions
{
    public static Result<T> ValidateAsResult<T>(this AbstractValidator<T> validator, T dto)
    {
        var validationRes = validator.Validate(dto);

        if (!validationRes.IsValid)
        {
            var error = new InvalidBodyInputError();
            validationRes.Errors.ForEach(e =>
            {
                if (error.ValidationErrors.TryGetValue(e.PropertyName, out List<string>? propErrorsList))
                    propErrorsList.Add(e.ErrorMessage);
                else
                    error.ValidationErrors.Add(e.PropertyName, [e.ErrorMessage]);
            });
            return Result.Fail<T>(error);
        }
        else return Result.Ok(dto);
    }
}
