namespace Qydha.Domain.Repositories;

public interface IRegistrationOTPRequestRepo
{
    Task<Result<RegistrationOTPRequest>> AddAsync(RegistrationOTPRequest request);
    Task<Result<RegistrationOTPRequest>> GetByIdAsync(Guid requestId);
    Task<Result> MarkRequestAsUsed(Guid requestId);
}
