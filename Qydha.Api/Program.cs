
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase_private_key.json")
});

var builder = WebApplication.CreateBuilder(args);

ReadingConfigurationFile.ReadConfigurationFile(builder.Configuration, builder.Environment);
string MyAllowSpecificOrigins = builder.Services.ConfigureCORS();
LoggerServiceExtension.AddLoggerConfiguration(builder.Configuration, builder.Environment);
builder.Host.UseSerilog();
// builder.WebHost.UseWebRoot("wwwroot");
builder.Services.DbConfiguration(builder.Configuration, builder.Environment);
builder.Services.RegisterServices(builder.Configuration, builder.Environment);
builder.Services.RegisterRepos();
builder.Services.ConfigureControllers();
builder.Services.ConfigureFluentValidation();
builder.Services.RegisterAttributes();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureMediatR();
builder.Services.RegisterSettings(builder.Configuration);
builder.Services.AuthConfiguration(builder.Configuration);
builder.Services.AddSignalR();

var app = builder.Build();


if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestContextLogging();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

// app.UseStaticFiles();
app.UseSerilogRequestLogging(op =>
{
    op.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms {NewLine} => UserId : {UserId} {NewLine} => Token Type : {TokenType} {NewLine} => Client IP : {ClientIp} {NewLine} => X-Info {XInfo} {NewLine} => Device_Info : Environment : {Environment} Platform : {Platform} DeviceName : {DeviceName} AppVersion : {AppVersion} DeviceId : {DeviceId} {NewLine} => TraceId : {RequestId} ";
    op.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("ClientIp", httpContext.Request.Headers["X-Real-IP"].ToString());
        diagnosticContext.Set("UserId", httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "user Id not provided");
        diagnosticContext.Set("TokenType", httpContext.User.FindFirst(ClaimsNamesConstants.TokenType)?.Value ?? "Token type not provided");
        diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
        diagnosticContext.Set("NewLine", " \n");
        diagnosticContext.Set("XInfo", httpContext.Request.Headers["X-INFO"]);
        var XInfoData = httpContext.Request.Headers.GetXInfoHeaderData();
        diagnosticContext.Set("Environment", XInfoData.Environment);
        diagnosticContext.Set("Platform", XInfoData.Platform);
        diagnosticContext.Set("DeviceName", XInfoData.DeviceName);
        diagnosticContext.Set("AppVersion", XInfoData.AppVersion);
        diagnosticContext.Set("DeviceId", XInfoData.DeviceId);
    };
});

app.MapHub<BalootGamesHub>("/baloot-games-hub");

app.MapControllers();

app.Run();