namespace Qydha.API.Models;

public class UpdateBalootBookDto
{
    public IFormFile File { get; set; } = null!;
}

public class UpdateBalootBookDtoValidator : AbstractValidator<UpdateBalootBookDto>
{
    private readonly BookSettings _bookSettings;
    public UpdateBalootBookDtoValidator(IOptions<BookSettings> bookOptions)
    {
        _bookSettings = bookOptions.Value;
        RuleFor(dto => dto.File)
        .Cascade(CascadeMode.Stop)
        .File("كتاب البلوت", _bookSettings);
    }
}