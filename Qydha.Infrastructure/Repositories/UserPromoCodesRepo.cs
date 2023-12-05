
namespace Qydha.Infrastructure.Repositories;

public class UserPromoCodesRepo(IDbConnection dbConnection) : IUserPromoCodesRepo
{
    private readonly IDbConnection _dbConnection = dbConnection;
    private const string TableName = "User_Promo_Codes";
    private HashSet<string> DbAddExcludedProperties { get; } = new() { "Id" };

    private string DbColumns
        => string.Join(", ", typeof(UserPromoCode).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => p.Name));
    private string DbValues
        => string.Join(", ", typeof(UserPromoCode).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => $"@{p.Name}"));

    public async Task<Result<UserPromoCode>> AddAsync(UserPromoCode userPromoCode)
    {
        var sql = @$"INSERT INTO  {TableName} 
                    ({DbColumns})
                    VALUES 
                    ({DbValues})
                    RETURNING Id;";

        var id = await _dbConnection.QuerySingleAsync<Guid>(sql, userPromoCode);
        userPromoCode.Id = id;
        return Result.Ok(userPromoCode);
    }

    public async Task<Result<UserPromoCode>> GetByIdAsync(Guid promoId)
    {
        var sql = @$"SELECT * FROM {TableName}
                     WHERE  id = @promoId ";
        var userPromoCode = await _dbConnection.QuerySingleOrDefaultAsync<UserPromoCode?>(sql, new { promoId });
        if (userPromoCode is null)
            return Result.Fail<UserPromoCode>(
                new()
                {
                    Code = ErrorCodes.UserPromoCodeNotFound,
                    Message = "User Promo Code Not Found"
                }
            );
        return Result.Ok(userPromoCode);
    }

    public async Task<Result<IEnumerable<UserPromoCode>>> GetAllUserValidPromoCodeAsync(Guid userId)
    {
        var sql = @$"SELECT * FROM {TableName}
                     WHERE  user_id = @userId and used_at is null and CURRENT_DATE <= Date(expire_at) 
                     order by expire_at ; ";
        IEnumerable<UserPromoCode> userPromoCode = await _dbConnection.QueryAsync<UserPromoCode>(sql, new { userId });
        if (userPromoCode is null)
            return Result.Fail<IEnumerable<UserPromoCode>>(
                new()
                {
                    Code = ErrorCodes.UserPromoCodeNotFound,
                    Message = "User Promo Code Not Found"
                }
            );
        return Result.Ok(userPromoCode);
    }

    public async Task<Result> PatchById(Guid codeId, Dictionary<string, object> props)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", codeId);
        var propsNamesInQueryList = new List<string>();
        foreach (var prop in props)
        {
            parameters.Add($"@{prop.Key}", prop.Value);
            propsNamesInQueryList.Add($"{prop.Key} = @{prop.Key}");
        }
        var sql = @$"UPDATE {TableName} 
                     SET {string.Join(",", propsNamesInQueryList)} 
                     WHERE id = @id ;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
        return effectedRows == 1 ?
           Result.Ok() :
           Result.Fail(new Error { Code = ErrorCodes.UserPromoCodeNotFound, Message = "User Promo Code Not Found" });
    }
    public async Task<Result> PatchById<T>(Guid codeId, string propName, T propValue)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", codeId);
        parameters.Add($"@{propName}", propValue);
        var sql = @$"UPDATE {TableName} 
                     SET {propName} = @{propName}
                     WHERE id = @id;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
        return effectedRows == 1 ?
          Result.Ok() :
          Result.Fail(new Error { Code = ErrorCodes.UserPromoCodeNotFound, Message = "User Promo Code Not Found" });
    }



}
