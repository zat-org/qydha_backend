
namespace Qydha.Infrastructure.Repositories;

public class PhoneAuthenticationRequestRepo : IPhoneAuthenticationRequestRepo
{
    private readonly IDbConnection _dbConnection;
    public PhoneAuthenticationRequestRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    private const string TableName = "Phone_Authentication_Requests";
    private HashSet<string> DbAddExcludedProperties { get; } = new() { "Id" };

    private string DbColumns
        => string.Join(", ", typeof(PhoneAuthenticationRequest).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => p.Name));
    private string DbValues
        => string.Join(", ", typeof(PhoneAuthenticationRequest).GetProperties()
                .Where((p) => !DbAddExcludedProperties.Contains(p.Name))
                .Select(p => $"@{p.Name}"));
    public async Task<Result<PhoneAuthenticationRequest>> AddAsync(PhoneAuthenticationRequest phoneAuthenticationRequest)
    {
        var sql = @$"INSERT INTO  {TableName} 
                    ({DbColumns})
                    VALUES 
                    ({DbValues})
                    RETURNING Id;";

        var id = await _dbConnection.QuerySingleAsync<Guid>(sql, phoneAuthenticationRequest);
        phoneAuthenticationRequest.Id = id;
        return Result.Ok(phoneAuthenticationRequest);
    }

    public async Task<Result<PhoneAuthenticationRequest>> GetByIdAsync(Guid reqId)
    {
        var sql = @$"SELECT * FROM {TableName}
                    WHERE id = @reqId";
        var phoneAuthenticationRequest = await _dbConnection.QuerySingleOrDefaultAsync<PhoneAuthenticationRequest?>(sql, new { reqId });
        if (phoneAuthenticationRequest is null)
            return Result.Fail<PhoneAuthenticationRequest>(
                new()
                {
                    Code = ErrorCodes.PhoneAuthenticationRequestNotFound,
                    Message = "Phone Authentication Request Not Found"
                }
            );
        return Result.Ok(phoneAuthenticationRequest);
    }
}
