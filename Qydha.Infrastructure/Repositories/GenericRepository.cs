namespace Qydha.Infrastructure.Repositories;

public class GenericRepository<T>(IDbConnection dbConnection) : IGenericRepository<T> where T : class
{
    private readonly IDbConnection _dbConnection = dbConnection;

    #region handle reflection of entities
    private static string GetTableName()
    {
        Type type = typeof(T);
        TableAttribute? tableAttr = type.GetCustomAttribute<TableAttribute>();
        return tableAttr?.Name ?? type.Name + "s";
    }
    private static string GetKeyColumnName()
    {
        PropertyInfo? keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault() is KeyAttribute);

        if (keyProperty is null) return "Id";

        ColumnAttribute? columnAttr = keyProperty.GetCustomAttribute<ColumnAttribute>();
        return columnAttr?.Name ?? keyProperty.Name;
    }
    private string GetKeyPropertyName()
    {
        PropertyInfo? keyProperty = typeof(T).GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault() is KeyAttribute);
        return keyProperty?.Name ?? "Id";
    }

    private static string GetColumns(bool excludeKey = false)
    {
        Type type = typeof(T);
        return string.Join(", ",
                type.GetProperties()
                    .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                    .Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name)
                    .Where(name => name is not null));
    }

    private string GetPropertyNames(bool excludeKey = false)
    {
        Type type = typeof(T);
        return string.Join(", ",
                type.GetProperties()
                    .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                    .Where(p => p.GetCustomAttribute<ColumnAttribute>() is not null)
                    .Select(p => $"@{p.Name}"));
    }

    private static string GetColumnsAndPropsForPut(bool excludeKey = false)
    {
        Type type = typeof(T);
        return string.Join(", ",
                type.GetProperties()
                    .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                    .Select(p =>
                    {
                        string? dbName = p.GetCustomAttribute<ColumnAttribute>()?.Name;
                        if (dbName is null) return null;
                        return $"{dbName} = @{p.Name}";
                    }).Where(name => name is not null));
    }


    private IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false) =>
        typeof(T).GetProperties()
            .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)));

    #endregion

    public async Task<Result<IdT>> AddAsync<IdT>(T entity)
    {
        try
        {
            string tableName = GetTableName();
            string columns = GetColumns(excludeKey: true);
            string properties = GetPropertyNames(excludeKey: true);
            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({properties}) RETURNING Id;";
            IdT entityId = await _dbConnection.QuerySingleAsync<IdT>(query, entity);
            return Result.Ok(entityId);
        }
        catch (Exception exp)
        {
            return Result.Fail<IdT>(new()
            {
                Code = ErrorCodes.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }

    public async Task<Result> DeleteByIdAsync<IdT>(IdT entityId)
    {
        try
        {
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();
            string query = $"DELETE FROM {tableName} WHERE {keyColumn} = @Id";
            int effectedRows = await _dbConnection.ExecuteAsync(query, new { Id = entityId });
            return effectedRows == 1 ?
                Result.Ok() :
                Result.Fail(new()
                {
                    Code = ErrorCodes.NotFound,
                    Message = "Entity not found"
                });
        }
        catch (Exception exp)
        {
            return Result.Fail(new()
            {
                Code = ErrorCodes.ServerErrorOnDB,
                Message = exp.Message
            });
        }

    }

    public async Task<Result<IEnumerable<T>>> GetAllAsync(Func<T, bool>? criteriaFunc = null)
    {
        try
        {
            string tableName = GetTableName();
            var sql = @$"SELECT * FROM {tableName};";
            IEnumerable<T> entities = await _dbConnection.QueryAsync<T>(sql);
            if (criteriaFunc is not null) entities = entities.Where(criteriaFunc);
            return Result.Ok(entities);
        }
        catch (Exception exp)
        {
            return Result.Fail<IEnumerable<T>>(new()
            {
                Code = ErrorCodes.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }
    public async Task<Result<IEnumerable<T>>> GetAllAsync(int pageSize = 10, int pageNumber = 1, Func<T, bool>? criteriaFunc = null)
    {
        return (await GetAllAsync(criteriaFunc))
        .OnSuccess<IEnumerable<T>>((entities) =>
            Result.Ok(entities.Skip((pageNumber - 1) * pageSize).Take(pageSize)));
    }

    public async Task<Result<T>> GetByUniquePropAsync<IdT>(PropertyInfo propertyInfo, IdT propValue)
    {
        ColumnAttribute columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>() ?? throw new InvalidOperationException("Invalid property. there is no Column Attribute!");

        if (columnAttribute.Name is null)
            throw new InvalidOperationException("Invalid property. there is no db Column name !");

        if (propertyInfo.PropertyType != typeof(IdT))
            throw new InvalidOperationException("Invalid property value.");

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PropVariable", propValue);
            string tableName = GetTableName();
            var sql = @$"SELECT * from {tableName}
                         where {columnAttribute.Name} = @PropVariable;";
            T? entity = await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, parameters);
            if (entity is null)
                return Result.Fail<T>(new()
                {
                    Code = ErrorCodes.NotFound,
                    Message = "Entity Not Found"
                });
            return Result.Ok(entity);
        }
        catch (Exception exp)
        {
            return Result.Fail<T>(new()
            {
                Code = ErrorCodes.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }

    public async Task<Result<T>> PutByIdAsync(T entity)
    {
        try
        {
            var sql = @$"UPDATE {GetTableName()} 
                    SET {GetColumnsAndPropsForPut()}
                    WHERE {GetKeyColumnName()} = @{GetKeyPropertyName()};";
            var effectedRows = await _dbConnection.ExecuteAsync(sql, entity);
            return effectedRows == 1 ?
                Result.Ok(entity) :
                Result.Fail<T>(new Error
                {
                    Code = ErrorCodes.NotFound,
                    Message = "Entity not Found"
                });
        }
        catch (Exception exp)
        {
            return Result.Fail<T>(new()
            {
                Code = ErrorCodes.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }

    public async Task<Result> PatchById<IdT>(IdT entityId, IEnumerable<PropertyInfo> properties, object obj)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", entityId);
        var propsNamesInQueryList = new List<string>();

        foreach (var prop in properties)
        {
            ColumnAttribute columnAttribute = prop.GetCustomAttribute<ColumnAttribute>() ?? throw new InvalidOperationException("Invalid property. there is no Column Attribute!");

            if (columnAttribute.Name is null)
                throw new InvalidOperationException("Invalid property. there is no db Column name !");
            parameters.Add($"@{prop.Name}", prop.GetValue(obj));
            propsNamesInQueryList.Add($"{columnAttribute.Name} = @{prop.Name}");
        }

        try
        {
            var sql = @$"UPDATE {GetTableName()} 
                     SET {string.Join(",", propsNamesInQueryList)} 
                     WHERE {GetKeyColumnName()} = @id;";
            var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
            return effectedRows == 1 ?
               Result.Ok() :
               Result.Fail(new Error
               {
                   Code = ErrorCodes.NotFound,
                   Message = "Entity not Found"
               });
        }
        catch (Exception exp)
        {
            return Result.Fail(new()
            {
                Code = ErrorCodes.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }

    public async Task<Result> PatchById<IdT, PropT>(IdT entityId, PropertyInfo propertyInfo, PropT propValue)
    {
        ColumnAttribute columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>() ?? throw new InvalidOperationException("Invalid property. there is no Column Attribute!");

        if (columnAttribute.Name is null)
            throw new InvalidOperationException("Invalid property. there is no db Column name !");

        if (propertyInfo.PropertyType != typeof(IdT))
            throw new InvalidOperationException("Invalid property value.");

        try
        {
            string tableName = GetTableName();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", entityId);
            parameters.Add("@PropVariable", propValue);
            var sql = @$"UPDATE {tableName} 
                     SET {columnAttribute.Name} = @PropVariable
                     WHERE {GetKeyColumnName()} = @Id;";
            var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
            return effectedRows == 1 ?
              Result.Ok() :
              Result.Fail(new()
              {
                  Code = ErrorCodes.NotFound,
                  Message = "Entity not Found"
              });
        }
        catch (Exception exp)
        {
            return Result.Fail<T>(new()
            {
                Code = ErrorCodes.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }


}
//         PropertyInfo propertyInfo = myObject.GetType().GetProperty("MyProperty");
