namespace Qydha.Domain.Repositories;

public interface IUpdatePhoneOTPRequestRepo
{
    Task<Result<UpdatePhoneRequest>> AddAsync(UpdatePhoneRequest request);
    Task<Result<UpdatePhoneRequest>> GetByIdAsync(Guid requestId);
}
