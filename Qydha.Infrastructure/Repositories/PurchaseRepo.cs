﻿
namespace Qydha.Infrastructure.Repositories;
public class PurchaseRepo(IDbConnection dbConnection, ILogger<PurchaseRepo> logger) : GenericRepository<Purchase>(dbConnection, logger), IPurchaseRepo
{
    public async Task<Result<IEnumerable<Purchase>>> GetAllByUserIdAsync(Guid userId)
    {
        return await GetAllAsync($"{Purchase.GetColumnName(nameof(Purchase.UserId))} = @userId",
                            new { userId },
                            $"{Purchase.GetColumnName(nameof(Purchase.PurchaseDate))} DESC");
    }

    public async Task<Result<int>> GetInfluencerCodeUsageByUserIdCountAsync(Guid userId, string code)
    {
        try
        {
            var sql = @$"SELECT Count(*) FROM {Purchase.GetTableName()} 
                        where {Purchase.GetColumnName(nameof(Purchase.UserId))} = @userId AND
                        {Purchase.GetColumnName(nameof(Purchase.ProductSku))} = @code AND
                        {Purchase.GetColumnName(nameof(Purchase.Type))} = @type ;";
            _logger.LogTrace("Before Execute Query :: {sql}", sql);
            int num = await _dbConnection.ExecuteScalarAsync<int>(sql, new { userId, code, type = "Influencer" });
            return Result.Ok(num);
        }
        catch (DbException exp)
        {
            _logger.LogCritical(exp, "Error from db : {msg} ", exp.Message);
            throw;
        }
    }

    public async Task<Result<int>> GetInfluencerCodeUsageByAllUsersCountAsync(string code)
    {
        try
        {
            var sql = @$"SELECT Count(*) FROM {Purchase.GetTableName()} 
                        WHERE 
                        {Purchase.GetColumnName(nameof(Purchase.ProductSku))} = @code AND
                        {Purchase.GetColumnName(nameof(Purchase.Type))} = @type ;";
            _logger.LogTrace("Before Execute Query :: {sql}", sql);
            int num = await _dbConnection.ExecuteScalarAsync<int>(sql, new { code, type = "Influencer" });
            return Result.Ok(num);
        }
        catch (DbException exp)
        {
            _logger.LogCritical(exp, "Error from db : {msg} ", exp.Message);
            throw;
        }
    }

    public async Task<Result<int>> GetInfluencerCodeUsageCountByCategoryForUserAsync(Guid userId, int categoryId)
    {
        try
        {
            var sql = @$"                    
                    SELECT Count(*) FROM {Purchase.GetTableName()} 
                        WHERE 
                        {Purchase.GetColumnName(nameof(Purchase.UserId))} = @UserId AND
                        {Purchase.GetColumnName(nameof(Purchase.Type))} = @Type AND
                        {Purchase.GetColumnName(nameof(Purchase.ProductSku))} IN (
                            SELECT {InfluencerCode.GetColumnName(nameof(InfluencerCode.Code))} 
                            FROM {InfluencerCode.GetTableName()} 
                            WHERE {InfluencerCode.GetColumnName(nameof(InfluencerCode.CategoryId))} = @CategoryId 
                        );";
            _logger.LogTrace("Before Execute Query :: {sql}", sql);
            int num = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                UserId = userId,
                CategoryId = categoryId,
                Type = "Influencer"
            });
            return Result.Ok(num);
        }
        catch (DbException exp)
        {
            _logger.LogCritical(exp, "Error from db : {msg} ", exp.Message);
            throw;
        }
    }
}
