namespace Qydha.Domain.Services.Implementation;

public class PurchaseService(IPurchaseRepo purchaseRepo, IMediator mediator, IUserRepo userRepo, IOptions<ProductsSettings> productSettings) : IPurchaseService
{
    private readonly IPurchaseRepo _purchaseRepo = purchaseRepo;
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IMediator _mediator = mediator;
    private readonly ProductsSettings _productsSettings = productSettings.Value;

    public async Task<Result<User>> AddPurchase(string purchaseId, Guid userId, string productSku, DateTimeOffset created_at)
    {
        Result<User> getUserRes = (await _userRepo.GetByIdAsync(userId))
                    .OnFailure((err) => new()
                    {
                        Code = err.Code,
                        Message = $"user id : {userId} provided in purchase Not Found and Purchase id : {purchaseId}"
                    });

        return getUserRes.OnSuccess((user) =>
        {
            if (!_productsSettings.ProductsSku.TryGetValue(productSku, out int numberOfDays))
            {
                return Result.Fail<Tuple<User, int>>(new()
                {
                    Code = ErrorType.InvalidProductSku,
                    Message = $"Invalid ProductSku : '{productSku}' from Purchase with Id :{purchaseId}"
                });
            }
            return Result.Ok(new Tuple<User, int>(user, numberOfDays));
        })
        .OnSuccessAsync(async (tuple) =>
        {
            Purchase purchase = new()
            { 
                IAPHubPurchaseId = purchaseId,
                UserId = tuple.Item1.Id,
                Type = "purchase",
                PurchaseDate = created_at,
                ProductSku = productSku,
                NumberOfDays = tuple.Item2
            };
            return (await _purchaseRepo.AddAsync(purchase)).MapTo(tuple.Item1);
        })
        .OnSuccessAsync<User>(async (user) =>
        {
            await _mediator.Publish(new AddTransactionNotification(user.Id, TransactionType.Purchase));
            return await _userRepo.UpdateUserExpireDate(userId);
        });
    }




}
