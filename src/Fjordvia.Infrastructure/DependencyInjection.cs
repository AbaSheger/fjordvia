using Azure.Messaging.ServiceBus;
using Fjordvia.Core.Interfaces;
using Fjordvia.Infrastructure.Data;
using Fjordvia.Infrastructure.Messaging;
using Fjordvia.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fjordvia.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FjordviaDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'FjordviaDatabase' is required.");
        }

        services.AddDbContext<FjordviaDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IIntegrationLogRepository, IntegrationLogRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddIntegrationMessagePublisher(configuration);

        return services;
    }

    private static IServiceCollection AddIntegrationMessagePublisher(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceBusConnectionString = configuration["ServiceBus:ConnectionString"];
        var invoiceImportQueueName = configuration["ServiceBus:InvoiceImportQueueName"];

        if (string.IsNullOrWhiteSpace(serviceBusConnectionString) ||
            string.IsNullOrWhiteSpace(invoiceImportQueueName))
        {
            services.AddSingleton<IIntegrationMessagePublisher, LocalIntegrationMessagePublisher>();
            return services;
        }

        services.AddSingleton(_ => new ServiceBusClient(serviceBusConnectionString));
        services.AddSingleton<IIntegrationMessagePublisher>(provider =>
            new AzureServiceBusIntegrationMessagePublisher(
                provider.GetRequiredService<ServiceBusClient>(),
                invoiceImportQueueName,
                provider.GetRequiredService<
                    Microsoft.Extensions.Logging.ILogger<AzureServiceBusIntegrationMessagePublisher>>()));

        return services;
    }
}
