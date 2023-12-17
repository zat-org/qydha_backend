
namespace Qydha.Infrastructure.Repositories;

public class UserPromoCodesRepo(IDbConnection dbConnection, ILogger<UserPromoCodesRepo> logger) : GenericRepository<UserPromoCode>(dbConnection, logger), IUserPromoCodesRepo
{
    public async Task<Result<IEnumerable<UserPromoCode>>> GetAllUserValidPromoCodeAsync(Guid userId) =>
         await GetAllAsync(@$"{GetColumnName(nameof(UserPromoCode.UserId))} = @userId 
                                    And {GetColumnName(nameof(UserPromoCode.UsedAt))} is null 
                                    And CURRENT_DATE <= Date({GetColumnName(nameof(UserPromoCode.ExpireAt))})",
                                    new { userId },
                                    $"{GetColumnName(nameof(UserPromoCode.ExpireAt))}");


}