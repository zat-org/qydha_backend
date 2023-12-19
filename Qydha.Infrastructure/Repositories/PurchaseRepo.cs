namespace Qydha.Infrastructure.Repositories;
public class PurchaseRepo(IDbConnection dbConnection, ILogger<PurchaseRepo> logger) : GenericRepository<Purchase>(dbConnection, logger), IPurchaseRepo
{
    public async Task<Result<IEnumerable<Purchase>>> GetAllByUserIdAsync(Guid userId)
    {
        return await GetAllAsync($"{GetColumnName(nameof(Purchase.UserId))} = @userId",
                            new { userId },
                            $"{GetColumnName(nameof(Purchase.PurchaseDate))} DESC");
    }
    public async Task<Result<int>> GetInfluencerCodeUsageByUserIdCountAsync(Guid userId, string code)
    {
        try
        {
            var sql = @$"SELECT Count(*) FROM {GetTableName()} 
                        where {GetColumnName(nameof(Purchase.UserId))} = @userId AND
                        {GetColumnName(nameof(Purchase.ProductSku))} = @code AND
                        {GetColumnName(nameof(Purchase.Type))} = @type ;";
            _logger.LogInformation($"Before Execute Query :: {sql}");
            int num = await _dbConnection.ExecuteScalarAsync<int>(sql, new { userId, code, type = "Influencer" });
            return Result.Ok(num);
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, $"error from db : {exp.Message} ");
            return Result.Fail<int>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }
}
