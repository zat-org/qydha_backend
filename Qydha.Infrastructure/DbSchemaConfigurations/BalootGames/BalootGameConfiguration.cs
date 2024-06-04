using Qydha.Domain.Constants;

namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class BalootGameConfiguration : IEntityTypeConfiguration<BalootGame>
{
    public void Configure(EntityTypeBuilder<BalootGame> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("baloot_games");

        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_at");

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.GameMode)
            .HasColumnName("game_mode")
            .HasConversion<string>();

        entity.Property(e => e.ModeratorId).HasColumnName("moderator_id");

        entity.HasOne(d => d.Moderator).WithMany(p => p.ModeratedBalootGames)
            .HasForeignKey(d => d.ModeratorId)
            .HasConstraintName("fk_moderator_baloot_games_link");

        entity.Property(e => e.OwnerId).HasColumnName("owner_id");

        entity.HasOne(d => d.Owner).WithMany(p => p.CreatedBalootGames)
            .HasForeignKey(d => d.OwnerId)
            .HasConstraintName("fk_owner_baloot_games_link");


        entity.Property(e => e.EventsJsonString)
            .HasDefaultValueSql("'[]'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("game_events");

        entity.Property(e => e.State)
            .HasDefaultValueSql("'{}'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("game_state")
            .HasConversion(
                v => JsonConvert.SerializeObject(v, BalootConstants.balootEventsSerializationSettings),
                v => JsonConvert.DeserializeObject<BalootGameState>(v, BalootConstants.balootEventsSerializationSettings) ?? new()
            );
    }
}