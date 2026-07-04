using Fjordvia.Api.Middleware;
using Fjordvia.Core.Mapping;
using Fjordvia.Core.Services;
using Fjordvia.Infrastructure;
using Fjordvia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
const string AngularDevelopmentCorsPolicy = "AngularDevelopment";
var configuredCorsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];
var angularCorsOrigins = configuredCorsOrigins.Length > 0
    ? configuredCorsOrigins
    : ["http://localhost:4200", "http://127.0.0.1:4200"];
var shouldUseCors = builder.Environment.IsDevelopment() || configuredCorsOrigins.Length > 0;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularDevelopmentCorsPolicy, policy =>
    {
        policy.WithOrigins(angularCorsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ErpInvoiceMapper>();
builder.Services.AddScoped<BusinessPartnerService>();
builder.Services.AddScoped<InvoiceImportService>();
builder.Services.AddScoped<IntegrationRetryService>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (shouldUseCors)
{
    app.UseCors(AngularDevelopmentCorsPolicy);
}

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Database:EnsureCreatedOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FjordviaDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
