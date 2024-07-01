using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Qydha.API.Extensions;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        JWTSettings jwtOptions = configuration.GetSection("Authentication").Get<JWTSettings>() ??
            throw new NullReferenceException("can't access Authentication 'jwt' Settings");

        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyConstants.CanReadBalootBook, builder =>
            {
                builder.RequireAssertion(context =>
                {
                    if (context.User.IsInRole(UserRoles.StaffAdmin.ToString()) || context.User.IsInRole(UserRoles.SuperAdmin.ToString()))
                        return true;
                    else if (context.User.IsInRole(UserRoles.User.ToString()))
                    {
                        var dateString = context.User.FindFirstValue("SubscriptionExpireDate");
                        if (dateString == null) return false;
                        var expireAt = DateTimeOffset.Parse(dateString);
                        return DateTimeOffset.UtcNow > expireAt;
                    }
                    else return false;
                });
            });

        services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, (options) =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretForKey)),
                ValidateLifetime = false, // TODO use refresh tokens 
            };
        });
        return services;
    }
}

public static class RoleConstants
{
    public const string SuperAdmin = nameof(UserRoles.SuperAdmin);
    public const string StaffAdmin = nameof(UserRoles.StaffAdmin);
    public const string User = nameof(UserRoles.User);
    public const string Admin = nameof(UserRoles.StaffAdmin) + "," + nameof(UserRoles.SuperAdmin);
}

public static class PolicyConstants
{
    public const string CanReadBalootBook = "CanReadBalootBook";

}