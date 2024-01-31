namespace Qydha.API.Extensions;

public static class AbstractValidatorExtensions
{
    public static Result<T> ValidateAsResult<T>(this AbstractValidator<T> validator, T dto)
    {
        var validationRes = validator.Validate(dto);

        if (!validationRes.IsValid)
            return Result.Fail<T>(new Error()
            {
                Code = ErrorType.InvalidBodyInput,
                Message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
            });
        else return Result.Ok(dto);
    }
}
