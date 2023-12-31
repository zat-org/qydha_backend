namespace Qydha.Infrastructure.Repositories;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : DbEntity<T>
{
    protected readonly IDbConnection _dbConnection;
    protected readonly ILogger<GenericRepository<T>> _logger;
    protected readonly Type _type;
    protected readonly ErrorType _notFoundError;


    public GenericRepository(IDbConnection dbConnection, ILogger<GenericRepository<T>> logger)
    {
        _dbConnection = dbConnection;
        _logger = logger;
        _type = typeof(T);
        NotFoundErrorAttribute? notFoundErrorAttribute = _type.GetCustomAttribute<NotFoundErrorAttribute>();
        if (notFoundErrorAttribute is not null)
            _notFoundError = notFoundErrorAttribute.NotFoundErrorType;
        else
            _notFoundError = ErrorType.EntityNotFound;

    }

    public virtual async Task<Result<T>> AddAsync<IdT>(T entity, bool excludeKey = true)
    {

        var keyProp = DbEntity<T>.GetKeyProperty() ?? throw new InvalidOperationException("Can't Add Entity In Column Without Key");
        try
        {
            string tableName = DbEntity<T>.GetTableName();
            string columns = DbEntity<T>.GetColumns(excludeKey);
            string properties = DbEntity<T>.GetPropertyNames(excludeKey);

            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({properties}) RETURNING {DbEntity<T>.GetKeyColumnName()};";
            _logger.LogTrace($"Before Execute Query :: {query}");
            var entityId = await _dbConnection.QuerySingleAsync<IdT>(query, entity);
            keyProp.SetValue(entity, entityId);
            return Result.Ok(entity);
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail<T>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }
    public virtual async Task<Result> DeleteByIdAsync<IdT>(IdT entityId, string filterCriteria = "", object? filterParams = null)
    {
        var parameters = filterParams is null ? new DynamicParameters() : new DynamicParameters(filterParams);
        parameters.Add("@Id", entityId);
        string criteria = string.IsNullOrWhiteSpace(filterCriteria) ? "" : $"AND {filterCriteria}";
        try
        {
            string tableName = DbEntity<T>.GetTableName();
            string keyColumn = DbEntity<T>.GetKeyColumnName();
            string query = $"DELETE FROM {tableName} WHERE {keyColumn} = @Id {criteria} ;";
            _logger.LogTrace($"Before Execute Query :: {query}");
            int effectedRows = await _dbConnection.ExecuteAsync(query, parameters);
            return effectedRows == 1 ?
                Result.Ok() :
                Result.Fail(new()
                {
                    Code = _notFoundError,
                    Message = $"{_notFoundError} :: Entity not found"
                });
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }

    }

    #region Get All Async
    public async Task<Result<IEnumerable<T>>> GetAllAsync()
    {
        try
        {
            var sql = @$"SELECT {DbEntity<T>.GetColumnsAndPropsForGet(excludeKey: false)} FROM {DbEntity<T>.GetTableName()};";
            _logger.LogTrace($"Before Execute Query :: {sql}");
            IEnumerable<T> entities = await _dbConnection.QueryAsync<T>(sql);
            return Result.Ok(entities);
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail<IEnumerable<T>>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }

    public async Task<Result<IEnumerable<T>>> GetAllAsync(string filterCriteria, object parameters, string orderCriteria = "")
    {

        string criteria = string.IsNullOrWhiteSpace(filterCriteria) ? "" : $" WHERE {filterCriteria}";
        string orderString = string.IsNullOrWhiteSpace(orderCriteria) ? "" : $" ORDER BY  {orderCriteria}";
        try
        {
            var sql = @$"SELECT {DbEntity<T>.GetColumnsAndPropsForGet(excludeKey: false)}
                         FROM {DbEntity<T>.GetTableName()}
                         {criteria}
                         {orderString} ;";
            _logger.LogTrace($"Before Execute Query :: {sql}");

            IEnumerable<T> entities = await _dbConnection.QueryAsync<T>(sql, parameters);
            return Result.Ok(entities);
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail<IEnumerable<T>>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }
    public async Task<Result<IEnumerable<T>>> GetAllAsync(string filterCriteria, object parameters, int pageSize = 10, int pageNumber = 1, string orderCriteria = "")
    {
        string orderString = string.IsNullOrWhiteSpace(orderCriteria) ? "" : $" ORDER BY {orderCriteria}";
        string criteria = string.IsNullOrWhiteSpace(filterCriteria) ? "" : $" WHERE {filterCriteria}";
        var dynamicParameters = new DynamicParameters(parameters);
        dynamicParameters.Add("@Limit", pageSize);
        dynamicParameters.Add("@Offset", (pageNumber - 1) * pageSize);
        try
        {
            var sql = @$"SELECT {DbEntity<T>.GetColumnsAndPropsForGet(excludeKey: false)}
                         FROM {DbEntity<T>.GetTableName()} 
                         {criteria}
                         {orderString}
                         LIMIT @Limit OFFSET @Offset ;";
            _logger.LogTrace($"Before Execute Query :: {sql}");

            IEnumerable<T> entities = await _dbConnection.QueryAsync<T>(sql, dynamicParameters);
            return Result.Ok(entities);
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail<IEnumerable<T>>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }
    #endregion
    public virtual async Task<Result<T>> GetByUniquePropAsync<IdT>(string propName, IdT propValue)
    {
        PropertyInfo propertyInfo = _type.GetProperty(propName) ?? throw new InvalidOperationException("Invalid property. there is no Property provided!");

        ColumnAttribute columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>() ?? throw new InvalidOperationException("Invalid property. there is no Column Attribute!");

        if (columnAttribute.Name is null)
            throw new InvalidOperationException("Invalid property. there is no db Column name !");

        if (!propertyInfo.PropertyType.IsAssignableFrom(typeof(IdT)))
            throw new InvalidOperationException("Invalid property value.");

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PropVariable", propValue);
            string tableName = DbEntity<T>.GetTableName();
            var sql = @$"SELECT {DbEntity<T>.GetColumnsAndPropsForGet(excludeKey: false)} from {tableName}
                         where {columnAttribute.Name} = @PropVariable;";
            _logger.LogTrace($"Before Execute Query :: {sql}");
            T? entity = await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, parameters);
            if (entity is null)
                return Result.Fail<T>(new()
                {
                    Code = _notFoundError,
                    Message = $"{_notFoundError} :: Entity not found"
                });
            return Result.Ok(entity);
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail<T>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }

    public virtual async Task<Result<T>> PutByIdAsync(T entity)
    {
        try
        {
            var sql = @$"UPDATE {DbEntity<T>.GetTableName()} 
                    SET {DbEntity<T>.GetColumnsAndPropsForPut(excludeKey: true)}
                    WHERE {DbEntity<T>.GetKeyColumnName()} = @{DbEntity<T>.GetKeyPropertyName()};";
            _logger.LogTrace($"Before Execute Query :: {sql}");

            var effectedRows = await _dbConnection.ExecuteAsync(sql, entity);
            return effectedRows == 1 ?
                Result.Ok(entity) :
                Result.Fail<T>(new Error
                {
                    Code = _notFoundError,
                    Message = $"{_notFoundError} :: Entity not found"
                });
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail<T>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }

    public virtual async Task<Result> PatchById<IdT>(IdT entityId, Dictionary<string, object> properties, string filterCriteria = "", object? filterParams = null)
    {
        var parameters = filterParams is null ? new DynamicParameters() : new DynamicParameters(filterParams);
        parameters.Add("@id", entityId);

        var propsNamesInQueryList = new List<string>();

        foreach (var propKeyValue in properties)
        {
            string propName = propKeyValue.Key; //username 
            object propValue = propKeyValue.Value;

            PropertyInfo prop = _type.GetProperty(propName) ?? throw new InvalidOperationException("Invalid property. there is no Property provided!");

            ColumnAttribute columnAttribute = prop.GetCustomAttribute<ColumnAttribute>() ?? throw new InvalidOperationException("Invalid property. there is no Column Attribute!");

            if (columnAttribute.Name is null)
                throw new InvalidOperationException("Invalid property. there is no db Column name !");

            if (!prop.PropertyType.IsAssignableFrom(propValue.GetType()))
                throw new InvalidOperationException("Invalid property value.");

            parameters.Add($"@{prop.Name}", propValue);
            string jsonb = prop.GetCustomAttribute<JsonFieldAttribute>() is not null ? " ::jsonb" : "";
            propsNamesInQueryList.Add($"{columnAttribute.Name} = @{prop.Name} {jsonb}");

        }
        string criteria = string.IsNullOrWhiteSpace(filterCriteria) ? "" : $" AND {filterCriteria}";
        try
        {
            var sql = @$"UPDATE {DbEntity<T>.GetTableName()} 
                     SET {string.Join(",", propsNamesInQueryList)} 
                     WHERE {DbEntity<T>.GetKeyColumnName()} = @id {criteria} ;";
            _logger.LogTrace($"Before Execute Query :: {sql}");

            var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
            return effectedRows == 1 ?
               Result.Ok() :
               Result.Fail(new Error
               {
                   Code = _notFoundError,
                   Message = $"{_notFoundError} :: Entity not found"
               });
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }

    public virtual async Task<Result> PatchById<IdT, PropT>(IdT entityId, string propName, PropT propValue, string filterCriteria = "", object? filterParams = null)
    {
        PropertyInfo propertyInfo = _type.GetProperty(propName) ?? throw new InvalidOperationException("Invalid property. there is no Property provided!");

        ColumnAttribute columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>() ?? throw new InvalidOperationException("Invalid property. there is no Column Attribute!");

        if (columnAttribute.Name is null)
            throw new InvalidOperationException("Invalid property. there is no db Column name !");

        if (!propertyInfo.PropertyType.IsAssignableFrom(typeof(PropT)))
            throw new InvalidOperationException("Invalid property value.");

        string criteria = !string.IsNullOrWhiteSpace(filterCriteria) ? $" And {filterCriteria} " : "";

        string jsonb = propertyInfo.GetCustomAttribute<JsonFieldAttribute>() is not null ? " ::jsonb" : "";
        try
        {
            string tableName = DbEntity<T>.GetTableName();
            var parameters = filterParams is null ? new DynamicParameters() : new DynamicParameters(filterParams);
            parameters.Add("@Id", entityId);
            parameters.Add("@PropVariable", propValue);
            var sql = @$"UPDATE {tableName} 
                     SET {columnAttribute.Name} = @PropVariable {jsonb}
                     WHERE {DbEntity<T>.GetKeyColumnName()} = @Id {criteria} ;";
            _logger.LogTrace($"Before Execute Query :: {sql}");

            var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
            return effectedRows == 1 ?
              Result.Ok() :
              Result.Fail(new()
              {
                  Code = _notFoundError,
                  Message = $"{_notFoundError} :: Entity not found"
              });
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail<T>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }
}
