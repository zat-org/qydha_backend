namespace Qydha.Domain.Repositories;

public interface IGenericRepository<T>
{
    Task<Result<T>> AddAsync<IdT>(T entity);
    Task<Result> DeleteByIdAsync<IdT>(IdT entityId, string filterCriteria = "", object? filterParams = null);
    Task<Result<IEnumerable<T>>> GetAllAsync();
    Task<Result<IEnumerable<T>>> GetAllAsync(string filterCriteria, object parameters, string orderCriteria = "");
    Task<Result<IEnumerable<T>>> GetAllAsync(string filterCriteria, object parameters, int pageSize = 10, int pageNumber = 1, string orderCriteria = "");
    Task<Result<T>> GetByUniquePropAsync<IdT>(string propName, IdT propValue);
    Task<Result<T>> PutByIdAsync(T entity);
    Task<Result> PatchById<IdT>(IdT entityId, Dictionary<string, object> properties, string filterCriteria = "", object? filterParams = null);
    Task<Result> PatchById<IdT, PropT>(IdT entityId, string propName, PropT propValue, string filterCriteria = "", object? filterParams = null);
}