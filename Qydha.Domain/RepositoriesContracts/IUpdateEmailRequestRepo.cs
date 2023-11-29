namespace Qydha.Domain.Repositories;

public interface IUpdateEmailRequestRepo
{
    Task<Result<UpdateEmailRequest>> AddAsync(UpdateEmailRequest updateEmailRequest);
    Task<Result<UpdateEmailRequest>> GetByIdAsync(Guid id);
}
