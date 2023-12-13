namespace Qydha.Domain.Repositories;

public interface IGenericRepository<T>
{
    Task<Result<IdT>> AddAsync<IdT>(T entity);
    Task<Result> DeleteByIdAsync<IdT>(IdT entityId);
    Task<Result<IEnumerable<T>>> GetAllAsync(Func<T, bool>? criteriaFunc = null);
    Task<Result<IEnumerable<T>>> GetAllAsync(int pageSize = 10, int pageNumber = 1, Func<T, bool>? criteriaFunc = null);
    Task<Result<T>> GetByUniquePropAsync<IdT>(PropertyInfo propertyInfo, IdT propValue);
    Task<Result<T>> PutByIdAsync(T entity);
    Task<Result> PatchById<IdT>(IdT entityId, IEnumerable<PropertyInfo> properties, object obj);
    Task<Result> PatchById<IdT, PropT>(IdT entityId, PropertyInfo propertyInfo, PropT propValue);
}
