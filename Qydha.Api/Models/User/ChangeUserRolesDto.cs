namespace Qydha.API.Models;
public class ChangeUserRolesDto
{
    public List<UserRoles> Roles { get; set; } = null!;
}
public class ChangeUserRolesDtoValidator : AbstractValidator<ChangeUserRolesDto>
{
    public ChangeUserRolesDtoValidator()
    {
        RuleForEach(r => r.Roles).IsInEnum();
    }
}