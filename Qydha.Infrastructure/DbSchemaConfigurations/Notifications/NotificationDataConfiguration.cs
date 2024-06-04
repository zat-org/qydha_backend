using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class NotificationDataConfiguration : IEntityTypeConfiguration<NotificationData>
{
    public void Configure(EntityTypeBuilder<NotificationData> entity)
    {
        var automaticNotifications = new NotificationData[] {
            new() {
                Id = 1,
                Title = "مرحباً بك في قيدها ♥",
                Description = "نتمنى لك تجربة جميلة، ارسلنا لك هدية بقسم المتجر 😉",
                CreatedAt = DateTimeOffset.Parse("2023-11-01T00:00:00.000000Z"),
                Payload = new(),
                ActionPath = "_",
                ActionType = NotificationActionType.NoAction,
                Visibility = NotificationVisibility.Private,
                SendingMechanism=NotificationSendingMechanism.Automatic,
                AnonymousClicks = 0
            },
            new() {
                Id = 2,
                Title = "شكرا لثقتك بقيدها..",
                Description = "نتمنى لك تجربة جميلة، لا تنسى قيدها ليس مجرد حاسبة",
                CreatedAt = DateTimeOffset.Parse("2023-11-01T00:00:00.000000Z"),
                Payload = new(),
                ActionPath = "_",
                ActionType = NotificationActionType.NoAction,
                Visibility = NotificationVisibility.Private,
                SendingMechanism=NotificationSendingMechanism.Automatic,
                AnonymousClicks = 0
            },
            new() {
                Id = 3,
                Title = "وصلتك هدية.. 🎁",
                Description = "شيك على المتجر .. تتهنى ♥",
                CreatedAt = DateTimeOffset.Parse("2023-11-01T00:00:00.000000Z"),
                Payload = new(),
                ActionPath = "_",
                ActionType = NotificationActionType.NoAction,
                Visibility = NotificationVisibility.Private,
                SendingMechanism=NotificationSendingMechanism.Automatic,
                AnonymousClicks = 0
            },
            new() {
                Id = 4,
                Title = "تستاهل ما جاك",
                Description = "نتمنى لك تجربة ممتعة ♥",
                CreatedAt = DateTimeOffset.Parse("2023-11-01T00:00:00.000000Z"),
                Payload = new(),
                ActionPath = "_",
                ActionType = NotificationActionType.NoAction,
                Visibility = NotificationVisibility.Private,
                SendingMechanism=NotificationSendingMechanism.Automatic,
                AnonymousClicks = 0
            },
            new() {
                Id = 5,
                Title = "تم تفعيل الكود",
                Description = "إذا عجبك التطبيق لا تنسى تنشره بين أخوياك",
                CreatedAt = DateTimeOffset.Parse("2023-11-01T00:00:00.000000Z"),
                Payload = new(),
                ActionPath = "_",
                ActionType = NotificationActionType.NoAction,
                Visibility = NotificationVisibility.Private,
                SendingMechanism=NotificationSendingMechanism.Automatic,
                AnonymousClicks = 0
            },
            new() {
                Id = 6,
                Title = "تسجيل دخول الى {ServiceName}",
                Description = "رمز الدخول هو {Otp} تستطيع استخدامه لتسجيل الدخول على {ServiceName} باستخدام حسابك بتطبيق قيدها",
                CreatedAt = DateTimeOffset.Parse("2023-11-01T00:00:00.000000Z"),
                Payload = new(),
                ActionPath = "_",
                ActionType = NotificationActionType.NoAction,
                Visibility = NotificationVisibility.Private,
                SendingMechanism=NotificationSendingMechanism.Automatic,
                AnonymousClicks = 0
            }
        };

        entity.HasData(automaticNotifications);

        entity.HasKey(e => e.Id).HasName("notifications_data_pkey");

        entity.ToTable("notifications_data");

        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.ActionPath)
            .HasMaxLength(350)
            .HasColumnName("action_path");
        entity.Property(e => e.ActionType).HasColumnName("action_type");
        entity.Property(e => e.AnonymousClicks)
            .HasDefaultValue(0)
            .HasColumnName("anonymous_clicks");
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_at");
        entity.Property(e => e.Description)
            .HasMaxLength(512)
            .HasColumnName("description");
        entity.Property(e => e.Payload)
            .HasDefaultValueSql("'{}'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("payload")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<NotificationDataPayload>(v) ?? new()
            );
        entity.Property(e => e.Title)
            .HasMaxLength(255)
            .HasColumnName("title");
        entity.Property(e => e.Visibility)
            .HasColumnName("visibility")
            .HasConversion<string>();
        entity.Property(e => e.SendingMechanism)
            .HasColumnName("sending_mechanism")
            .HasConversion<string>();
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
                    c => c.ToDictionary()));
    }
}