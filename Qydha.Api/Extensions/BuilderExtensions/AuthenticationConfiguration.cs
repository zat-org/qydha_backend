using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Qydha.API.Extensions;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        JWTSettings jwtOptions = configuration.GetSection("Authentication").Get<JWTSettings>() ??
            throw new NullReferenceException("can't access Authentication 'jwt' Settings");


        services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        services.AddScoped<IAuthorizationHandler, SubscribedUserPolicyHandler>();
        services.AddScoped<IAuthorizationHandler, AdminOrPermissionPolicyHandler>();
        services.AddScoped<IAuthorizationHandler, AdminOrSubscribedUserPolicyHandler>();
        services.AddScoped<IAuthorizationHandler, UserOrPermissionPolicyHandler>();



        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyConstants.ServiceAccountPermission, policy => policy.Requirements.Add(new PermissionRequirement()))
            .AddPolicy(PolicyConstants.SubscribedUser, policy => policy.Requirements.Add(new SubscribedUserPolicyRequirement()))
            .AddPolicy(PolicyConstants.AdminOrServiceAccount, policy => policy.Requirements.Add(new AdminOrPermissionPolicyRequirement()))
            .AddPolicy(PolicyConstants.AdminOrSubscribedUser, policy => policy.Requirements.Add(new AdminOrSubscribedUserPolicyRequirement()))
            .AddPolicy(PolicyConstants.UserOrServiceAccount, policy => policy.Requirements.Add(new UserOrPermissionPolicyRequirement()));


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
                ClockSkew = TimeSpan.Zero,
                LifetimeValidator = (notBefore, expires, token, parameters) =>
                {
                    if (token is not JsonWebToken jwtToken) return false;
                    var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimsNamesConstants.TokenType)?.Value;
                    return tokenType == ServiceAccount.TokenType || notBefore <= DateTime.UtcNow && expires > DateTime.UtcNow;
                },
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
    public const string Streamer = nameof(UserRoles.Streamer);
    public const string UserWithAnyRole = nameof(UserRoles.StaffAdmin) + "," + nameof(UserRoles.SuperAdmin) + "," + nameof(UserRoles.User);
    public const string Admin = nameof(UserRoles.StaffAdmin) + "," + nameof(UserRoles.SuperAdmin);
}

public static class PolicyConstants
{
    public const string AdminOrSubscribedUser = "AdminOrSubscribedUser";
    public const string AdminOrServiceAccount = "AdminOrServiceAccountPermission";
    public const string ServiceAccountPermission = "ServiceAccountPermission";
    public const string SubscribedUser = "SubscribedUser";
    public const string UserOrServiceAccount = "UserOrServiceAccount";

}

