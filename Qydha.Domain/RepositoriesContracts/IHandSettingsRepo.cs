namespace Qydha.Domain.Repositories;

public interface IUserHandSettingsRepo
{
    Task<Result<UserHandSettings>> GetByUserIdAsync(Guid userId);
    Task<Result<UserHandSettings>> UpdateByUserIdAsync(Guid userId, UserHandSettings settings);
}
