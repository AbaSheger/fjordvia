using Fjordvia.Api.Middleware;
using Fjordvia.Core.Mapping;
using Fjordvia.Core.Services;
using Fjordvia.Infrastructure;
using Fjordvia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
const string AngularDevelopmentCorsPolicy = "AngularDevelopment";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularDevelopmentCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
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
    app.UseCors(AngularDevelopmentCorsPolicy);

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FjordviaDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
