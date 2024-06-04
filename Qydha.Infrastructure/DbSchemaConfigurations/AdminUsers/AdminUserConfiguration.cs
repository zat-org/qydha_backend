
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> entity)
    {

        var superAdmin = new AdminUser()
        {
            Id = Guid.Parse("d2705466-4304-4830-b48a-3e44e031927e"),
            Username = "Admin",
            NormalizedUsername = "ADMIN",
            Role = AdminType.SuperAdmin,
            CreatedAt = DateTimeOffset.Parse("2023-11-01T00:00:00.000000Z"),
            PasswordHash = "$2a$11$V0A5.EYwXlFUjK3RIis3...A9rfzUm.mO.88MUYW9.uHSZLjURNsC"
        };
        entity.HasData(superAdmin);

        entity.HasKey(e => e.Id).HasName("admins_pkey");

        entity.ToTable("admins");

        entity.HasIndex(e => e.NormalizedUsername, "admins_normalized_username_key").IsUnique();

        entity.HasIndex(e => e.Username, "admins_username_key").IsUnique();

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_at");
        entity.Property(e => e.NormalizedUsername)
            .HasMaxLength(100)
            .HasColumnName("normalized_username");
        entity.Property(e => e.PasswordHash)
            .HasMaxLength(255)
            .HasColumnName("password_hash");
        entity.Property(e => e.Role)
            .HasMaxLength(25)
            .HasColumnName("role")
            .HasConversion<string>();

        entity.Property(e => e.Username)
            .HasMaxLength(100)
            .HasColumnName("username");
    }
}