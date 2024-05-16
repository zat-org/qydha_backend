FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase_private_key.json")
});

var builder = WebApplication.CreateBuilder(args);

builder.AddLoggerService();
builder.ConfigureDb();
builder.ReadConfigrationFile();
string MyAllowSpecificOrigins = builder.ConfigureCORS();

builder.Services.RegisterServices();
builder.Services.RegisterRepos();
builder.Services.ConfigureControllers();
builder.Services.ConfigureFluentValidation();
builder.Services.RegisterFilters();
builder.Services.RegisterAttributes();
builder.Services.ConfigureSwagger();
builder.Services.ConfigreMediatR();
builder.Services.RegisterSettings(builder.Configuration);

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
        diagnosticContext.Set("UserId", httpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "user Id not provided");
        diagnosticContext.Set("NewLine", "\n");
    };
});

app.MapControllers();

app.Run();



