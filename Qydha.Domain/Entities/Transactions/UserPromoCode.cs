namespace Qydha.Domain.Entities;
public class UserPromoCode
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public int NumberOfDays { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpireAt { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset? UsedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public UserPromoCode() { }
    public UserPromoCode(Guid userId, string code, int numberOfDays, DateTimeOffset expireAt)
    {
        UserId = userId;
        Code = code;
        NumberOfDays = numberOfDays;
        ExpireAt = expireAt;
        CreatedAt = DateTimeOffset.UtcNow;
        UsedAt = null;
    }

    public Result IsPromoCodeValidToUse(Guid userId)
    {
        if (userId != UserId)
            return Result.Fail(new UserDoesNotOwnThisPromoCodeError());
        if (UsedAt is not null)
            return Result.Fail(new PromoCodeAlreadyUsedError(UsedAt.Value));
        if (ExpireAt.Date < DateTimeOffset.UtcNow.Date)
            return Result.Fail(new PromoCodeExpiredError(ExpireAt));
        return Result.Ok();
    }
}

public class UserDoesNotOwnThisPromoCodeError()
    : ResultError($"Authenticated user doesn't own this promo code.", ErrorType.UserDoesNotOwnThePromoCode, StatusCodes.Status403Forbidden)
{ }

public class PromoCodeAlreadyUsedError(DateTimeOffset usedAt)
    : ResultError($"this promo code is already used at  {usedAt}", ErrorType.PromoCodeAlreadyUsed, StatusCodes.Status400BadRequest)
{ }
public class PromoCodeExpiredError(DateTimeOffset expireAt)
    : ResultError($"this promo code is Expired used at {expireAt} and can't used after that.", ErrorType.PromoCodeExpired, StatusCodes.Status400BadRequest)
{ }