namespace Qydha.Infrastructure.Repositories;
public class PurchaseRepo : IPurchaseRepo
{
    private readonly IDbConnection _dbConnection;
    public PurchaseRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    private const string TableName = "Purchases";
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
        return string.Join(", ", typeof(Purchase).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, dbAction))
                .Select(p => p.Name));
    }
    private string DbValues(DbAction dbAction)
    {
        return string.Join(", ", typeof(Purchase).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, dbAction))
                .Select(p => $"@{p.Name}"));
    }
    public async Task<Result<Purchase>> AddAsync(Purchase purchase)
    {
        var sql = @$"INSERT INTO  {TableName} 
                    ({DbColumns(DbAction.Add)})
                    VALUES 
                    ({DbValues(DbAction.Add)})
                    RETURNING Id;";

        var purchaseId = await _dbConnection.QuerySingleAsync<Guid>(sql, purchase);
        purchase.Id = purchaseId;
        return Result.Ok(purchase);
    }
}
