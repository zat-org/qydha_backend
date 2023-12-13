namespace Qydha.Infrastructure.Repositories;

public class InfluencerCodesRepo(IDbConnection dbConnection) : IInfluencerCodesRepo
{
    private readonly IDbConnection _dbConnection = dbConnection;
    #region table func.
    private const string TableName = "InfluencerCodes";
    private HashSet<string> DbAddExcludedProperties { get; } = new() { "Id" };
    private HashSet<string> DbUpdateExcludedProperties { get; } = new() { "Id" };
    private bool CheckPropIsExcluded(string propName, DbAction dbAction)
    {
        return dbAction switch
        {
            DbAction.Add => !DbAddExcludedProperties.Contains(propName),
            DbAction.Update => !DbUpdateExcludedProperties.Contains(propName),
            _ => false,
        };
    }
    private string DbColumns(DbAction dbAction)
    {
        return string.Join(", ", typeof(InfluencerCode).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, dbAction))
                .Select(p => p.Name));
    }
    private string DbValues(DbAction dbAction)
    {
        return string.Join(", ", typeof(InfluencerCode).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, dbAction))
                .Select(p => $"@{p.Name}"));
    }
    private string DbColumnsValuesForPUTUpdate()
    {
        return string.Join(", ", typeof(InfluencerCode).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, DbAction.Update))
                .Select(p => $"{p.Name} = @{p.Name}"));
    }
    #endregion

    #region AddInfluencerCode
    public async Task<Result<InfluencerCode>> AddAsync(InfluencerCode influencerCode)
    {
        string sql = @$"INSERT INTO  {TableName} ({DbColumns(DbAction.Add)})  VALUES ( {DbValues(DbAction.Add)}) RETURNING Id;";
        Guid id = await _dbConnection.QuerySingleAsync<Guid>(sql, influencerCode);
        influencerCode.Id = id;
        return Result.Ok(influencerCode);
    }
    #endregion

    #region DeleteInfluencerCode
    public async Task<Result> DeleteByIdAsync(Guid id)
    {
        var sql = @$"DELETE FROM {TableName} WHERE id = @Id;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, new { Id = id });
        return effectedRows == 1 ?
          Result.Ok() :
          Result.Fail(new Error { Code = ErrorCodes.UserNotFound, Message = "Influencer code not Found" });
    }
    #endregion

    #region getInfluencerCode

    public async Task<Result<IEnumerable<InfluencerCode>>> GetAsync(Func<InfluencerCode, bool>? criteriaFunc)
    {
        string sql = @$"SELECT * FROM {TableName} ;";

        IEnumerable<InfluencerCode> codes = await _dbConnection.QueryAsync<InfluencerCode>(sql);

        return Result.Ok(criteriaFunc is null ? codes : codes.Where(criteriaFunc));
    }
    private async Task<Result<InfluencerCode>> GetByPropAsync<T>(string propName, T propValue)
    {
        var parameters = new DynamicParameters();
        parameters.Add($"@{propName}", propValue);
        var sql = $"SELECT *  from {TableName} where {propName} = @{propName};";
        InfluencerCode? code = await _dbConnection.QuerySingleOrDefaultAsync<InfluencerCode>(sql, parameters);
        if (code is null)
            return Result.Fail<InfluencerCode>(new Error()
            {
                Code = ErrorCodes.InfluencerCodeNotFound,
                Message = "Influencer Code Not Found"
            });
        return Result.Ok(code);
    }
    public async Task<Result<InfluencerCode>> GetByIdAsync(Guid id)
    {
        return await GetByPropAsync("Id", id);
    }

    public async Task<Result<InfluencerCode>> GetByCodeAsync(string code)
    {
        return await GetByPropAsync("Normalized_Code", code.ToUpper());
    }

    public async Task<Result> IsCodeAvailable(string code)
    {
        Result<InfluencerCode> getCodeRes = await GetByCodeAsync(code);
        if (getCodeRes.IsSuccess)
            return Result.Fail(new Error { Code = ErrorCodes.DbUniqueViolation, Message = "هذا الكود موجود بالفعل" });
        return Result.Ok();
    }

    #endregion

    #region editInfluencerCode
    public async Task<Result<InfluencerCode>> PutByIdAsync(InfluencerCode code)
    {
        string sql = @$"UPDATE {TableName} 
                    SET {DbColumnsValuesForPUTUpdate()}
                    WHERE id = @id;";
        int effectedRows = await _dbConnection.ExecuteAsync(sql, code);
        return effectedRows == 1 ?
            Result.Ok(code) :
            Result.Fail<InfluencerCode>(new Error
            {
                Code = ErrorCodes.InfluencerCodeNotFound,
                Message = "Influencer Code not Found"
            });
    }
    private async Task<Result> PatchById(Guid id, Dictionary<string, object> props)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);
        var propsNamesInQueryList = new List<string>();
        foreach (var prop in props)
        {
            parameters.Add($"@{prop.Key}", prop.Value);
            propsNamesInQueryList.Add($"{prop.Key} = @{prop.Key}");
        }
        string sql = @$"UPDATE {TableName} 
                     SET {string.Join(",", propsNamesInQueryList)} 
                     WHERE id = @id;";
        int effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
        return effectedRows == 1 ?
           Result.Ok() :
           Result.Fail(new Error
           {
               Code = ErrorCodes.InfluencerCodeNotFound,
               Message = "Influencer Code User not Found"
           });
    }
    private async Task<Result> PatchById<T>(Guid id, string propName, T propValue)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);
        parameters.Add($"@{propName}", propValue);
        string sql = @$"UPDATE {TableName} 
                     SET {propName} = @{propName}
                     WHERE id = @id;";
        int effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
        return effectedRows == 1 ?
          Result.Ok() :
          Result.Fail(new Error { Code = ErrorCodes.InfluencerCodeNotFound, Message = "Influencer Code not Found" });
    }


    public async Task<Result> UpdateCode(Guid codeId, string code) =>
                    await PatchById(codeId, new() {
                        { "code", code },
                        { "Normalized_code", code.ToUpper() }
                    });
    public async Task<Result> UpdateExpireDate(Guid codeId, DateTime expireAt) =>
                   await PatchById(codeId, "expire_at", expireAt);

    public async Task<Result> UpdateNumberOfDays(Guid codeId, int numOfDays) =>
                       await PatchById(codeId, "number_of_days", numOfDays);


    #endregion

}
