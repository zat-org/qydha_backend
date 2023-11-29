namespace Qydha.Domain.Repositories;

public interface IUpdatePhoneOTPRequestRepo
{
    Task<Result<UpdatePhoneRequest>> AddAsync(UpdatePhoneRequest updatePhoneRequest);
    Task<Result<UpdatePhoneRequest>> GetByIdAsync(Guid id);

}
