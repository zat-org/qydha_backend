
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
            return Result.Fail(new InvalidPatchBodyInputError(exp.Message));
        }
    }
}
public class InvalidPatchBodyInputError(string msg) : ResultError(msg, ErrorType.InvalidPatchBodyInput, StatusCodes.Status400BadRequest) { }