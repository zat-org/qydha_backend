namespace Qydha.Domain.Settings;

public class ProductsSettings
{
    public Dictionary<string, int> ProductsSku { get; set; } = null!;

    public Result<int> GetNumberOfDaysForProductSku(string productSku, string purchaseId)
    {
        if (!ProductsSku.TryGetValue(productSku, out int numberOfDays))
            return Result.Fail(new InvalidProductSkuError(productSku, purchaseId));
        return Result.Ok(numberOfDays);
    }
}
public class InvalidProductSkuError(string productSku, string purchaseId)
 : ResultError($"Invalid ProductSku : '{productSku}' from Purchase with Id :{purchaseId}", ErrorType.InvalidProductSku, StatusCodes.Status400BadRequest)
{ }
