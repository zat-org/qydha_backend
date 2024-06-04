using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class UserGeneralSettingsConfiguration : IEntityTypeConfiguration<UserGeneralSettings>
{
    public void Configure(EntityTypeBuilder<UserGeneralSettings> entity)
    {
        entity.HasKey(e => e.UserId).HasName("user_general_settings_pkey");

        entity.ToTable("user_general_settings");

        entity.Property(e => e.UserId)
            .ValueGeneratedNever()
            .HasColumnName("user_id");
        entity.Property(e => e.EnableVibration)
            .HasDefaultValue(true)
            .HasColumnName("enable_vibration");
        entity.Property(e => e.PlayersNames)
            .HasDefaultValueSql("'[]'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("players_names")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList())
                );
        entity.Property(e => e.TeamsNames)
            .HasDefaultValueSql("'[]'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("teams_names")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList())
            );

        entity.HasOne(d => d.User).WithOne(p => p.UserGeneralSettings)
            .HasForeignKey<UserGeneralSettings>(d => d.UserId)
            .HasConstraintName("user_general_settings_user_id_fkey");
    }
}