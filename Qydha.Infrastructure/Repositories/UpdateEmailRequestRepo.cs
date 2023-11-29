namespace Qydha.Infrastructure.Repositories;

public class UpdateEmailRequestRepo : IUpdateEmailRequestRepo
{
    private readonly IDbConnection _dbConnection;

    public UpdateEmailRequestRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    private const string TableName = "update_email_requests";

    private HashSet<string> DbAddExcludedProperties { get; } = new() { };
    private string DbColumns => string.Join(", ", typeof(UpdateEmailRequest).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => p.Name));

    private string DbValues => string.Join(", ", typeof(UpdateEmailRequest).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => $"@{p.Name}"));


    public async Task<Result<UpdateEmailRequest>> AddAsync(UpdateEmailRequest updateEmailRequest)
    {
        var sql = @$"INSERT INTO  {TableName}
                    ({DbColumns})
                    VALUES 
                    ({DbValues})
                    RETURNING Id;";

        var updateEmailRequestId = await _dbConnection.QuerySingleAsync<Guid>(sql, updateEmailRequest);
        updateEmailRequest.Id = updateEmailRequestId;
        return Result.Ok(updateEmailRequest);
    }

    public async Task<Result<UpdateEmailRequest>> GetByIdAsync(Guid reqId)
    {
        var sql = @$"SELECT * FROM {TableName}
                    WHERE id = @reqId";
        var updateEmailRequest = await _dbConnection.QuerySingleOrDefaultAsync<UpdateEmailRequest?>(sql, new { reqId });
        if (updateEmailRequest is null)
            return Result.Fail<UpdateEmailRequest>(
                new()
                {
                    Code = ErrorCodes.UpdateEmailRequestNotFound,
                    Message = "Update Email Request not Found"
                }
            );
        return Result.Ok(updateEmailRequest);
    }

}
