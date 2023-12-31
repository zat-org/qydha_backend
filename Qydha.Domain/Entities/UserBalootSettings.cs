namespace Qydha.Domain.Entities;

[Table("user_baloot_settings")]
public class UserBalootSettings : DbEntity<UserBalootSettings>
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("is_flipped")]
    public bool IsFlipped { get; set; } = false;

    [Column("is_advanced_recording")]
    public bool IsAdvancedRecording { get; set; } = false;

}
