namespace Qydha.Domain.Services.Implementation;

public class AdminUserService(IUserRepo userRepo, TokenManager tokenManager) : IAdminUserService
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly TokenManager _tokenManager = tokenManager;

    public async Task<Result<(User, string)>> Login(string username, string password)
    {
        return (await _userRepo.CheckUserCredentials(username, password))
        .OnSuccess((user) =>
        {
            if (user.Roles.Contains(UserRoles.SuperAdmin) || user.Roles.Contains(UserRoles.StaffAdmin))
                return Result.Ok(user);
            return Result.Fail(new ForbiddenError());
        })
        .OnSuccess((user) =>
        {
            var jwtToken = _tokenManager.Generate(user.GetClaims());
            return Result.Ok((user, jwtToken));
        });
    }
}
