
namespace Qydha.Domain.Common;

public class Error
{
    public ErrorType Code { get; set; } = ErrorType.UnknownServerError;
    public string Message { get; set; } = string.Empty;
    public override string ToString()
    {
        return $"Code = {Code} , Message = {Message}";
    }
}
