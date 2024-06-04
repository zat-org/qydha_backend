
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class UserHandSettingsConfiguration : IEntityTypeConfiguration<UserHandSettings>
{
    public void Configure(EntityTypeBuilder<UserHandSettings> entity)
    {
        entity.HasKey(e => e.UserId).HasName("user_hand_settings_pkey");

        entity.ToTable("user_hand_settings");

        entity.Property(e => e.UserId)
            .ValueGeneratedNever()
            .HasColumnName("user_id");
        entity.Property(e => e.MaxLimit)
            .HasDefaultValue(0)
            .HasColumnName("max_limit");
        entity.Property(e => e.PlayersCountInTeam)
            .HasDefaultValue(2)
            .HasColumnName("players_count_in_team");
        entity.Property(e => e.RoundsCount)
            .HasDefaultValue(7)
            .HasColumnName("rounds_count");
        entity.Property(e => e.TakweeshPoints)
            .HasDefaultValue(100)
            .HasColumnName("takweesh_points");
        entity.Property(e => e.TeamsCount)
            .HasDefaultValue(2)
            .HasColumnName("teams_count");
        entity.Property(e => e.WinUsingZat)
            .HasDefaultValue(false)
            .HasColumnName("win_using_zat");

        entity.HasOne(d => d.User).WithOne(p => p.UserHandSettings)
            .HasForeignKey<UserHandSettings>(d => d.UserId)
            .HasConstraintName("user_hand_settings_user_id_fkey");
    }
}