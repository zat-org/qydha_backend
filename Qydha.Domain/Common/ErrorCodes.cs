﻿namespace Qydha.Domain.Common;

public static class ErrorCodes
{
    public const string InvalidInput = "4000";
    public const string InvalidBodyInput = "4001";
    public const string InvalidPhotoFileInput = "4002";
    public const string NotFound = "4040";

    public const string AnonymousUserNotFound = "4041";
    public const string UserNotFound = "4042";
    public const string RegistrationRequestNotFound = "4043";
    public const string UpdatePhoneRequestNotFound = "4044";
    public const string UpdateEmailRequestNotFound = "4045";

    public const string NotificationNotFound = "4046";
    public const string PhoneAuthenticationRequestNotFound = "4047";
    public const string UserPromoCodeNotFound = "4048";
    public const string InfluencerCodeNotFound = "4049";
    public const string InfluencerCodeExpired = "4080";
    public const string InvalidTokensArray = "4081";
    public const string AuthenticatedUserDoesNotOwnThisPromoCode = "4082";
    public const string PromoCodeAlreadyUsed = "4083";
    public const string PromoCodeExpired = "4084";

    public const string ForgetPasswordRequestExceedTime = "4085";

    public const string InvalidForgetPasswordRequest = "4086";

    public const string InvalidOperationOnAnonymousUser = "4087";

    public const string InvalidProductSku = "4088";
    public const string InvalidTopicName = "4089";
    public const string InvalidFCMToken = "4090";
    public const string InvalidDeleteOnRegularUser = "4091";
    public const string InvalidIAPHupToken = "4092";
    public const string EmailSendingError = "4093";
    public const string InvalidToken = "4094";
    public const string InvalidOTP = "4095";
    public const string OTPExceededTimeLimit = "4096";
    public const string InvalidCredentials = "4097";
    public const string AnonymousUserTokenNotProvided = "4098";
    public const string OTPSendingError = "4099";
    public const string DbUniqueViolation = "4100";
    public const string FileUploadError = "4101";
    public const string FileDeleteError = "4102";
    public const string UnhandledIAPHubTransactionType = "4103";
    public const string FreeSubscriptionUsedExceededTheAllowedNumber = "4104";
    public const string ServerError = "5000";
    public const string ServerErrorOnDB = "5001";

    public const string UnknownError = "1000";

}