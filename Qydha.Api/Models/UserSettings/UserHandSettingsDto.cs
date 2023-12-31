
namespace Qydha.API.Models;

public class UserHandSettingsDto
{
    public int RoundsCount { get; set; }

    public int MaxLimit { get; set; }

    public int TeamsCount { get; set; }

    public int PlayersCountInTeam { get; set; }

}
public class UserHandSettingsDtoValidator : AbstractValidator<UserHandSettingsDto>
{
    public UserHandSettingsDtoValidator()
    {
        RuleFor(r => r.MaxLimit).Must(val => val >= 0).WithMessage("الحد يجب ان يكون اكبر من الصفر");
        RuleFor(r => r.RoundsCount).Must(val => val >= 0 && val <= 9).WithMessage("عدد اللعبات يجب ان يكون بين 1 ~ 9 ");
        RuleFor(r => r.TeamsCount).Must(val => val >= 2 && val <= 6).WithMessage("عدد الفرق يجب ان يكون بين 2 ~ 6");
        RuleFor(r => r.PlayersCountInTeam).Must(val => val >= 1 && val <= 4).WithMessage("  عدد لاعبى كل فريق يجب ان يكون بين 1 ~ 4");
    }
}