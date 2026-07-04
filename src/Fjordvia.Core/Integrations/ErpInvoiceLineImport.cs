namespace Fjordvia.Core.Integrations;

public sealed record ErpInvoiceLineImport(string Description, int Quantity, decimal UnitPrice);
