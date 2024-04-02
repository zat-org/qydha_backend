namespace Qydha.Domain.Common;

public static class ResultExtensions
{


    public static Result<T> MapTo<T>(this Result res, T value)
    {
        if (res.IsFailure)
            return Result.Fail<T>(res.Error);
        return Result.Ok(value);
    }
    public static Result<OutT> MapTo<InT, OutT>(this Result<InT> res, Func<InT, OutT> func)
    {
        if (res.IsFailure)
            return Result.Fail<OutT>(res.Error);
        return Result.Ok(func(res.Value));
    }

    public static Result OnSuccess(this Result result, Action action)
    {
        if (result.IsFailure) return result;
        action();
        return Result.Ok();
    }

    // public static Result OnSuccess(this Result result, Func<Result> func)
    // {
    //     if (result.IsFailure) return result;
    //     return func();
    // }

    public static Result<OutT> OnSuccess<InT, OutT>(this Result<InT> result, Func<InT, Result<OutT>> func)
    {
        if (result.IsSuccess)
            return func(result.Value);
        return Result.Fail<OutT>(result.Error);
    }

    public static Result OnSuccessAsync(this Result result, Func<Task<Result>> func)
    {
        if (result.IsSuccess)
        {
            Task<Result> awaitableTask = func();
            Result res = awaitableTask.GetAwaiter().GetResult();
            return res;
        }
        return result;
    }

    public static Result<T> OnSuccessAsync<T>(this Result result, Func<Task<Result<T>>> func)
    {
        if (result.IsSuccess)
        {
            Task<Result<T>> awaitableTask = func();
            Result<T> res = awaitableTask.GetAwaiter().GetResult();
            return res;
        }
        return Result.Fail<T>(result.Error);
    }
    public static Result<T> OnSuccess<T>(this Result<T> result, Func<T, Result<T>> func)
    {
        if (result.IsSuccess) return func(result.Value);
        return result;
    }
    public static Result<T> OnSuccessAsync<T>(this Result<T> result, Func<T, Task<Result<T>>> func)
    {
        if (result.IsSuccess)
        {
            Task<Result<T>> awaitableTask = func(result.Value);
            Result<T> res = awaitableTask.GetAwaiter().GetResult();
            return res;
        }
        return result;
    }
    public static Result<OutT> OnSuccessAsync<InT, OutT>(this Result<InT> result, Func<InT, Task<Result<OutT>>> func)
    {
        if (result.IsSuccess)
        {
            Task<Result<OutT>> awaitableTask = func(result.Value);
            Result<OutT> res = awaitableTask.GetAwaiter().GetResult();
            return res;
        }
        return Result.Fail<OutT>(result.Error);
    }

    public static Result OnFailure(this Result result, Action action)
    {
        if (result.IsFailure)
            action();
        return result;
    }
    public static Result OnFailure(this Result result, Action<Result> action)
    {
        if (result.IsFailure)
            action(result);
        return result;
    }
    public static Result OnFailure(this Result result, Func<Error, Error> action)
    {
        if (result.IsFailure)
            return Result.Fail(action(result.Error));
        return result;
    }


    public static Result<T> OnFailure<T>(this Result<T> result, Func<Error, Error> action)
    {
        if (result.IsFailure)
            return Result.Fail<T>(action(result.Error));
        return result;
    }


    public static OutT HandleAsync<InT, OutT>(this Result<InT> result, Func<InT, OutT> OnSuccessFunc, Func<Error, Task<OutT>> OnFailureFunc)
    {
        if (result.IsSuccess)
        {
            return OnSuccessFunc(result.Value);
        }
        else
        {
            Task<OutT> awaitableTask = OnFailureFunc(result.Error);
            OutT res = awaitableTask.GetAwaiter().GetResult();
            return res;
        }
    }

    public static T Handle<T>(this Result result, Func<T> OnSuccessFunc, Func<Error, T> OnFailureFunc)
    {
        if (result.IsSuccess)
        {
            return OnSuccessFunc();
        }
        else
        {
            return OnFailureFunc(result.Error);
        }
    }

    public static void Handle<InT>(this Result<InT> result, Action<InT> OnSuccessFunc, Action<Error> OnFailureFunc)
    {
        if (result.IsSuccess)
        {
            OnSuccessFunc(result.Value);
        }
        else
        {
            OnFailureFunc(result.Error);
        }
    }

    public static OutT Handle<InT, OutT>(this Result<InT> result, Func<InT, OutT> OnSuccessFunc, Func<Error, OutT> OnFailureFunc)
    {
        if (result.IsSuccess)
        {
            return OnSuccessFunc(result.Value);
        }
        else
        {
            return OnFailureFunc(result.Error);
        }
    }
}
