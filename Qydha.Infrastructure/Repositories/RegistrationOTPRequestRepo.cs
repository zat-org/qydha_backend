namespace Qydha.Infrastructure.Repositories;
public class RegistrationOTPRequestRepo : IRegistrationOTPRequestRepo
{
    private readonly IDbConnection _dbConnection;
    public RegistrationOTPRequestRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    private const string TableName = "Registration_OTP_Request";
    private HashSet<string> DbAddExcludedProperties { get; } = new() { "Id" };

    private string DbColumns
        => string.Join(", ", typeof(RegistrationOTPRequest).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => p.Name));
    private string DbValues
        => string.Join(", ", typeof(RegistrationOTPRequest).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => $"@{p.Name}"));

    public async Task<Result<RegistrationOTPRequest>> AddAsync(RegistrationOTPRequest registrationOTPRequest)
    {
        var sql = @$"INSERT INTO  {TableName} 
                    ({DbColumns})
                    VALUES 
                    ({DbValues})
                    RETURNING Id;";

        var registrationOTPRequestId = await _dbConnection.QuerySingleAsync<Guid>(sql, registrationOTPRequest);
        registrationOTPRequest.Id = registrationOTPRequestId;
        return Result.Ok(registrationOTPRequest);
    }

    public async Task<Result<RegistrationOTPRequest>> GetByIdAsync(Guid reqId)
    {
        var sql = @$"SELECT * FROM {TableName}
                    WHERE id = @reqId";
        var registrationOTPRequest = await _dbConnection.QuerySingleOrDefaultAsync<RegistrationOTPRequest?>(sql, new { reqId });
        if (registrationOTPRequest is null)
            return Result.Fail<RegistrationOTPRequest>(
                new()
                {
                    Code = ErrorCodes.RegistrationRequestNotFound,
                    Message = "Registration Request not Found"
                }
            );
        return Result.Ok(registrationOTPRequest);
    }


}
