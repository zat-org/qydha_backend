using StackExchange.Profiling;
using System.Globalization;
using Dapper;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using StackExchange.Profiling.Data;
using Serilog.Sinks.GoogleCloudLogging;
using Serilog.Exceptions;


FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase_private_key.json")
});

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("app_keys.json");
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddJsonFile("app_keys.Development.json");

var connectionString = builder.Configuration.GetConnectionString("postgres");

builder.Services.AddControllers((options) =>
{
    options.Filters.Add<ExceptionHandlerAttribute>();
    options.Filters.Add<AuthorizationFilter>();
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
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
            new string[]{}
        }
    });
});

builder.Services.AddMiniProfiler(options =>
      {
          options.RouteBasePath = "/profiler"; // Configure the route path as needed

          //   options.ResultsAuthorize = _ => true;
          //   options.ResultsListAuthorize = _ => true;
      });

#region Serilog
var loggerConfig = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()

    .Enrich.WithExceptionDetails()

    .WriteTo.File(new JsonFormatter(renderMessage: true), "./Error_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
    .WriteTo.File(new JsonFormatter(renderMessage: true), "./info_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information);

if (builder.Environment.IsProduction())
{
    string serviceAccountCredential = File.ReadAllText("googleCloud_private_key.json");
    var googleCloudConfig = new GoogleCloudLoggingSinkOptions
    {
        ProjectId = "qydha-98740",
        GoogleCredentialJson = serviceAccountCredential,
        ServiceName = "Qydha Production"
    };
    loggerConfig.WriteTo.GoogleCloudLogging(googleCloudConfig);
}

Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();
#endregion

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
builder.Services.Configure<NotificationsSettings>(builder.Configuration.GetSection("NotificationsSettings"));
// Notification Image Settings
builder.Services.Configure<NotificationImageSettings>(builder.Configuration.GetSection("NotificationImageSettings"));
// Book Settings
builder.Services.Configure<BookSettings>(builder.Configuration.GetSection("BookSettings"));
// UltraMsg Settings
builder.Services.Configure<UltraMsgSettings>(builder.Configuration.GetSection("UltraMsgSettings"));
builder.Services.Configure<RegisterGiftSetting>(builder.Configuration.GetSection("RegisterGiftSetting"));




// db connection
builder.Services.AddScoped<IDbConnection, ProfiledDbConnection>(
    sp =>
    {
        var connection = new NpgsqlConnection(connectionString);
        var profiledConnection = new ProfiledDbConnection(connection, MiniProfiler.Current);
        profiledConnection.Open(); // Open the connection when it's created
        return profiledConnection;
    });
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



#endregion

#region SQL Mappers for json
SqlMapper.AddTypeHandler(new JsonTypeHandler<IEnumerable<string>>());


#endregion

#region DI Services
builder.Services.AddSingleton<TokenManager>();
builder.Services.AddSingleton<OtpManager>();

// if (builder.Environment.IsDevelopment())
//     builder.Services.AddScoped<IMessageService, UltraMsgService>();
// else
//     builder.Services.AddScoped<IMessageService, WhatsAppService>();

builder.Services.AddScoped<IMessageService, UltraMsgService>();


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

if (connectionString is not null)
{
    var configSec = builder.Configuration.GetSection("AdminCredentials");

    var variables = new Dictionary<string, string>(){
            {"password",BCrypt.Net.BCrypt.HashPassword(configSec["password"] ?? "admin@123") },
            {"username",configSec["username"] ?? "admin"},
            {"capitalUsername" , (configSec["username"] ?? "admin").ToUpper()}
        };
    DbMigrator.Migrate(connectionString, variables);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Configuration.GetValue<bool>("UseMiniProfiler"))
{
    app.UseMiniProfiler();
}


app.UseCors(MyAllowSpecificOrigins);

app.UseStaticFiles();

app.UseSerilogRequestLogging(op =>
{
    op.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms {NewLine} => UserId : {UserId} {NewLine} => Client IP : {ClientIp} {NewLine} => User-Agent : {UserAgent} ";
    op.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        diagnosticContext.Set("ClientIp", httpContext.Request.Headers["X-Real-IP"].ToString());
        diagnosticContext.Set("UserId", httpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
        diagnosticContext.Set("NewLine", "\n");

    };
});

app.MapControllers();

app.Run();



