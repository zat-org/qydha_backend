﻿namespace Qydha.Domain.Services.Contracts;

public interface IInfluencerCodesService
{
    Task<Result<InfluencerCode>> AddInfluencerCode(string code, int numOfDays, DateTime? expireDate, int maxInfluencedUsersCount, int? categoryId);
    Task<Result<User>> UseInfluencerCode(Guid userId, string code);

}
