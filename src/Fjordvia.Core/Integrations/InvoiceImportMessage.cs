namespace Fjordvia.Core.Integrations;

public sealed record InvoiceImportMessage(
    Guid InvoiceId,
    Guid BusinessPartnerId,
    string ExternalInvoiceNumber,
    string BusinessPartnerName,
    string BusinessPartnerOrganizationNumber,
    string Currency,
    decimal TotalAmount,
    string SourceSystem,
    string TargetSystem,
    DateTimeOffset ImportedAt);
