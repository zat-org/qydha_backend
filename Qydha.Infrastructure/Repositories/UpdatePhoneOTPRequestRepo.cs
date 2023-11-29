namespace Qydha.Infrastructure.Repositories;

public class UpdatePhoneOTPRequestRepo : IUpdatePhoneOTPRequestRepo
{
    private readonly IDbConnection _dbConnection;
    public UpdatePhoneOTPRequestRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    private const string TableName = "update_phone_requests";
    private HashSet<string> DbAddExcludedProperties { get; } = new() { "Id" };
    private string DbColumns => string.Join(", ", typeof(UpdatePhoneRequest).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => p.Name));

    private string DbValues => string.Join(", ", typeof(UpdatePhoneRequest).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => $"@{p.Name}"));
    public async Task<Result<UpdatePhoneRequest>> AddAsync(UpdatePhoneRequest updatePhoneRequest)
    {
        var sql = @$"INSERT INTO  {TableName} 
                    ({DbColumns})
                    VALUES 
                    ({DbValues})
                    RETURNING Id;";

        var updatePhoneRequestId = await _dbConnection.QuerySingleAsync<Guid>(sql, updatePhoneRequest);
        updatePhoneRequest.Id = updatePhoneRequestId;
        return Result.Ok(updatePhoneRequest);
    }

    public async Task<Result<UpdatePhoneRequest>> GetByIdAsync(Guid reqId)
    {
        var sql = @$"SELECT * FROM {TableName}
                    WHERE id = @reqId";
        var updatePhoneRequest = await _dbConnection.QuerySingleOrDefaultAsync<UpdatePhoneRequest?>(sql, new { reqId });
        if (updatePhoneRequest is null)
            return Result.Fail<UpdatePhoneRequest>(
                new()
                {
                    Code = ErrorCodes.UpdatePhoneRequestNotFound,
                    Message = "Update Phone Request not Found"
                }
            );
        return Result.Ok(updatePhoneRequest);
    }

}
