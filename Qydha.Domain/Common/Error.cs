namespace Qydha.Domain.Common;

public class Error
{
    public string Code { get; set; } = ErrorCodes.UnknownError;
    public string Message { get; set; } = string.Empty;
    public override string ToString()
    {
        return $"Code = {Code} , Message = {Message}";
    }
}
