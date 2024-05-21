namespace Qydha.API.Extensions;

public static class SevicesRegistrations
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {

        #region DI Services
        services.AddSingleton<TokenManager>();
        services.AddSingleton<OtpManager>();
        services.AddSingleton<WaApiInstancesTracker>();

        services.AddSingleton(new GoogleStorageService(configuration.GetSection("GoogleStorage").Get<GoogleStorageSettings>()?.JsonKeyPath
           ?? throw new ArgumentNullException("can't get storage service key.")));

        services.AddHttpClient();

        services.AddScoped<WaApiService>();
        services.AddScoped<WhatsAppService>();

        services.AddScoped<IMessageService, OtpSenderByWhatsAppService>();
        services.AddScoped<IMailingService, MailingService>();
        services.AddScoped<IFileService, GoogleCloudFileService>();
        services.AddScoped<IPushNotificationService, FCMService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IUserPromoCodesService, UserPromoCodesService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IInfluencerCodesService, InfluencerCodesService>();
        services.AddScoped<IAppAssetsService, AppAssetsService>();
        services.AddScoped<IInfluencerCodeCategoryService, InfluencerCodeCategoryService>();
        services.AddScoped<ILoginWithQydhaOtpSenderService, LoginWithQydhaOtpSenderAsNotification>();
        services.AddScoped<IBalootGamesService, BalootGamesService>();
        #endregion
        return services;

    }
}
