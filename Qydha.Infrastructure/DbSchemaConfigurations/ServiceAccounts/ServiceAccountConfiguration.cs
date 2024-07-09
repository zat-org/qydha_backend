namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class ServiceAccountConfiguration : IEntityTypeConfiguration<ServiceAccount>
{
    public void Configure(EntityTypeBuilder<ServiceAccount> builder)
    {

        builder.HasKey(e => e.Id).HasName("service_accounts_pkey");

        builder.ToTable("service_accounts");

        builder.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .HasMaxLength(50)
            .HasColumnName("name");

        builder.Property(e => e.Description)
            .HasMaxLength(256)
            .HasColumnName("description");

        builder.Property(e => e.Permissions)
            .HasColumnName("permissions");
    }
}