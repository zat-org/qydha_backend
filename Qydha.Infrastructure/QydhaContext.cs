namespace Qydha.Infrastructure;

public partial class QydhaContext(DbContextOptions<QydhaContext> options) : DbContext(options)
{
    #region  dbSets
    public virtual DbSet<AdminUser> Admins { get; set; }
    public virtual DbSet<AppAsset> AppAssets { get; set; }
    public virtual DbSet<InfluencerCode> InfluencerCodes { get; set; }
    public virtual DbSet<InfluencerCodeCategory> InfluencerCodeCategories { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserBalootSettings> UserBalootSettings { get; set; }
    public virtual DbSet<UserGeneralSettings> UserGeneralSettings { get; set; }
    public virtual DbSet<UserHandSettings> UserHandSettings { get; set; }
    public virtual DbSet<UpdateEmailRequest> UpdateEmailRequests { get; set; }
    public virtual DbSet<UpdatePhoneRequest> UpdatePhoneRequests { get; set; }
    public virtual DbSet<PhoneAuthenticationRequest> PhoneAuthenticationRequests { get; set; }
    public virtual DbSet<RegistrationOTPRequest> RegistrationOtpRequests { get; set; }
    public virtual DbSet<UserPromoCode> UserPromoCodes { get; set; }
    public virtual DbSet<Purchase> Purchases { get; set; }
    public virtual DbSet<LoginWithQydhaRequest> LoginWithQydhaRequests { get; set; }
    public virtual DbSet<NotificationData> NotificationsData { get; set; }
    public virtual DbSet<NotificationUserLink> NotificationUserLinks { get; set; }
    public virtual DbSet<InfluencerCodeUserLink> InfluencerCodeUserLinks { get; set; }
    public virtual DbSet<BalootGame> BalootGames { get; set; }

    #endregion

    public async Task<PagedList<T>> GetPagedData<T>(
         IQueryable<T> query, int pageNumber, int pageSize) where T : class
    {
        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QydhaContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
