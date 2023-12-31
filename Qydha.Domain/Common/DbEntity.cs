namespace Qydha.Domain.Common;

public abstract class DbEntity<T> where T : class
{
    public static string GetTableName()
    {
        Type type = typeof(T);
        TableAttribute? tableAttr = type.GetCustomAttribute<TableAttribute>();
        return tableAttr?.Name ?? type.Name + "s";
    }
    public static string GetColumnName(string propName)
    {
        PropertyInfo prop = typeof(T).GetProperty(propName) ?? throw new InvalidOperationException("Invalid property. there is no Property provided!");
        ColumnAttribute? columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
        return columnAttr?.Name ?? prop.Name;
    }
    public static string GetKeyColumnName()
    {
        PropertyInfo? keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault() is KeyAttribute);

        if (keyProperty is null) return "Id";

        ColumnAttribute? columnAttr = keyProperty.GetCustomAttribute<ColumnAttribute>();
        return columnAttr?.Name ?? keyProperty.Name;
    }
    public static PropertyInfo? GetKeyProperty()
    {
        PropertyInfo? keyProperty = typeof(T).GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault() is KeyAttribute);
        return keyProperty;
    }

    public static string GetKeyPropertyName() => GetKeyProperty()?.Name ?? "Id";


    public static string GetColumns(bool excludeKey = false)
    {
        Type type = typeof(T);
        return string.Join(", ",
                type.GetProperties()
                    .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                    .Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name)
                    .Where(name => name is not null));
    }

    public static string GetPropertyNames(bool excludeKey = false)
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

    public static Tuple<string, string, Dictionary<string, object?>> GetInsertQueryData(T entity, bool excludeKey = false, string prefix = "")
    {
        List<string> columnsNames = [];
        List<string> propsNames = [];
        Dictionary<string, object?> parameters = new();

        Type type = typeof(T);
        foreach (PropertyInfo propInfo in type.GetProperties())
        {
            ColumnAttribute? columnAttr = propInfo.GetCustomAttribute<ColumnAttribute>();
            if ((!excludeKey || !propInfo.IsDefined(typeof(KeyAttribute))) && columnAttr?.Name is not null)
            {
                string jsonb = propInfo.GetCustomAttribute<JsonFieldAttribute>() is not null ? " ::jsonb" : "";
                columnsNames.Add(columnAttr.Name);
                propsNames.Add($"@{prefix}{columnAttr.Name} {jsonb} ");
                parameters.Add($"@{prefix}{columnAttr.Name}", propInfo.GetValue(entity));
            }
        }
        return new Tuple<string, string, Dictionary<string, object?>>(string.Join(", ", columnsNames), string.Join(", ", propsNames), parameters);
    }

    public static string GetColumnsAndPropsForPut(bool excludeKey = false)
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
    public static string GetColumnsAndPropsForGet(bool excludeKey = false)
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


    public static IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false) =>
        typeof(T).GetProperties()
            .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)));

}
