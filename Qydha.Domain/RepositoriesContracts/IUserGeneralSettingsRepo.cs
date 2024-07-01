﻿namespace Qydha.Domain.Repositories;

public interface IUserGeneralSettingsRepo
{
    Task<Result<UserGeneralSettings>> GetByUserIdAsync(Guid userId);
    Task<Result<UserGeneralSettings>> UpdateByUserIdAsync(Guid userId, UserGeneralSettings settings);
}

