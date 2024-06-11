using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Qydha.Domain.Constants;
using Qydha.Domain.ValueObjects;

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

        entity.Property(e => e.GameData)
            .HasDefaultValueSql("'{}'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("game_state")
            .HasConversion(
                v => JsonConvert.SerializeObject(v, BalootConstants.balootEventsSerializationSettings),
                v => JsonConvert.DeserializeObject<BalootGameData>(v, BalootConstants.balootEventsSerializationSettings) ?? new()
            );

        // static void moshtaraBuilder(OwnedNavigationBuilder<BalootSakka, BalootMoshtara> builder)
        // {
        //     builder.OwnsMany(moshtara => moshtara.PausingIntervals);
        //     // builder.OwnsOne(moshtara => moshtara.Data);
        //     builder.Property(moshtara => moshtara.Data).HasConversion(
        //         v => JsonConvert.SerializeObject(v!.ToDto(), BalootConstants.balootEventsSerializationSettings),
        //         v => MoshtaraData.FromDto(
        //                 JsonConvert.DeserializeObject<MoshtaraDataDto>(v, BalootConstants.balootEventsSerializationSettings)
        //                     ?? new(BalootRecordingMode.Regular, 0, 0, null))
        //     );
        // }

        // static void SakkaBuilder(OwnedNavigationBuilder<BalootGameData, BalootSakka> builder)
        // {
        //     builder.OwnsOne(sakka => sakka.PausingIntervals);
        //     builder.OwnsMany(sakka => sakka.Moshtaras, moshtaraBuilder);
        //     builder.OwnsOne(sakka => sakka.CurrentMoshtara, moshtaraBuilder);
        // }

        // entity.OwnsOne(g => g.GameData, builder =>
        //     {
        //         // builder.Property(g => g).HasColumnName("game_State");
        //         builder.ToJson();
        //         builder.OwnsMany(data => data.PausingIntervals);
        //         builder.OwnsMany(data => data.Sakkas, SakkaBuilder);
        //         builder.OwnsOne(data => data.CurrentSakka, SakkaBuilder);
        //     });


    }
}