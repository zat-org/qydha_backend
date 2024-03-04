namespace Qydha.Domain.Repositories;

public interface ILoginWithQydhaRequestRepo : IGenericRepository<LoginWithQydhaRequest>
{
    Task<Result> MarkRequestAsUsed(Guid requestId);
}
