namespace Qydha.Infrastructure.Repositories;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
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

    #region handle reflection of entities
    protected static string GetTableName()
    {
        Type type = typeof(T);
        TableAttribute? tableAttr = type.GetCustomAttribute<TableAttribute>();
        return tableAttr?.Name ?? type.Name + "s";
    }
    protected static string GetColumnName(string propName)
    {
        PropertyInfo prop = typeof(T).GetProperty(propName) ?? throw new InvalidOperationException("Invalid property. there is no Property provided!");
        ColumnAttribute? columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
        return columnAttr?.Name ?? prop.Name;
    }
    protected static string GetKeyColumnName()
    {
        PropertyInfo? keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault() is KeyAttribute);

        if (keyProperty is null) return "Id";

        ColumnAttribute? columnAttr = keyProperty.GetCustomAttribute<ColumnAttribute>();
        return columnAttr?.Name ?? keyProperty.Name;
    }
    protected static PropertyInfo? GetKeyProperty()
    {
        PropertyInfo? keyProperty = typeof(T).GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault() is KeyAttribute);
        return keyProperty;
    }
    protected static string GetKeyPropertyName() => GetKeyProperty()?.Name ?? "Id";


    protected static string GetColumns(bool excludeKey = false)
    {
        Type type = typeof(T);
        return string.Join(", ",
                type.GetProperties()
                    .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                    .Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name)
                    .Where(name => name is not null));
    }

    protected static string GetPropertyNames(bool excludeKey = false)
    {
        Type type = typeof(T);
        return string.Join(", ",
                type.GetProperties()
                    .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                    .Where(p => p.GetCustomAttribute<ColumnAttribute>() is not null)
                    .Select(p =>
                    {
                        string jsonb = p.GetCustomAttribute<JsonFieldAttribute>() is not null ? " ::jsonb" : "";
                        return $"@{p.Name} {jsonb} ";
                    }));
    }

    protected static string GetColumnsAndPropsForPut(bool excludeKey = false)
    {
        Type type = typeof(T);
        return string.Join(", ",
                type.GetProperties()
                    .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                    .Select(p =>
                    {
                        string? dbName = p.GetCustomAttribute<ColumnAttribute>()?.Name;
                        if (dbName is null) return null;
                        string jsonb = p.GetCustomAttribute<JsonFieldAttribute>() is not null ? " ::jsonb" : "";
                        return $"{dbName} = @{p.Name} {jsonb} ";
                    }).Where(name => name is not null));
    }
    protected static string GetColumnsAndPropsForGet(bool excludeKey = false)
    {
        Type type = typeof(T);
        return string.Join(", ",
                type.GetProperties()
                    .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                    .Select(p =>
                    {
                        string? dbName = p.GetCustomAttribute<ColumnAttribute>()?.Name;
                        if (dbName is null) return null;
                        return $"{dbName} as {p.Name}";
                    }).Where(name => name is not null));
    }


    protected static IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false) =>
        typeof(T).GetProperties()
            .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)));

    #endregion

    public virtual async Task<Result<T>> AddAsync<IdT>(T entity, bool excludeKey = true)
    {
        var keyProp = GetKeyProperty() ?? throw new InvalidOperationException("Can't Add Entity In Column Without Key");
        try
        {
            string tableName = GetTableName();
            string columns = GetColumns(excludeKey);
            string properties = GetPropertyNames(excludeKey);

            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({properties}) RETURNING {GetKeyColumnName()};";
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
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();
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
            var sql = @$"SELECT {GetColumnsAndPropsForGet(excludeKey: false)} FROM {GetTableName()};";
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
            var sql = @$"SELECT {GetColumnsAndPropsForGet(excludeKey: false)}
                         FROM {GetTableName()}
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
            var sql = @$"SELECT {GetColumnsAndPropsForGet(excludeKey: false)}
                         FROM {GetTableName()} 
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
            string tableName = GetTableName();
            var sql = @$"SELECT {GetColumnsAndPropsForGet(excludeKey: false)} from {tableName}
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
            var sql = @$"UPDATE {GetTableName()} 
                    SET {GetColumnsAndPropsForPut(excludeKey: true)}
                    WHERE {GetKeyColumnName()} = @{GetKeyPropertyName()};";
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
            var sql = @$"UPDATE {GetTableName()} 
                     SET {string.Join(",", propsNamesInQueryList)} 
                     WHERE {GetKeyColumnName()} = @id {criteria} ;";
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
            string tableName = GetTableName();
            var parameters = filterParams is null ? new DynamicParameters() : new DynamicParameters(filterParams);
            parameters.Add("@Id", entityId);
            parameters.Add("@PropVariable", propValue);
            var sql = @$"UPDATE {tableName} 
                     SET {columnAttribute.Name} = @PropVariable {jsonb}
                     WHERE {GetKeyColumnName()} = @Id {criteria} ;";
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
