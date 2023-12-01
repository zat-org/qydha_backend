
namespace Qydha.API.Controllers.Attributes;

public class AuthFilter : IAuthorizationFilter
{
    private readonly IUserRepo _userRepo;
    public AuthFilter(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }
    public void OnAuthorization(AuthorizationFilterContext ctx)
    {
        // Console.WriteLine(">>>>>>>>>>>> From AuthFilter <<<<<<<<<<<");
        var userIdStr = ctx.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userIdStr is null || !Guid.TryParse(userIdStr, out Guid userId))
            ctx.Result = new UnauthorizedObjectResult(new
            {
                Code = ErrorCodes.InvalidToken,
                Message = "Invalid Token Data"
            });

        // var getUserRes = await _userRepo.GetByIdAsync(userId);
        else
        {
            ctx.HttpContext.Items["UserId"] = userId;
        }
    }
}
