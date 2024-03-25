using System.Globalization;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Serilog.Sinks.GoogleCloudLogging;
using Serilog.Exceptions;
using Microsoft.EntityFrameworkCore;


FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase_private_key.json")
});

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("app_keys.json");
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddJsonFile("app_keys.Development.json");
else if (builder.Environment.IsStaging())
    builder.Configuration.AddJsonFile("app_keys.Staging.json");


var connectionString = builder.Configuration.GetConnectionString("postgres");

builder.Services.AddControllers((options) =>
{
    options.Filters.Add<ExceptionHandlerAttribute>();
    options.Filters.Add<AuthorizationFilter>();
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

#region fluent validation
builder.Services.AddValidatorsFromAssemblyContaining<NotificationSendDtoValidator>();

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ValidatorResultFactory>();
    configuration.EnableFormBindingSourceAutomaticValidation = true;
    configuration.EnableBodyBindingSourceAutomaticValidation = true;
    configuration.EnablePathBindingSourceAutomaticValidation = true;
    configuration.EnablePathBindingSourceAutomaticValidation = true;
    configuration.EnableQueryBindingSourceAutomaticValidation = true;
});

ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("ar");

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

#endregion 

#region Filters 

//defined filters 
builder.Services.AddScoped<ExceptionHandlerAttribute>();
builder.Services.AddScoped<AuthorizationFilter>();
#endregion

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(User).Assembly);
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Qydha", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


#region Serilog
var loggerConfig = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File(new JsonFormatter(renderMessage: true), "./Error_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning);

if (builder.Environment.IsProduction())
{
    string serviceAccountCredential = File.ReadAllText("googleCloud_private_key.json");
    var googleLoggerConfig = builder.Configuration.GetSection("GoogleLogger");
    var googleCloudConfig = new GoogleCloudLoggingSinkOptions
    {
        ProjectId = googleLoggerConfig["ProjectId"],
        GoogleCredentialJson = serviceAccountCredential,
        ServiceName = googleLoggerConfig["ServiceName"]
    };
    loggerConfig.WriteTo.GoogleCloudLogging(googleCloudConfig);
}
else
{
    loggerConfig.WriteTo.File(new JsonFormatter(renderMessage: true), "./Info_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information);
}

Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();
#endregion

// db connection
builder.Services.AddDbContext<QydhaContext>(
    (opt) =>
    {
        opt.UseNpgsql(connectionString, b => b.MigrationsAssembly("Qydha.Api"))
        .EnableSensitiveDataLogging()
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
);

#region DI settings
// otp options  
builder.Services.Configure<OTPSettings>(builder.Configuration.GetSection("OTP"));
// twilio options 
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
// JWT options 
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("Authentication"));
// mail server settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("MailSettings"));
// whatsapp  settings
builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));
// Photo Settings
builder.Services.Configure<AvatarSettings>(builder.Configuration.GetSection("AvatarSettings"));
// IAPHub Settings
builder.Services.Configure<IAPHubSettings>(builder.Configuration.GetSection("IAPHubSettings"));
// Products Settings
builder.Services.Configure<ProductsSettings>(builder.Configuration.GetSection("ProductsSettings"));
// Subscription Settings
builder.Services.Configure<SubscriptionSetting>(builder.Configuration.GetSection("SubscriptionSetting"));
// Notifications Settings
builder.Services.Configure<PushNotificationsSettings>(builder.Configuration.GetSection("PushNotificationsSettings"));
// Notification Image Settings
builder.Services.Configure<NotificationImageSettings>(builder.Configuration.GetSection("NotificationImageSettings"));
// Book Settings
builder.Services.Configure<BookSettings>(builder.Configuration.GetSection("BookSettings"));
// UltraMsg Settings
builder.Services.Configure<UltraMsgSettings>(builder.Configuration.GetSection("UltraMsgSettings"));

builder.Services.Configure<RegisterGiftSetting>(builder.Configuration.GetSection("RegisterGiftSetting"));

#endregion

#region DI Repos
// repos 
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IRegistrationOTPRequestRepo, RegistrationOTPRequestRepo>();
builder.Services.AddScoped<IUpdatePhoneOTPRequestRepo, UpdatePhoneOTPRequestRepo>();
builder.Services.AddScoped<IPhoneAuthenticationRequestRepo, PhoneAuthenticationRequestRepo>();
builder.Services.AddScoped<IUpdateEmailRequestRepo, UpdateEmailRequestRepo>();
builder.Services.AddScoped<IPurchaseRepo, PurchaseRepo>();
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
builder.Services.AddScoped<IUserPromoCodesRepo, UserPromoCodesRepo>();
builder.Services.AddScoped<IAdminUserRepo, AdminUserRepo>();
builder.Services.AddScoped<IInfluencerCodesRepo, InfluencerCodesRepo>();
builder.Services.AddScoped<IUserGeneralSettingsRepo, UserGeneralSettingsRepo>();
builder.Services.AddScoped<IUserHandSettingsRepo, UserHandSettingsRepo>();
builder.Services.AddScoped<IUserBalootSettingsRepo, UserBalootSettingsRepo>();
builder.Services.AddScoped<IAppAssetsRepo, AppAssetsRepo>();
builder.Services.AddScoped<IInfluencerCodesCategoriesRepo, InfluencerCodesCategoriesRepo>();
builder.Services.AddScoped<ILoginWithQydhaRequestRepo, LoginWithQydhaRequestRepo>();
#endregion

#region DI Services
builder.Services.AddSingleton<TokenManager>();
builder.Services.AddSingleton<OtpManager>();

if (builder.Configuration.GetValue<bool>("UseUltraMessage"))
    builder.Services.AddScoped<IMessageService, UltraMsgService>();
else
    builder.Services.AddScoped<IMessageService, WhatsAppService>();

// builder.Services.AddScoped<IMessageService, UltraMsgService>();


builder.Services.AddScoped<IMailingService, MailingService>();
builder.Services.AddScoped<IFileService, GoogleCloudFileService>();
builder.Services.AddScoped<IPushNotificationService, FCMService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IUserPromoCodesService, UserPromoCodesService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IInfluencerCodesService, InfluencerCodesService>();
builder.Services.AddScoped<IAppAssetsService, AppAssetsService>();
builder.Services.AddScoped<IInfluencerCodeCategoryService, InfluencerCodeCategoryService>();
builder.Services.AddScoped<ILoginWithQydhaOtpSenderService, LoginWithQydhaOtpSenderAsNotification>();
builder.Services.AddSingleton(new GoogleStorageService("googleCloud_private_key.json"));
#endregion

#region Add Cors
string MyAllowSpecificOrigins = "_MyAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseStaticFiles();

app.UseSerilogRequestLogging(op =>
{
    op.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms {NewLine} => UserId : {UserId} {NewLine} => Client IP : {ClientIp} {NewLine} => X-INFO : {xINFO} ";
    op.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("xINFO", httpContext.Request.Headers["x-info"].ToString());
        diagnosticContext.Set("ClientIp", httpContext.Request.Headers["X-Real-IP"].ToString());
        diagnosticContext.Set("UserId", httpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
        diagnosticContext.Set("NewLine", "\n");
    };
});

app.MapControllers();

app.Run();



