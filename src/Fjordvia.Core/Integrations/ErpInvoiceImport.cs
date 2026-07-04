namespace Fjordvia.Core.Integrations;

public sealed record ErpInvoiceImport(
    string ExternalInvoiceNumber,
    string CustomerName,
    string CustomerOrganizationNumber,
    string CustomerEmail,
    string CountryCode,
    string Currency,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    IReadOnlyCollection<ErpInvoiceLineImport> Lines);
