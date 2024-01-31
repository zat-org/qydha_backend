namespace Qydha.API.Extensions;

public static class JsonPatchExtension
{
    public static Result<T> ApplyToAsResult<T>(this JsonPatchDocument<T> patchDocument, T dto) where T : class
    {
        try
        {
            patchDocument.ApplyTo(dto);
            return Result.Ok(dto);
        }
        catch (JsonPatchException exp)
        {
            return Result.Fail<T>(new()
            {
                Code = ErrorType.InvalidPatchBodyInput,
                Message = exp.Message
            });
        }
    }
}