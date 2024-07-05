namespace Qydha.API.Extensions;

public static class ReposRegistrations
{
    public static IServiceCollection RegisterRepos(this IServiceCollection services)
    {

        #region DI Repos
        // repos 
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IRegistrationOTPRequestRepo, RegistrationOTPRequestRepo>();
        services.AddScoped<IUpdatePhoneOTPRequestRepo, UpdatePhoneOTPRequestRepo>();
        services.AddScoped<IPhoneAuthenticationRequestRepo, PhoneAuthenticationRequestRepo>();
        services.AddScoped<IUpdateEmailRequestRepo, UpdateEmailRequestRepo>();
        services.AddScoped<IPurchaseRepo, PurchaseRepo>();
        services.AddScoped<INotificationRepo, NotificationRepo>();
        services.AddScoped<IUserPromoCodesRepo, UserPromoCodesRepo>();
        services.AddScoped<IInfluencerCodesRepo, InfluencerCodesRepo>();
        services.AddScoped<IAppAssetsRepo, AppAssetsRepo>();
        services.AddScoped<IInfluencerCodesCategoriesRepo, InfluencerCodesCategoriesRepo>();
        services.AddScoped<ILoginWithQydhaRequestRepo, LoginWithQydhaRequestRepo>();
        services.AddScoped<IBalootGamesRepo, BalootGamesRepo>();
        return services;
        #endregion

    }
}
