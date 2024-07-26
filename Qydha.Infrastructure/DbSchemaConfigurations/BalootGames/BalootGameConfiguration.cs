using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class BalootGameConfiguration : IEntityTypeConfiguration<BalootGame>
{
    public void Configure(EntityTypeBuilder<BalootGame> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("baloot_games");

        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("created_at");

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.GameMode)
            .HasColumnName("game_mode")
            .HasConversion<string>();

        entity.Property(e => e.ModeratorId).HasColumnName("moderator_id");

        entity.HasOne(d => d.Moderator).WithMany(p => p.ModeratedBalootGames)
            .HasForeignKey(d => d.ModeratorId)
            .HasConstraintName("fk_moderator_baloot_games_link")
            .OnDelete(DeleteBehavior.SetNull);

        entity.Property(e => e.OwnerId).HasColumnName("owner_id");

        entity.HasOne(d => d.Owner).WithMany(p => p.CreatedBalootGames)
            .HasForeignKey(d => d.OwnerId)
            .HasConstraintName("fk_owner_baloot_games_link")
            .OnDelete(DeleteBehavior.SetNull);


        entity.Property(e => e.EventsJsonString)
            .HasDefaultValueSql("'[]'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("game_events");


        entity.Property(e => e.UsName)
            .HasColumnName("us_name")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.ThemName)
            .HasColumnName("them_name")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.UsGameScore)
            .HasColumnName("us_game_score");

        entity.Property(e => e.ThemGameScore)
            .HasColumnName("them_game_score");

        entity.Property(e => e.MaxSakkaPerGame)
                  .HasColumnName("max_sakka_per_game")
                  .HasColumnType("smallint");


        entity.Property(e => e.StartedAt)
            .IsRequired(false)
            .HasColumnName("started_at")
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter);

        entity.Property(e => e.EndedAt)
            .IsRequired(false)
            .HasColumnName("ended_at")
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter);

        entity.Property(e => e.StateName)
            .HasColumnName("state")
            .HasConversion<string>();

        entity.Property(e => e.Winner)
            .IsRequired(false)
            .HasColumnName("winner")
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
            .HasMany(s => s.Sakkas)
            .WithOne()
            .HasForeignKey(s => s.BalootGameId);

        entity.Property(e => e.Location)
            .HasColumnType("geometry(Point, 4326)");


    }
}