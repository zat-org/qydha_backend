using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class BalootSakkaConfiguration : IEntityTypeConfiguration<BalootSakka>
{
    public void Configure(EntityTypeBuilder<BalootSakka> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("baloot_sakkas");

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.IsMashdoda)
            .HasColumnName("is_mashdoda");

        entity.Property(e => e.BalootGameId)
            .HasColumnName("baloot_game_id");

        entity.Property(e => e.UsScore)
            .HasColumnName("us_score");

        entity.Property(e => e.ThemScore)
            .HasColumnName("them_score");

        entity.Property(e => e.StartedAt)
            .IsRequired()
            .HasColumnName("started_at")
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.EndedAt)
            .IsRequired(false)
            .HasColumnName("ended_at")
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.StateName)
            .HasColumnName("state")
            .HasConversion<string>();

        entity.Property(e => e.Winner)
            .IsRequired(false)
            .HasColumnName("winner")
            .HasConversion<string>();

        entity.Property(e => e.DrawHandler)
            .HasColumnName("draw_handler")
            .HasConversion<string>();

        entity.Property(e => e.PausingIntervals)
            .HasDefaultValueSql("'[]'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("pausing_intervals")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<PausingInterval>>(v) ?? new List<PausingInterval>(),
                new ValueComparer<List<PausingInterval>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList())
                );

        entity
            .HasMany(s => s.Moshtaras)
            .WithOne()
            .HasForeignKey(m => m.BalootSakkaId);

    }
}