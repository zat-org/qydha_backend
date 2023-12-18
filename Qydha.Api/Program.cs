
using System.Globalization;
using Microsoft.OpenApi.Models;


FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase_private_key.json")
});

var builder = WebApplication.CreateBuilder(args);
//  Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("postgres");

builder.Services.AddControllers((options) =>
{
    options.Filters.Add<ExceptionHandlerAttribute>();
    options.Filters.Add<AuthorizationFilter>();
}).AddNewtonsoftJson();

#region fluent validation
builder.Services.AddValidatorsFromAssemblyContaining<NotificationSendDtoValidator>();

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ValidatorResultFactory>();
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



#region Serilog
Log.Logger =
    new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(new JsonFormatter(), "./Error_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
    .WriteTo.File(new JsonFormatter(), "./info_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();
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

// Authentication 
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new()
        {
            // TODO ::  Validate Life time when have refresh token mechanism
            // ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"] ?? "MustProvideSecretKeyIn__CONFIGURATION__")
            )
        };

    });

// db connection
builder.Services.AddScoped<IDbConnection, NpgsqlConnection>(
    sp => new NpgsqlConnection(connectionString));

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



#endregion

#region DI Services
builder.Services.AddSingleton<TokenManager>();
builder.Services.AddSingleton<OtpManager>();

builder.Services.AddScoped<IMessageService, WhatsAppService>();
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

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var configSec = app.Configuration.GetSection("AdminCredentials");

var variables = new Dictionary<string, string>(){
    {"password",BCrypt.Net.BCrypt.HashPassword(configSec["password"] ?? "admin@123") },
    {"username",configSec["username"] ?? "admin"},
    {"capitalUsername" , (configSec["username"] ?? "admin").ToUpper()}
};

if (connectionString is not null)
    DbMigrator.Migrate(connectionString, variables);

app.Run();
