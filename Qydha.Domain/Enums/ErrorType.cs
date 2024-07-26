namespace Qydha.Domain.Enums;

public enum ErrorType
{
    #region input errors
    InvalidBodyInput = 4001, //400
    InvalidPatchBodyInput = 4002, //400
    #endregion

    #region AuthN , AuthZ errors
    InvalidAuthToken = 4010, // 401
    InvalidCredentials = 4011, // 400
    InvalidActionOrForbidden = 4014, //403
    InvalidRefreshToken = 4015, // 400
    #endregion

    #region influencer codes errors
    InfluencerCodeExpired = 4020, //400
    InfluencerCodeAlreadyUsed = 4021,//400
    InfluencerCodeExceedMaxUsageCount = 4022,//400
    InfluencerCodeCategoryAlreadyUsed = 4023,//400

    #endregion

    #region Promo Codes errors
    UserDoesNotOwnThePromoCode = 4030, //403
    PromoCodeAlreadyUsed = 4031, //400
    PromoCodeExpired = 4032,//400
    #endregion

    #region db errors
    DbUniqueViolation = 4039,//409
    EntityNotFound = 4040, //404

    #endregion

    #region Purchase Errors
    InvalidProductSku = 4092, // 400

    #endregion

    #region Push Notifications Errors
    InvalidFCMToken = 4101, //400
    FcmMessagingException = 4103, //500
    #endregion


    #region Otp Errors
    IncorrectOTP = 4110, //400 
    RequestExceededTimeLimit = 4111, //400
    OTPEmailSendingError = 4112, //500
    OTPPhoneSendingError = 4113, //500
    OTPAlreadyUsedError = 4114,//400

    #endregion

    #region WaApi Errors
    WaApiInstanceNotReady = 4130,//500
    WaApiUnknownError = 4131,//500

    #endregion


    #region files errors
    FileUploadError = 4120,//500
    FileDeleteError = 4121,//500
    #endregion

    #region BalootGame Errors 
    InvalidBalootGameAction = 4200,//400
    #endregion

    #region server errors
    InternalServerError = 5000,//500
    #endregion
}
