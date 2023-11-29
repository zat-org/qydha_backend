namespace Qydha.Domain.Repositories;

public interface IRegistrationOTPRequestRepo
{
    Task<Result<RegistrationOTPRequest>> AddAsync(RegistrationOTPRequest registrationOTPRequest);
    Task<Result<RegistrationOTPRequest>> GetByIdAsync(Guid id);
}
