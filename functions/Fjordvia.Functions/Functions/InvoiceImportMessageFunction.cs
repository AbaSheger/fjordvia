using System.Text.Json;
using Fjordvia.Core.Integrations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Fjordvia.Functions.Functions;

public sealed class InvoiceImportMessageFunction(ILogger<InvoiceImportMessageFunction> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Function(nameof(InvoiceImportMessageFunction))]
    public void Run(
        [ServiceBusTrigger("%ServiceBus:InvoiceImportQueueName%", Connection = "ServiceBus:ConnectionString")]
        string messageBody)
    {
        var message = JsonSerializer.Deserialize<InvoiceImportMessage>(messageBody, JsonOptions);
        if (message is null)
        {
            logger.LogWarning("Received an invoice import message that could not be deserialized.");
            return;
        }

        logger.LogInformation(
            "Received invoice import message for invoice {ExternalInvoiceNumber} ({InvoiceId}). Partner {BusinessPartnerName} ({BusinessPartnerOrganizationNumber}), amount {TotalAmount} {Currency}, route {SourceSystem} to {TargetSystem}.",
            message.ExternalInvoiceNumber,
            message.InvoiceId,
            message.BusinessPartnerName,
            message.BusinessPartnerOrganizationNumber,
            message.TotalAmount,
            message.Currency,
            message.SourceSystem,
            message.TargetSystem);
    }
}
