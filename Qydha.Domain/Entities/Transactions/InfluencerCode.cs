namespace Qydha.Domain.Entities;

public class InfluencerCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string NormalizedCode { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ExpireAt { get; set; } // expire at == null => no expire limit 
    public int NumberOfDays { get; set; }
    public int MaxInfluencedUsersCount { get; set; } // 0 => no limit
    public int? CategoryId { get; set; }
    public InfluencerCodeCategory? Category { get; set; }
    public ICollection<InfluencerCodeUserLink> Users { get; set; } = [];
    public InfluencerCode() { }
    public InfluencerCode(string code, int numOfDays, DateTimeOffset? expireDate, int maxInfluencedUsers, int? categoryId)
    {
        Code = code;
        NormalizedCode = code.ToUpper();
        CreatedAt = DateTimeOffset.UtcNow;
        ExpireAt = expireDate;
        NumberOfDays = numOfDays;
        MaxInfluencedUsersCount = maxInfluencedUsers;
        CategoryId = categoryId;
    }
    public Result IsUserReachLimit(int usageCount)
    {
        if (usageCount > 0)
            return Result.Fail(new InfluencerCodeAlreadyUsedByAuthUserError());
        return Result.Ok();
    }
    public Result IsUsersReachedMaxUsage(int usageCount)
    {
        if (usageCount >= MaxInfluencedUsersCount)
            return Result.Fail(new InfluencerCodeExceedMaxUsageCountError(usageCount, MaxInfluencedUsersCount));
        return Result.Ok();
    }
    public Result IsUserReachedCategoryMaxUsage(int usageCount)
    {
        int maxUsageNum = Category!.MaxCodesPerUserInGroup;
        if (usageCount >= maxUsageNum)
            return Result.Fail(new InfluencerCodeCategoryAlreadyUsedError(usageCount, maxUsageNum));
        return Result.Ok();
    }
}

public class InfluencerCodeAlreadyUsedByAuthUserError()
    : ResultError($"Influencer code Already Used before", ErrorType.InfluencerCodeAlreadyUsed, StatusCodes.Status400BadRequest)
{ }
public class InfluencerCodeExceedMaxUsageCountError(int usersUsage, int maxUsage)
    : ResultError($"Influencer code Used {usersUsage} times by different users. this number is >= allowed number {maxUsage}", ErrorType.InfluencerCodeExceedMaxUsageCount, StatusCodes.Status400BadRequest)
{ }

public class InfluencerCodeCategoryAlreadyUsedError(int userUsage, int maxUsage)
    : ResultError($"Auth User Used codes from the category with count {userUsage}. this number is >= allowed number {maxUsage}", ErrorType.InfluencerCodeCategoryAlreadyUsed, StatusCodes.Status400BadRequest)
{ }
public class InfluencerCodeExpiredError(DateTimeOffset codeExpireAt)
    : ResultError($"Influencer Code already Expired at {codeExpireAt} current timestamp :{DateTimeOffset.UtcNow}", ErrorType.InfluencerCodeExpired, StatusCodes.Status400BadRequest)
{ }