namespace Qydha.API.Models;

public class CreateBalootGameDto
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<BalootGameEventDto> Events { get; set; } = null!;
}