namespace Fjordvia.Core.Integrations;

public sealed record CrmInvoiceActivity(
    string CustomerName,
    string CustomerEmail,
    string ActivityType,
    string Summary,
    decimal Amount,
    DateOnly ActivityDate);
