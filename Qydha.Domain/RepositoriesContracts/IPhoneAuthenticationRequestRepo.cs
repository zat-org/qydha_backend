namespace Qydha.Domain.Repositories;

public interface IPhoneAuthenticationRequestRepo
{
    Task<Result<PhoneAuthenticationRequest>> AddAsync(PhoneAuthenticationRequest request);
    Task<Result<PhoneAuthenticationRequest>> GetByIdAsync(Guid requestId);
}
