namespace Qydha.API.Extensions;

public static class ServicesRegistrations
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {

        #region DI Services
        services.AddSingleton<TokenManager>();
        services.AddSingleton<OtpManager>();

        string storageServiceKey = configuration.GetSection("GoogleStorage").Get<GoogleStorageSettings>()?.JsonKeyPath
            ?? throw new NullReferenceException("can't get storage service key.");
        services.AddSingleton(new GoogleStorageService(storageServiceKey));

        services.AddHttpClient();

        // services.AddScoped<WaApiService>();
        // services.AddScoped<WhatsAppService>();
        // services.AddSingleton<WaApiInstancesTracker>();
        // services.AddScoped<IMessageService, OtpSenderByWhatsAppService>();

        if (environment.IsProduction())
            services.AddScoped<IMessageService, WhatsAppService>();
        else
            services.AddScoped<IMessageService, WaApiService>();

        services.AddScoped<IMailingService, MailingService>();
        services.AddScoped<IFileService, GoogleCloudFileService>();
        services.AddScoped<IPushNotificationService, FCMService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IUserPromoCodesService, UserPromoCodesService>();
        services.AddScoped<IInfluencerCodesService, InfluencerCodesService>();
        services.AddScoped<IAppAssetsService, AppAssetsService>();
        services.AddScoped<IInfluencerCodeCategoryService, InfluencerCodeCategoryService>();
        services.AddScoped<ILoginWithQydhaOtpSenderService, LoginWithQydhaOtpSenderAsNotification>();
        services.AddScoped<IBalootGamesService, BalootGamesService>();
        services.AddScoped<IServiceAccountsService, ServiceAccountsService>();
        #endregion
        return services;

    }
}
