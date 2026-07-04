namespace Fjordvia.Core.Integrations;

public sealed record AccountingInvoiceExport(
    string InvoiceNumber,
    string CustomerAccount,
    string Currency,
    decimal NetAmount,
    DateOnly PostingDate,
    DateOnly DueDate);
