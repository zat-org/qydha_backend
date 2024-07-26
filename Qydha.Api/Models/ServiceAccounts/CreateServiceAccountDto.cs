namespace Qydha.API.Models;
public class ServiceAccountDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<ServiceAccountPermission> Permissions { get; set; } = null!;

}
public class ServiceAccountDtoValidator : AbstractValidator<ServiceAccountDto>
{
    public ServiceAccountDtoValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(50).WithName("الاسم");

        RuleFor(r => r.Description).NotEmpty().MaximumLength(256).WithName("البريد الالكترونى");

        RuleForEach(r => r.Permissions).IsInEnum();

        RuleFor(r => r.Permissions).NotEmpty();
    }
}

public class ServiceAccountPage(List<ServiceAccount> items, int count, int pageNumber, int pageSize)
    : Page<ServiceAccount>(items, count, pageNumber, pageSize)
{ }