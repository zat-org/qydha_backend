using Microsoft.AspNetCore.Mvc;

namespace Qydha.Domain.Common;

public abstract class ResultError(string msg, ErrorType errorCode, int statusCode) : Error(msg)
{
    public ErrorType ErrorCode { get; set; } = errorCode;
    public int StatusCode { get; set; } = statusCode;
    public virtual IActionResult ToIResult(string traceId) =>
        new JsonResult(new ErrorResponse(ErrorCode, Message, traceId)) { StatusCode = StatusCode };
}
public class InvalidBodyInputError : ResultError
{
    public Dictionary<string, List<string>> ValidationErrors { get; set; } = [];

    public InvalidBodyInputError()
        : base("Invalid Body Input", ErrorType.InvalidBodyInput, StatusCodes.Status400BadRequest)
    { }
    public InvalidBodyInputError(string msg)
     : base(msg, ErrorType.InvalidBodyInput, StatusCodes.Status400BadRequest)
    { }

    public override IActionResult ToIResult(string traceId)
    {
        return new JsonResult(new ValidationErrorResponse(ErrorCode, Message, ValidationErrors)) { StatusCode = StatusCode };
    }
}

public class OtpPhoneSendingError(string serviceName) : ResultError(
    $"Sending otp using {serviceName} Failed, Please try again later.",
    ErrorType.OTPPhoneSendingError,
    StatusCodes.Status500InternalServerError)
{ }

public class OtpEmailSendingError(string serviceName) : ResultError(
    $"Sending otp using {serviceName} Failed, Please try again later.",
    ErrorType.OTPEmailSendingError,
    StatusCodes.Status500InternalServerError)
{ }


public class WaApiUnknownError() : ResultError(
    $"Sending otp using WaApi Failed with unknown Error.",
    ErrorType.WaApiUnknownError,
    StatusCodes.Status500InternalServerError)
{ }
public class WaApiInstanceNotReadyError(int instanceId) : ResultError(
    $"Sending otp using WaApi Failed Because Instance with id : {instanceId} NotReady.",
    ErrorType.WaApiInstanceNotReady,
    StatusCodes.Status500InternalServerError)
{ }


public class FileStorageOperationError(FileStorageAction action, string path) : ResultError(
    $"can't finish action : {action} on file with {path} , Please try again later.",
    action == FileStorageAction.Upload ? ErrorType.FileUploadError : ErrorType.FileDeleteError,
    StatusCodes.Status500InternalServerError)
{ }

public class NotifyingInvalidFCMTokenError(string token) : ResultError(
    $"Invalid FCM Token Value : '{token}'",
    ErrorType.InvalidFCMToken,
    StatusCodes.Status400BadRequest)
{ }

public class FCMError() : ResultError(
    "unknown fcm error",
    ErrorType.FcmMessagingException,
    StatusCodes.Status400BadRequest)
{ }

public class EntityNotFoundError<T>(T identifier, string entityName, ErrorType notFoundErrorCode)
: ResultError(
    $"Entity {entityName} Not Found with identifier : {identifier}",
    notFoundErrorCode,
    StatusCodes.Status404NotFound)
{ }

public class EntityUniqueViolationError(string userMessage) : ResultError(
    userMessage,
    ErrorType.DbUniqueViolation,
    StatusCodes.Status400BadRequest)
{ }



public class InvalidCredentialsError(string userMessage)
    : ResultError(userMessage, ErrorType.InvalidCredentials, StatusCodes.Status400BadRequest)
{ }


// TODO can be converted to 403
public class InvalidPhoneAuthenticationRequestError()
    : ResultError($"Authenticated User is not the same in the phone Authentication request", ErrorType.InvalidForgetPasswordRequest, StatusCodes.Status400BadRequest)
{ }

public class RequestExceedTimeError(DateTimeOffset referenceTime, string requestEntityName)
    : ResultError(
        $"Request of ( {requestEntityName} ) Exceed the time Interval reference time at : {referenceTime} , current time :{DateTimeOffset.UtcNow}",
        ErrorType.OTPExceededTimeLimit,
        StatusCodes.Status400BadRequest)
{ }
public class IncorrectOtpError(string requestEntityName)
    : ResultError($"Incorrect Otp for Request of type ( {requestEntityName} ) ", ErrorType.IncorrectOTP, StatusCodes.Status400BadRequest)
{ }

public class OtpAlreadyUsedError(DateTimeOffset usedAt, string requestEntityName)
    : ResultError($"Otp for {requestEntityName} Already Used before At {usedAt}", ErrorType.OTPAlreadyUsedError, StatusCodes.Status400BadRequest)
{ }


public class InvalidIAPHupTokenError()
    : ResultError($"x-auth-token header is Missing OR wrong", ErrorType.InvalidIAPHupToken, StatusCodes.Status403Forbidden)
{ }

public class InvalidAuthTokenError()
    : ResultError($"Invalid Bearer Token", ErrorType.InvalidAuthToken, StatusCodes.Status401Unauthorized)
{ }

public class ForbiddenError() : ResultError($"Auth user is has not previliadge to do the requested action", ErrorType.InvalidActionOrForbidden, StatusCodes.Status403Forbidden)
{ }