using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Fjordvia.Core.Integrations;
using Fjordvia.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fjordvia.Infrastructure.Messaging;

public sealed class AzureServiceBusIntegrationMessagePublisher(
    ServiceBusClient serviceBusClient,
    string queueName,
    ILogger<AzureServiceBusIntegrationMessagePublisher> logger)
    : IIntegrationMessagePublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task PublishInvoiceImportedAsync(InvoiceImportMessage message, CancellationToken cancellationToken)
    {
        await using var sender = serviceBusClient.CreateSender(queueName);
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message, JsonOptions))
        {
            ContentType = "application/json",
            Subject = nameof(InvoiceImportMessage),
            MessageId = message.InvoiceId.ToString()
        };

        serviceBusMessage.ApplicationProperties["externalInvoiceNumber"] = message.ExternalInvoiceNumber;
        serviceBusMessage.ApplicationProperties["sourceSystem"] = message.SourceSystem;
        serviceBusMessage.ApplicationProperties["targetSystem"] = message.TargetSystem;

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

        logger.LogInformation(
            "Published invoice import message {MessageId} to Azure Service Bus queue {QueueName}.",
            serviceBusMessage.MessageId,
            queueName);
    }
}
