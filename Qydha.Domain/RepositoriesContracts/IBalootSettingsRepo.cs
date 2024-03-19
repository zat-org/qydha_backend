namespace Qydha.Domain.Repositories;

public interface IUserBalootSettingsRepo
{
    Task<Result<UserBalootSettings>> GetByUserIdAsync(Guid userId);
    Task<Result<UserBalootSettings>> UpdateByUserIdAsync(UserBalootSettings settings);
}
