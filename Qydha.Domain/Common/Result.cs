namespace Qydha.Domain.Common;

public class Result

{
    private readonly Error? _error;
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;

    public Error Error
    {
        get
        {
            if (IsSuccess || _error is null) throw new InvalidOperationException("Can't get the Error at Success.");
            return _error;
        }
    }

    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Can't create result with state of Success with error.");
        if (!isSuccess && error is null)
            throw new InvalidOperationException("Can't create result with state of Failure without error.");

        IsSuccess = isSuccess;
        _error = error;
    }

    public static Result Fail(Error error)
    {
        return new Result(false, error);
    }
    public static Result<T> Fail<T>(Error error)
    {
        return new Result<T>(default, false, error);
    }
    public static Result Ok()
    {
        return new Result(true, null);
    }
    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(value, true, null);
    }

    public static Result Combine(Result[] results)
    {
        foreach (Result res in results)
        {
            if (res.IsFailure) return res;
        }
        return Ok();
    }
}

public class Result<T> : Result
{
    public readonly T? _value;
    protected internal Result(T? value, bool isSuccess, Error? error) : base(isSuccess, error)
    {
        if (isSuccess && value is null)
            throw new InvalidOperationException("Can't set the result value with null in Success state.");
        _value = value;
    }

    public T Value
    {
        get
        {
            if (IsFailure || _value is null) throw new InvalidOperationException("Can't get the result value at failure.");
            return _value;
        }
    }
}
