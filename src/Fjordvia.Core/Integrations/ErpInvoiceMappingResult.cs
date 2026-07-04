using Fjordvia.Core.Domain;

namespace Fjordvia.Core.Integrations;

public sealed record ErpInvoiceMappingResult(
    Invoice Invoice,
    AccountingInvoiceExport AccountingInvoice,
    CrmInvoiceActivity CrmActivity);
