using Fjordvia.Core.Integrations;
using Fjordvia.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fjordvia.Infrastructure.Messaging;

public sealed class LocalIntegrationMessagePublisher(ILogger<LocalIntegrationMessagePublisher> logger)
    : IIntegrationMessagePublisher
{
    public Task PublishInvoiceImportedAsync(InvoiceImportMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Local integration message recorded for invoice {ExternalInvoiceNumber} ({InvoiceId}) from {SourceSystem} to {TargetSystem}.",
            message.ExternalInvoiceNumber,
            message.InvoiceId,
            message.SourceSystem,
            message.TargetSystem);

        return Task.CompletedTask;
    }
}
