using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class NotificationUserLinkConfiguration : IEntityTypeConfiguration<NotificationUserLink>
{
    public void Configure(EntityTypeBuilder<NotificationUserLink> entity)
    {
        entity.HasKey(e => e.Id).HasName("notifications_users_link_pkey");

        entity.ToTable("notifications_users_link");

        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.NotificationId).HasColumnName("notification_id");
        entity.Property(e => e.ReadAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("read_at");
        entity.Property(e => e.SentAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("sent_at");

        entity.Property(e => e.TemplateValues)
            .HasDefaultValueSql("'{}'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("template_values")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v) ?? new(),
                new ValueComparer<Dictionary<string, string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary())
        );
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.Notification).WithMany(p => p.NotificationUserLinks)
            .HasForeignKey(d => d.NotificationId)
            .HasConstraintName("fk_notification_at_notification_link_table");

        entity.HasOne(d => d.User).WithMany(p => p.NotificationUserLinks)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_user_at_notification_link_table");
    }
}