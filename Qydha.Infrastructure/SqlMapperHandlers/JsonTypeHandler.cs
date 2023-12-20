
namespace Qydha.Infrastructure.SqlMappersHandlers
{
    public class JsonTypeHandler<T> : SqlMapper.TypeHandler<Json<T>>
    {
        public override void SetValue(IDbDataParameter parameter, Json<T>? value)
        {
            parameter.Value = value is null ? null : JsonConvert.SerializeObject(value.Value);
        }

        public override Json<T> Parse(object value)
        {
            if (value is string json)
            {
                return new Json<T>(JsonConvert.DeserializeObject<T>(json));
            }

            return new Json<T>(default);
        }
    }
}