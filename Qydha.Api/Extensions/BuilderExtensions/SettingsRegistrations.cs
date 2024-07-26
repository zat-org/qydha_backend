namespace Qydha.API.Extensions;

public static class SettingsRegistrations
{
    public static IServiceCollection RegisterSettings(this IServiceCollection services, IConfiguration configuration)
    {

        #region   DI settings
        // otp options  
        services.Configure<OTPSettings>(configuration.GetSection("OTP"));
        // twilio options 
        services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));
        // JWT options 
        services.Configure<JWTSettings>(configuration.GetSection("Authentication"));
        // mail server settings
        services.Configure<EmailSettings>(configuration.GetSection("MailSettings"));
        // whatsapp  settings
        services.Configure<WhatsAppSettings>(configuration.GetSection("WhatsAppSettings"));
        // Photo Settings
        services.Configure<AvatarSettings>(configuration.GetSection("AvatarSettings"));
        // IAPHub Settings
        services.Configure<IAPHubSettings>(configuration.GetSection("IAPHubSettings"));
        // Products Settings
        services.Configure<ProductsSettings>(configuration.GetSection("ProductsSettings"));
        // Notifications Settings
        services.Configure<PushNotificationsSettings>(configuration.GetSection("PushNotificationsSettings"));
        // Notification Image Settings
        services.Configure<NotificationImageSettings>(configuration.GetSection("NotificationImageSettings"));
        // Book Settings
        services.Configure<BookSettings>(configuration.GetSection("BookSettings"));
        // UltraMsg Settings
        services.Configure<UltraMsgSettings>(configuration.GetSection("UltraMsgSettings"));
        // RegisterGiftSetting
        services.Configure<RegisterGiftSetting>(configuration.GetSection("RegisterGiftSetting"));
        // WaApiSettings
        services.Configure<WaApiSettings>(configuration.GetSection("WaApiSettings"));
        // GoogleStorageSettings
        services.Configure<GoogleStorageSettings>(configuration.GetSection("GoogleStorage"));

        #endregion
        return services;
    }
}
