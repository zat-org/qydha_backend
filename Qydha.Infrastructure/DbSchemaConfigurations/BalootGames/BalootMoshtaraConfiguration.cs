using Microsoft.EntityFrameworkCore.ChangeTracking;
using Qydha.Domain.Constants;
using Qydha.Domain.ValueObjects;

namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class BalootMoshtaraConfiguration : IEntityTypeConfiguration<BalootMoshtara>
{
    public void Configure(EntityTypeBuilder<BalootMoshtara> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("baloot_moshtaras");

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.BalootSakkaId)
            .HasColumnName("baloot_sakka_id");

        entity.Property(e => e.StartedAt)
            .IsRequired(false)
            .HasColumnName("started_at")
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.EndedAt)
            .IsRequired(false)
            .HasColumnName("ended_at")
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.UsScore)
            .HasColumnName("us_score");

        entity.Property(e => e.ThemScore)
            .HasColumnName("them_score");

        entity.Property(e => e.Data)
            .IsRequired(false)
            .HasColumnType("jsonb")
            .HasColumnName("data")
            .HasConversion(
                v => JsonConvert.SerializeObject(v, BalootConstants.balootEventsSerializationSettings),
                v => JsonConvert.DeserializeObject<MoshtaraData>(v, BalootConstants.balootEventsSerializationSettings)
            );


        entity.Property(e => e.StateName)
            .HasColumnName("state")
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
    }
}