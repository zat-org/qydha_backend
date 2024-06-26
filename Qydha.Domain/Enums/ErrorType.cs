﻿namespace Qydha.Domain.Enums;

public enum ErrorType
{
    #region input errors
    InvalidBodyInput = 4001, //400
    InvalidPatchBodyInput = 4002, //400
    #endregion

    #region AuthN , AuthZ errors
    InvalidAuthToken = 4010, // 401
    InvalidCredentials = 4011, // 400
    InvalidActionByAnonymousUser = 4012, // 400
    InvalidActionByRegularUser = 4013, // 400
    InvalidActionOrForbidden = 4014, //403
    #endregion

    #region influencer codes errors
    InfluencerCodeExpired = 4020,
    InfluencerCodeAlreadyUsed = 4021,
    InfluencerCodeExceedMaxUsageCount = 4022,
    InfluencerCodeCategoryAlreadyUsed = 4023,

    #endregion

    #region Promo Codes errors
    UserDoesNotOwnThePromoCode = 4030,
    PromoCodeAlreadyUsed = 4031,
    PromoCodeExpired = 4032,
    #endregion

    #region db errors
    DbForeignKeyViolation = 4038,
    DbUniqueViolation = 4039,
    EntityNotFound = 4040,
    UserNotFound = 4041,
    AdminUserNotFound = 4042,
    InfluencerCodeNotFound = 4043,
    NotificationNotFound = 4044,
    PhoneAuthenticationRequestNotFound = 4045,
    PurchaseNotFound = 4046,
    RegistrationOTPRequestNotFound = 4047,
    UpdateEmailRequestNotFound = 4048,
    UpdatePhoneRequestNotFound = 4049,
    UserPromoCodeNotFound = 4050,
    InfluencerCodeCategoryNotFound = 4051,
    LoginWithQydhaRequestNotFound = 4052,
    AssetNotFound = 4053,
    UserGeneralSettingsNotFound = 4054,
    UserBalootSettingsNotFound = 4055,
    UserHandSettingsNotFound = 4056,

    #endregion

    #region forget password errors
    InvalidForgetPasswordRequest = 4080,
    ForgetPasswordRequestExceedTime = 4081,
    #endregion

    #region Purchase Errors
    InvalidIAPHupToken = 4090,
    FreeSubscriptionExceededTheLimit = 4091,
    InvalidProductSku = 4092,

    #endregion

    #region Push Notifications Errors
    InvalidTopicName = 4100,
    InvalidFCMToken = 4101,
    InvalidFCMTokensArray = 4102,
    FcmMessagingException = 4103,
    FcmRegularException = 4104,
    #endregion


    #region Otp Errors
    IncorrectOTP = 4110,
    OTPExceededTimeLimit = 4111,
    OTPEmailSendingError = 4112,
    OTPPhoneSendingError = 4113,
    OTPAlreadyUsedError = 4114,

    #endregion

    #region WaApi Errors
    WaApiInstanceNotReady = 4130,
    WaApiUnknownError = 4131,

    #endregion


    #region files errors
    FileUploadError = 4120,
    FileDeleteError = 4121,
    #endregion

    #region server errors
    UnknownServerError = 5000,
    ServerErrorOnDB = 5001,
    #endregion
}
