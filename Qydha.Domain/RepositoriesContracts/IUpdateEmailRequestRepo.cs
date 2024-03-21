namespace Qydha.Domain.Repositories;

public interface IUpdateEmailRequestRepo
{
    Task<Result<UpdateEmailRequest>> AddAsync(UpdateEmailRequest request);
    Task<Result<UpdateEmailRequest>> GetByIdAsync(Guid requestId);
    Task<Result> MarkRequestAsUsed(Guid requestId);
}
