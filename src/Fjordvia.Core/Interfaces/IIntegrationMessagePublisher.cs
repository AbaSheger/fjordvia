using Fjordvia.Core.Integrations;

namespace Fjordvia.Core.Interfaces;

public interface IIntegrationMessagePublisher
{
    Task PublishInvoiceImportedAsync(InvoiceImportMessage message, CancellationToken cancellationToken);
}
