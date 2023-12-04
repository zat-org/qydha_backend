namespace Qydha.Domain.Repositories;

public interface IPhoneAuthenticationRequestRepo
{
    Task<Result<PhoneAuthenticationRequest>> AddAsync(PhoneAuthenticationRequest phoneAuthenticationRequest);
    Task<Result<PhoneAuthenticationRequest>> GetByIdAsync(Guid id);
}
