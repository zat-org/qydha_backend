
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class UserBalootSettingsConfiguration : IEntityTypeConfiguration<UserBalootSettings>
{
    public void Configure(EntityTypeBuilder<UserBalootSettings> entity)
    {
        entity.HasKey(e => e.UserId).HasName("user_baloot_settings_pkey");

        entity.ToTable("user_baloot_settings");

        entity.Property(e => e.UserId)
            .ValueGeneratedNever()
            .HasColumnName("user_id");
        entity.Property(e => e.IsAdvancedRecording)
            .HasDefaultValue(false)
            .HasColumnName("is_advanced_recording");
        entity.Property(e => e.IsCommentsSoundEnabled)
            .HasDefaultValue(true)
            .HasColumnName("is_comments_sound_enabled");
        entity.Property(e => e.IsFlipped)
            .HasDefaultValue(false)
            .HasColumnName("is_flipped");
        entity.Property(e => e.IsNumbersSoundEnabled)
            .HasDefaultValue(true)
            .HasColumnName("is_numbers_sound_enabled");
        entity.Property(e => e.IsSakkahMashdodahMode)
            .HasDefaultValue(false)
            .HasColumnName("is_sakkah_mashdodah_mode");
        entity.Property(e => e.ShowWhoWonDialogOnDraw)
            .HasDefaultValue(false)
            .HasColumnName("show_who_won_dialog_on_draw");
        entity.Property(e => e.IsEkakShown)
            .HasDefaultValue(false)
            .HasColumnName("is_ekak_shown");
        entity.Property(e => e.IsAklatShown)
            .HasDefaultValue(false)
            .HasColumnName("is_aklat_shown");
        entity.Property(e => e.SakkasCount)
            .HasDefaultValue(1)
            .HasColumnName("sakkas_count");

        entity.HasOne(d => d.User).WithOne(p => p.UserBalootSettings)
            .HasForeignKey<UserBalootSettings>(d => d.UserId)
            .HasConstraintName("user_baloot_settings_user_id_fkey");
    }
}