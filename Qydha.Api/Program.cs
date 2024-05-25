FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase_private_key.json")
});

var builder = WebApplication.CreateBuilder(args);

ReadingConfigurationFile.ReadConfigurationFile(builder.Configuration, builder.Environment);
string MyAllowSpecificOrigins = builder.Services.ConfigureCORS();
LoggerServiceExtension.AddLoggerConfiguration(builder.Configuration, builder.Environment);
builder.Host.UseSerilog();
builder.Services.DbConfiguration(builder.Configuration);
builder.Services.RegisterServices(builder.Configuration);
builder.Services.RegisterRepos();
builder.Services.ConfigureControllers();
builder.Services.ConfigureFluentValidation();
builder.Services.RegisterFilters();
builder.Services.RegisterAttributes();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureMediatR();
builder.Services.RegisterSettings(builder.Configuration);

var app = builder.Build();


if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestContextLogging();

app.UseCors(MyAllowSpecificOrigins);

app.UseStaticFiles();

app.UseSerilogRequestLogging(op =>
{
    op.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms {NewLine} => UserId : {UserId} {NewLine} => Client IP : {ClientIp} {NewLine} => X-INFO : {xINFO} {NewLine} => TraceId : {RequestId} ";
    op.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("xINFO", httpContext.Request.Headers["x-info"].ToString());
        diagnosticContext.Set("ClientIp", httpContext.Request.Headers["X-Real-IP"].ToString());
        diagnosticContext.Set("UserId", httpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "user Id not provided");
        diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
        diagnosticContext.Set("NewLine", "\n");
    };
});

app.MapControllers();

app.Run();