namespace Qydha.Domain.Repositories;

public interface ILoginWithQydhaRequestRepo
{
    Task<Result> MarkRequestAsUsed(Guid requestId);
    Task<Result<LoginWithQydhaRequest>> AddAsync(LoginWithQydhaRequest request);
    Task<Result<LoginWithQydhaRequest>> GetByIdAsync(Guid requestId);
}
