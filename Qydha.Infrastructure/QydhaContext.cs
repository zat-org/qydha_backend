using Microsoft.EntityFrameworkCore.ChangeTracking;
using Qydha.Domain.Constants;
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
        #region data creation for seeding
        var superAdmin = new AdminUser()
        {
            Id = Guid.Parse("d2705466-4304-4830-b48a-3e44e031927e"),
            Username = "Admin",
            NormalizedUsername = "ADMIN",
            Role = AdminType.SuperAdmin,
            CreatedAt = DateTimeOffset.Parse("2023-11-01T00:00:00.000000Z"),
            PasswordHash = "$2a$11$V0A5.EYwXlFUjK3RIis3...A9rfzUm.mO.88MUYW9.uHSZLjURNsC"
        };

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

        var AppAssets = new AppAsset[] {
            new(){
                AssetKey = "baloot_book",
                AssetData = "{}"
                },
            new(){
                AssetKey = "popup",
                AssetData = "{}"
            }
        };

        #endregion

        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AdminUser>().HasData(superAdmin);

        modelBuilder.Entity<NotificationData>().HasData(automaticNotifications);

        modelBuilder.Entity<AppAsset>().HasData(AppAssets);

        #region Entities_Configuration

        modelBuilder.Entity<AdminUser>(entity =>
        {
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
        });

        modelBuilder.Entity<AppAsset>(entity =>
        {
            entity.HasKey(e => e.AssetKey).HasName("app_assets_pkey");

            entity.ToTable("app_assets");

            entity.Property(e => e.AssetKey)
                .HasMaxLength(100)
                .HasColumnName("asset_key");
            entity.Property(e => e.AssetData)
                .HasColumnType("jsonb")
                .HasColumnName("asset_data");
        });

        modelBuilder.Entity<InfluencerCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("influencer_codes_pkey");

            entity.ToTable("influencer_codes");

            entity.HasIndex(e => e.Code, "influencer_codes_code_key").IsUnique();

            entity.HasIndex(e => e.NormalizedCode, "influencer_codes_normalized_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpireAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("expire_at");
            entity.Property(e => e.MaxInfluencedUsersCount)
                .HasDefaultValue(0)
                .HasColumnName("max_influenced_users_count");
            entity.Property(e => e.NormalizedCode)
                .HasMaxLength(100)
                .HasColumnName("normalized_code");
            entity.Property(e => e.NumberOfDays).HasColumnName("number_of_days");

            entity.HasOne(d => d.Category).WithMany(p => p.InfluencerCodes)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("fk_influencer_code_categories");
        });

        modelBuilder.Entity<InfluencerCodeCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("influencer_codes_categories_pkey");

            entity.ToTable("influencer_codes_categories");

            entity.HasIndex(e => e.CategoryName, "influencer_codes_categories_category_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
            entity.Property(e => e.MaxCodesPerUserInGroup)
                .HasDefaultValue(1)
                .HasColumnName("max_codes_per_user_in_group");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.NormalizedEmail, "users_normalized_email_key").IsUnique();

            entity.HasIndex(e => e.NormalizedUsername, "users_normalized_username_key").IsUnique();

            entity.HasIndex(e => e.Phone, "users_phone_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AvatarPath)
                .HasColumnType("character varying")
                .HasColumnName("avatar_path");
            entity.Property(e => e.AvatarUrl)
                .HasColumnType("character varying")
                .HasColumnName("avatar_url");
            entity.Property(e => e.BirthDate)
                .HasColumnType("DATE")
                .HasColumnName("birth_date");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.ExpireDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("expire_date");
            entity.Property(e => e.FCMToken)
                .HasMaxLength(200)
                .HasColumnName("fcm_token");

            entity.Property(e => e.IsAnonymous)
                .HasDefaultValue(false)
                .HasColumnName("is_anonymous");
            entity.Property(e => e.IsEmailConfirmed)
                .HasDefaultValue(false)
                .HasColumnName("is_email_confirmed");
            entity.Property(e => e.IsPhoneConfirmed)
                .HasDefaultValue(false)
                .HasColumnName("is_phone_confirmed");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.NormalizedEmail)
                .HasMaxLength(200)
                .HasColumnName("normalized_email");
            entity.Property(e => e.NormalizedUsername)
                .HasMaxLength(100)
                .HasColumnName("normalized_username");
            entity.Property(e => e.PasswordHash)
                .HasColumnType("character varying")
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserBalootSettings>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_baloot_settings_pkey");

            entity.ToTable("user_baloot_settings");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.IsAdvancedRecording)
                .HasDefaultValue(false)
                .HasColumnName("is_advanced_recording");
            entity.Property(e => e.IsCommentsSoundEnabled)
                .HasDefaultValue(true)
                .HasColumnName("is_comments_sound_enabled");
            entity.Property(e => e.IsFlipped)
                .HasDefaultValue(false)
                .HasColumnName("is_flipped");
            entity.Property(e => e.IsNumbersSoundEnabled)
                .HasDefaultValue(true)
                .HasColumnName("is_numbers_sound_enabled");
            entity.Property(e => e.IsSakkahMashdodahMode)
                .HasDefaultValue(false)
                .HasColumnName("is_sakkah_mashdodah_mode");
            entity.Property(e => e.ShowWhoWonDialogOnDraw)
                .HasDefaultValue(false)
                .HasColumnName("show_who_won_dialog_on_draw");
            entity.Property(e => e.IsEkakAklatShown)
                .HasDefaultValue(false)
                .HasColumnName("is_ekak_aklat_shown");

            entity.HasOne(d => d.User).WithOne(p => p.UserBalootSettings)
                .HasForeignKey<UserBalootSettings>(d => d.UserId)
                .HasConstraintName("user_baloot_settings_user_id_fkey");
        });

        modelBuilder.Entity<UserGeneralSettings>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_general_settings_pkey");

            entity.ToTable("user_general_settings");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.EnableVibration)
                .HasDefaultValue(true)
                .HasColumnName("enable_vibration");
            entity.Property(e => e.PlayersNames)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("players_names")
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>(),
                    new ValueComparer<List<string>>(
                        (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList())
                    );
            entity.Property(e => e.TeamsNames)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("teams_names")
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>(),
                    new ValueComparer<List<string>>(
                        (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList())
                );

            entity.HasOne(d => d.User).WithOne(p => p.UserGeneralSettings)
                .HasForeignKey<UserGeneralSettings>(d => d.UserId)
                .HasConstraintName("user_general_settings_user_id_fkey");
        });

        modelBuilder.Entity<UserHandSettings>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_hand_settings_pkey");

            entity.ToTable("user_hand_settings");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.MaxLimit)
                .HasDefaultValue(0)
                .HasColumnName("max_limit");
            entity.Property(e => e.PlayersCountInTeam)
                .HasDefaultValue(2)
                .HasColumnName("players_count_in_team");
            entity.Property(e => e.RoundsCount)
                .HasDefaultValue(7)
                .HasColumnName("rounds_count");
            entity.Property(e => e.TakweeshPoints)
                .HasDefaultValue(100)
                .HasColumnName("takweesh_points");
            entity.Property(e => e.TeamsCount)
                .HasDefaultValue(2)
                .HasColumnName("teams_count");
            entity.Property(e => e.WinUsingZat)
                .HasDefaultValue(false)
                .HasColumnName("win_using_zat");

            entity.HasOne(d => d.User).WithOne(p => p.UserHandSettings)
                .HasForeignKey<UserHandSettings>(d => d.UserId)
                .HasConstraintName("user_hand_settings_user_id_fkey");
        });

        modelBuilder.Entity<UpdateEmailRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("update_email_requests_pkey");

            entity.ToTable("update_email_requests");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.OTP)
                .HasMaxLength(6)
                .HasColumnName("otp");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<UpdatePhoneRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("update_phone_requests_pkey");

            entity.ToTable("update_phone_requests");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.OTP)
                .HasMaxLength(6)
                .HasColumnName("otp");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<PhoneAuthenticationRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("phone_authentication_requests_pkey");

            entity.ToTable("phone_authentication_requests");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.Otp)
                .HasMaxLength(6)
                .HasColumnName("otp");
        });

        modelBuilder.Entity<RegistrationOTPRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("registration_otp_request_pkey");

            entity.ToTable("registration_otp_request");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.FCMToken)
                .HasMaxLength(200)
                .HasColumnName("fcm_token");
            entity.Property(e => e.OTP)
                .HasMaxLength(6)
                .HasColumnName("otp");
            entity.Property(e => e.PasswordHash)
                .HasColumnType("character varying")
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

        });

        modelBuilder.Entity<UserPromoCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_promo_codes_pkey");

            entity.ToTable("user_promo_codes");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpireAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("expire_at");
            entity.Property(e => e.NumberOfDays).HasColumnName("number_of_days");
            entity.Property(e => e.UsedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserPromoCodes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_codes");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("purchases_pkey");

            entity.ToTable("purchases");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.IAPHubPurchaseId)
                .HasMaxLength(40)
                .HasColumnName("iaphub_purchase_id");
            entity.Property(e => e.NumberOfDays).HasColumnName("number_of_days");
            entity.Property(e => e.ProductSku)
                .HasMaxLength(15)
                .HasColumnName("productsku");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("purchase_date");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LoginWithQydhaRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("login_with_qydha_requests_pkey");

            entity.ToTable("login_with_qydha_requests");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Otp)
                .HasMaxLength(6)
                .HasColumnName("otp");
            entity.Property(e => e.UsedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.LoginWithQydhaRequests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_id");
        });

        modelBuilder.Entity<NotificationData>(entity =>
        {
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
        });

        modelBuilder.Entity<NotificationUserLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_users_link_pkey");

            entity.ToTable("notifications_users_link");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.ReadAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("read_at");
            entity.Property(e => e.SentAt)
                .HasColumnType("timestamp with time zone")
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
        });

        modelBuilder.Entity<InfluencerCodeUserLink>(entity =>
        {
            entity.HasKey(e => new { e.InfluencerCodeId, e.UserId }).HasName("influencer_code_users_link_pkey");

            entity.ToTable("influencer_code_users_link");

            entity.Property(e => e.UsedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("used_at");
            entity.Property(e => e.NumberOfDays)
                .HasColumnName("number_of_days");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.InfluencerCodeId).HasColumnName("influencer_code_id");

            entity.HasOne(d => d.User).WithMany(p => p.InfluencerCodes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_at_influencer_code_user_link_table");

            entity.HasOne(d => d.InfluencerCode).WithMany(p => p.Users)
                .HasForeignKey(d => d.InfluencerCodeId)
                .HasConstraintName("fk_influencer_code_at_influencer_code_link_table");
        });


        modelBuilder.Entity<BalootGame>(entity =>
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
        });
        #endregion
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
