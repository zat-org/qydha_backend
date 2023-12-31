
namespace Qydha.Infrastructure.Repositories;

public class UserPromoCodesRepo(IDbConnection dbConnection, ILogger<UserPromoCodesRepo> logger) : GenericRepository<UserPromoCode>(dbConnection, logger), IUserPromoCodesRepo
{
    public async Task<Result<IEnumerable<UserPromoCode>>> GetAllUserValidPromoCodeAsync(Guid userId) =>
         await GetAllAsync(@$"{UserPromoCode.GetColumnName(nameof(UserPromoCode.UserId))} = @userId 
                                    And {UserPromoCode.GetColumnName(nameof(UserPromoCode.UsedAt))} is null 
                                    And CURRENT_DATE <= Date({UserPromoCode.GetColumnName(nameof(UserPromoCode.ExpireAt))})",
                                    new { userId },
                                    $"{UserPromoCode.GetColumnName(nameof(UserPromoCode.ExpireAt))}");


}