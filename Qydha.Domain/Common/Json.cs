namespace Qydha.Domain.Common;

public class Json<T>(T? value)
{
    public T? Value { get; } = value;

    public static implicit operator Json<T>(T? value)
    {
        return new Json<T>(value);
    }
}
