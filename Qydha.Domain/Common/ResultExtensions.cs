// using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
namespace Qydha.Domain.Common;

public static class ResultExtensions
{
    public static Result OnSuccess(this Result result, Action action)
    {
        if (result.IsFailed) return result;
        action();
        return Result.Ok();
    }
    public static Result OnSuccess(this Result result, Func<Result> func)
    {
        if (result.IsFailed) return result;
        return func();
    }

    public static Result<OutT> OnSuccess<InT, OutT>(this Result<InT> result, Func<InT, Result<OutT>> func)
    {
        if (result.IsSuccess)
            return func(result.Value);
        return result.ToResult<OutT>();
    }
    public static Result OnSuccess<InT>(this Result<InT> result, Func<InT, Result> func)
    {
        if (result.IsSuccess)
            return func(result.Value);
        return result.ToResult();
    }
    public static Result<OutT> OnSuccess<OutT>(this Result result, Func<Result<OutT>> func)
    {
        if (result.IsSuccess)
            return func();
        return result;
    }


    public static Result OnSuccessAsync(this Result result, Func<Task<Result>> func)
    {
        if (result.IsSuccess)
            return func().GetAwaiter().GetResult();
        return result;
    }

    public static Result<T> OnSuccessAsync<T>(this Result result, Func<Task<Result<T>>> func)
    {
        if (result.IsSuccess)
            return func().GetAwaiter().GetResult();
        return result;
    }

    public static Result OnSuccessAsync<T>(this Result<T> result, Func<T, Task<Result>> func)
    {
        if (result.IsSuccess)
            return func(result.Value).GetAwaiter().GetResult();
        return result.ToResult();
    }

    public static Result<OutT> OnSuccessAsync<InT, OutT>(this Result<InT> result, Func<InT, Task<Result<OutT>>> func)
    {
        if (result.IsSuccess)
            return func(result.Value).GetAwaiter().GetResult();
        return result.ToResult<OutT>();
    }

    public static OutT Handle<OutT>(this Result result, Func<OutT> OnSuccessFunc, Func<List<IError>, OutT> OnFailureFunc)
    {
        if (result.IsSuccess)
        {
            return OnSuccessFunc();
        }
        else
        {
            return OnFailureFunc(result.Errors);
        }
    }
    public static void Handle<InT>(this Result<InT> result, Action<InT> OnSuccessFunc, Action<List<IError>> OnFailureFunc)
    {
        if (result.IsSuccess)
        {
            OnSuccessFunc(result.Value);
        }
        else
        {
            OnFailureFunc(result.Errors);
        }
    }

    public static OutT Handle<InT, OutT>(this Result<InT> result, Func<InT, OutT> OnSuccessFunc, Func<List<IError>, OutT> OnFailureFunc)
    {
        if (result.IsSuccess)
        {
            return OnSuccessFunc(result.Value);
        }
        else
        {
            return OnFailureFunc(result.Errors);
        }
    }

}
public static class ResultResolver
{
    public static IActionResult Resolve(this Result result, Func<IActionResult> success) =>
        result switch
        {
            { IsSuccess: true } => success(),
            { IsFailed: true } => result.Errors.First().Handle(),
            _ => throw new InvalidOperationException()
        };

    public static IActionResult Resolve<T>(this Result<T> result,
        Func<T, IActionResult> success) =>
        result switch
        {
            { IsSuccess: true } => success(result.Value),
            { IsFailed: true } => result.Errors.First().Handle(),
            _ => throw new InvalidOperationException()
        };
}
public static class ErrorHandler
{
    private static HttpContext HttpContext => new HttpContextAccessor().HttpContext!;

    public static IActionResult Handle(this IError error)
    {
        if (error is ResultError resultError)
            return resultError.ToIResult("Just Trace ID");
        else
            return new JsonResult(new ErrorResponse(ErrorType.InternalServerError, error.Message, "Just Trace ID"))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
    }
}
