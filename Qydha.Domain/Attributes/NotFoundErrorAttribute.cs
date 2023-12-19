namespace Qydha.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class NotFoundErrorAttribute(ErrorType errorType) : Attribute
{
    public readonly ErrorType NotFoundErrorType = errorType;
}
